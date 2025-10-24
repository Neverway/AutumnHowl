//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once HollowTypeName
public class BattleStateController : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public int currentWave;
    public PlayerAction playerAction {get; set;}
    public enum PlayerAction
    {
        attack,
        spell,
        item,
        defend,
    }
    // Used during defending to see if the player gets their bonus power for no hit
    public bool playerWasHitThisStep;

    public int stepsRemaining;
    //Counts upward how many turns the wave has been going for.
    public int waveStepCount = 0;
    public List<Char_Battle> turnOrder;
    public int currentTurn = 0;

    /// <summary>
    /// Triggered when a wave starts.
    /// </summary>
    public UnityEvent OnStartWave = new UnityEvent();


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private BattleState currentBattleState;
    private bool initialized;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GI_AuHoGameState gameState;
    public Func_TextEvent textEvent;
    public WB_Battle battleWidget;
    public Char_Battle_Player battlePlayer;
    public BattleGrid battleGrid;
    private Coroutine nextTurnCoroutine;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private IEnumerator Start()
    {
        if (textEvent == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Text Event is not set");
        if (battleWidget == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Widget is not set");
        if (battlePlayer == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Player is not set");
        if (battleGrid == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Grid is not set");

        //Debug.Log("BattleSystem Start (wait)");
        yield return new WaitUntil(()=>BattleGrid.Instance != null);
        //Debug.Log("BattleSystem Start (continue)");
        gameState = GameInstance.Get<GI_AuHoGameState>();
        currentBattleState = new BS_Start(this);
        currentBattleState.OnStateEnter(null);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;
        currentBattleState.OnStateUpdate();
    }
    private void OnDestroy()
    {
        //Debug.Log("Battle destroyed!");
    }

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void NewState(BattleState newBS)
    {
        BattleState oldBS = currentBattleState;
        oldBS.OnStateLeave(newBS);

        currentBattleState = newBS;
        currentBattleState.OnStateEnter(oldBS);
    }

    public void SetPlayerAction(int _action)
    {
        switch (_action)
        {
            case 0:
                playerAction = PlayerAction.attack;
                break;
            case 2:
                playerAction = PlayerAction.spell;
                break;
            case 1:
                playerAction = PlayerAction.item;
                break;
            case 3:
                playerAction = PlayerAction.defend;
                break;
        }
        NewState(new BS_GridAction(this));
    }

    public IEnumerator CoNextTurnStep(float _delay = 0.1f)
    {
        // Disable movement for the current character
        turnOrder[currentTurn].SetTurnActive(false);
        //print($"Ending {turnOrder[currentTurn].gameObject.name}'s turn");

        yield return new WaitForSeconds(_delay);
        
        // If there are more characters waiting for their turn
        if (currentTurn + 1 < turnOrder.Count)
        {
            currentTurn++;
            // Enable movement for the next character
            //print($"Started {turnOrder[currentTurn].gameObject.name}'s turn");
            turnOrder[currentTurn].SetTurnActive(true);
        }
        
        // If there are no more characters waiting for their turn, end the step
        else
        {
            stepsRemaining--;
            waveStepCount++;
            currentTurn = 0;
            turnOrder[0].SetTurnActive(true);
            //print($"All turns completed, going to step {stepsRemaining}");
        }
        CheckForVictory();
        nextTurnCoroutine = null;
        
    }

    public void NextTurnStep(float _delay=0.1f)
    {
        if (nextTurnCoroutine == null) nextTurnCoroutine = StartCoroutine(CoNextTurnStep(_delay));
    }

    public void AddCharacter (Char_Battle _char_Battle, Vector2Int _position)
    {            
        // Create the enemy on the grid
        var newEnemy = battleGrid.InstantiatePawn (
        _position,
        _char_Battle.gameObject);

        // Add the enemy to be next in the turn order
        turnOrder.Add (newEnemy.GetComponent<Char_Battle>());
    }

    internal void RemoveCharacter (Char_Battle char_Battle)
    {
        turnOrder.Remove (char_Battle);
    }

    private void CheckForVictory()
    {
        var currentBattle = GameInstance.Get<GI_AuHoGameState>().currentGameState.currentBattle;
        List<Char_Battle> aliveCharacters = turnOrder.Where(_character => !_character.isDead).ToList();

        currentBattle.victoryState.victoryConditionMet(aliveCharacters);
    }


    #endregion
}