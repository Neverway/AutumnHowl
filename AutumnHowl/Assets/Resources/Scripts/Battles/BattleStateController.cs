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
[RequireComponent(typeof(Func_TextEvent))]
public class BattleStateController : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [Tooltip("The action the player chose for this round")]
    public PlayerAction currentPlayerAction {get; set;}
    public enum PlayerAction
    {
        attack,
        spell,
        item,
        defend,
    }
    [Tooltip("Used during defending to see if the player gets their bonus power for no hit")]
    public bool playerWasHitThisStep;
    [Tooltip("How many turns each character gets before the end of this round")]
    public int stepsRemaining;
    [Tooltip("A list of each character that's currently fighting, in order of who's turn comes first")]
    public List<Char_Battle> turnOrder; // <= BattleSystem.cs sets the contents of this in its battle start state
    [Tooltip("Used to store who's turn it is in the turn order list")]
    public int currentTurn = 0;
    [Tooltip("Used by bosses to spawn their wave attacks")]
    public UnityEvent OnStartWave;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    [Tooltip("The active state of the battle state machine")]
    private BattleState currentBattleState;
    [Tooltip("Used to prevent calling update on a battle state, if the state machine hasn't been initialized")]
    private bool initialized;
    public Coroutine nextTurnCoroutine;
    public int pendingNextTurnCalls;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Func_TextEvent textEvent;
    public GI_AuHoGameState gameState;
    public WB_Battle battleWidget;
    public Char_Battle_Player battlePlayer;
    public BattleGrid battleGrid;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private IEnumerator Start()
    {
        CheckAndReportNullReferences();
        
        // Wait for the battle grid to initialize
        yield return new WaitUntil(()=>BattleGrid.Instance != null);
        
        
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

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Throw understandable error messages for missing references
    /// </summary>
    private void CheckAndReportNullReferences()
    {
        if (textEvent == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Text Event is not set");
        if (battleWidget == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Widget is not set");
        if (battlePlayer == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Player is not set");
        if (battleGrid == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Grid is not set");
    }
    
    /// <summary>
    /// See if the current battle data's victory state has been reached,
    /// If so, switch to the victory state in the battle state machine
    /// </summary>
    private void CheckForVictory()
    {
        var currentBattle = gameState.currentGameState.currentBattle;
        List<Char_Battle> aliveCharacters = turnOrder.Where(_character => !_character.isDead).ToList();


        if (currentBattle.victoryState.victoryConditionMet(aliveCharacters))
        {
            ChangeState(new BS_Victory(this));
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Switch the current state on the battle state machine
    /// </summary>
    public void ChangeState(BattleState newBS)
    {
        BattleState oldBS = currentBattleState;
        oldBS.OnStateLeave(newBS);

        currentBattleState = newBS;
        currentBattleState.OnStateEnter(oldBS);
    }

    /// <summary>
    /// Sets the player's action choice and switches the state machine to the grid action state
    /// Called by WB_Battle's action button unity events,
    /// </summary>
    /// <param name="_action"></param>
    public void SetPlayerAction(int _action)
    {
        // TODO this could probably be added to those unity events via code rather than an inspector reference
        switch (_action)
        {
            case 0:
                currentPlayerAction = PlayerAction.attack;
                break;
            case 2:
                currentPlayerAction = PlayerAction.spell;
                break;
            case 1:
                currentPlayerAction = PlayerAction.item;
                break;
            case 3:
                currentPlayerAction = PlayerAction.defend;
                break;
        }
        ChangeState(new BS_GridAction(this));
    }

    public IEnumerator CoNextTurnStep(float _delay = 0.1f)
    {
        // Disable movement for the current character
        turnOrder[currentTurn].SetTurnActive(false);
        print($"Ending {turnOrder[currentTurn].gameObject.name}'s turn");

        yield return new WaitForSeconds(_delay);
        
        // If there are more characters waiting for their turn
        if (currentTurn + 1 < turnOrder.Count)
        {
            currentTurn++;
            // Enable movement for the next character
            print($"Started {turnOrder[currentTurn].gameObject.name}'s turn");
            turnOrder[currentTurn].SetTurnActive(true);
        }
        
        // If there are no more characters waiting for their turn, end the step
        else
        {
            stepsRemaining--;
            currentTurn = 0;
            turnOrder[0].SetTurnActive(true);
            print($"All turns completed, going to step {stepsRemaining}");
        }
        print($"nextTurnCoroutine Completed!");
        nextTurnCoroutine = null;
        CheckForVictory();

        if (pendingNextTurnCalls > 0)
        {
            pendingNextTurnCalls--;
            nextTurnCoroutine = StartCoroutine(CoNextTurnStep());
        }
    }

    public void NextTurnStep(float _delay=0.1f, string caller="")
    {
        print($"BS {caller} Called next turn step");
        if (nextTurnCoroutine == null)
        {
            print($"nextTurnCoroutine started!");
            nextTurnCoroutine = StartCoroutine(CoNextTurnStep(_delay));
        }
        else
        {
            pendingNextTurnCalls++;
            print($"nextTurnCoroutine failed, caching the request to start the call to waiting list");
        }
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


    public void LoadLayout()
    {
        Instantiate( gameState.currentGameState.currentBattle.layoutPrefab, battleGrid.transform);
    }


    #endregion
}