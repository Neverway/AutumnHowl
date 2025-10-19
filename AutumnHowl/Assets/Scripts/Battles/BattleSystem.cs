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
using UnityEngine;

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


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private BattleState currentBattleState;
    private bool initialized;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GI_AuHoGameState gameState;
    public Func_TextEvent textEvent;
    public WB_Battle battleWidget;
    public Char_Battle_Player battlePlayer;
    public BattleGrid battleGrid;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private IEnumerator Start()
    {
        if (textEvent == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Text Event is not set");
        if (battleWidget == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Widget is not set");
        if (battlePlayer == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Player is not set");
        if (battleGrid == null) throw new NullReferenceException($"{nameof(BattleStateController)}: Battle Grid is not set");

        Debug.Log("BattleSystem Start (wait)");
        yield return new WaitUntil(()=>BattleGrid.Instance != null);
        Debug.Log("BattleSystem Start (continue)");
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
        Debug.Log("Battle destroyed!");
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
            waveStepCount++;
            currentTurn = 0;
            turnOrder[0].SetTurnActive(true);
            print($"All turns completed, going to step {stepsRemaining}");
        }
    }

    public void NextTurnStep(float _delay=0.1f)
    {
        StartCoroutine(CoNextTurnStep(_delay));
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


    #endregion
}

/// <summary>
/// A structure for states the battle can be in
/// </summary>
public abstract class BattleState
{
    [Tooltip("A reference to the battle controller, so states can access it")]
    public BattleStateController controller;

    public BattleState(BattleStateController _controller)
    {
        this.controller = _controller;
    }

    public abstract void OnStateEnter(BattleState stateLeaving);
    public abstract void OnStateUpdate();
    public abstract void OnStateLeave(BattleState stateEntering);
}


// -----------------------------------
// A WHOLE BUNCH OF BS DOWN HERE
// -----------------------------------


/// <summary>
/// 
/// </summary>
public class BS_Start : BattleState
{
    public BS_Start(BattleStateController controller) : base(controller)
    {
    }

    public override void OnStateEnter(BattleState stateLeaving)
    {
        Debug.Log($"BS_START_ENTER: {controller.name}", controller);
        // Add the player to be first in the turn order
        controller.turnOrder.Add(controller.battlePlayer);

        // Add enemies to the battle
        foreach (var enemy in controller.gameState.currentGameState.currentBattle.enemySpawnLocations)
        {
            controller.AddCharacter(enemy.enemyPrefab.GetComponent<Char_Battle>(), enemy.enemyStartPosition);
        }
        
        // Display opening text
        controller.textEvent.textEvent = controller.gameState.currentGameState.currentBattle.openingText;
        controller.textEvent.textEvent.OnFinish.AddListener(() =>
        {
            controller.NewState(new BS_PlayerAction(controller));
        });
        controller.textEvent.CallEvent();
        
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
        Debug.Log("BS_START_LEAVE", controller);
        controller.textEvent.textEvent.OnFinish.RemoveAllListeners();
    }
}

/// <summary>
/// 
/// </summary>
public class BS_PlayerAction : BattleState
{
    public BS_PlayerAction(BattleStateController controller) : base(controller)
    {
    }

    public override void OnStateEnter(BattleState stateLeaving)
    {
        controller.battleWidget.SetActionBarVisible(true);
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
        controller.battleWidget.SetActionBarVisible(false);
        switch (controller.playerAction)
        {
            case BattleStateController.PlayerAction.attack:
                controller.battleWidget.SetAttackBarVisible(true);
                controller.battlePlayer.useBlock = false;
                break;
            case BattleStateController.PlayerAction.defend:
                controller.battlePlayer.ModifyPower(10);
                controller.battlePlayer.isDefenseActive = true;
                controller.battlePlayer.useBlock = true;
                controller.playerWasHitThisStep = false;
                controller.battlePlayer.OnHurt -= SetPlayerHitThisStep; //b-but why?
                controller.battlePlayer.OnHurt += SetPlayerHitThisStep;
                break;
        }
    }

    public void SetPlayerHitThisStep()
    {
        controller.playerWasHitThisStep = true;
    }
}

/// <summary>
/// 
/// </summary>
public class BS_GridAction : BattleState
{
    private BattleWave activeWave;
    private bool initialized = false;
    
    public BS_GridAction(BattleStateController controller) : base(controller)
    {
    }

    public override void OnStateEnter(BattleState stateLeaving)
    {
        activeWave = controller.gameState.currentGameState.currentBattle.battleSequence.GetBattleWave();
        controller.stepsRemaining = activeWave.waveSteps;
        controller.battlePlayer.canMove = true;
        controller.waveStepCount = 0;
    }

    public override void OnStateUpdate()
    {
        controller.battleWidget.stepCountText.text = controller.stepsRemaining.ToString();
        if (controller.stepsRemaining <= 0)
        {
            controller.NewState(new BS_PlayerAction(controller));
        }
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
        switch (controller.playerAction)
        {
            case BattleStateController.PlayerAction.attack:
                controller.battleWidget.SetAttackBarVisible(false);
                break;
            case BattleStateController.PlayerAction.defend:
                controller.battlePlayer.isDefenseActive = false;
                // Give no-hit bonus
                if (controller.playerWasHitThisStep == false)
                {
                    controller.battlePlayer.ModifyPower(5);
                }
                break;
        }
        controller.battlePlayer.canMove = false;
    }
}