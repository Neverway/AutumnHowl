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
        //Debug.Log($"BS_START_ENTER: {controller.name}", controller);
        // Add the player to be first in the turn order
        controller.turnOrder.Add(controller.battlePlayer);

        // Add enemies to the battle
        foreach (var enemy in controller.gameState.currentGameState.currentBattle.enemySpawnLocations)
        {
            controller.AddCharacter(enemy.enemyPrefab.GetComponent<Char_Battle>(), enemy.enemyStartPosition);
        }
        // Add layout to battle
        controller.LoadLayout();
        
        // Display opening text
        controller.textEvent.textEvent = controller.gameState.currentGameState.currentBattle.openingText;
        controller.textEvent.textEvent.OnFinish.AddListener(() =>
        {
            controller.ChangeState(new BS_PlayerAction(controller));
        });
        controller.textEvent.CallEvent();
        
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
        //Debug.Log("BS_START_LEAVE", controller);
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
        switch (controller.currentPlayerAction)
        {
            case BattleStateController.PlayerAction.attack:
                controller.battleWidget.SetAttackBarVisible(true);
                controller.battlePlayer.useBlock = false;
                break;
            case BattleStateController.PlayerAction.defend:
                controller.battlePlayer.ModifyPower(20);
                controller.battlePlayer.isDefenseActive = true;
                controller.battlePlayer.StartCoroutine(controller.battlePlayer.InputDelay());
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
    
    public BS_GridAction(BattleStateController controller) : base(controller)
    {
    }

    public override void OnStateEnter(BattleState stateLeaving)
    {
        activeWave = controller.gameState.currentGameState.currentBattle.battleSequence.GetBattleWave();
        controller.stepsRemaining = activeWave.waveSteps;
        controller.battlePlayer.canMove = true;
        controller.OnStartWave?.Invoke();
    }

    public override void OnStateUpdate()
    {
        controller.battleWidget.stepCountText.text = controller.stepsRemaining.ToString();
        if (controller.stepsRemaining <= 0)
        {
            controller.ChangeState(new BS_PlayerAction(controller));
        }
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
        switch (controller.currentPlayerAction)
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


/// <summary>
/// 
/// </summary>
public class BS_Victory : BattleState
{
    public BS_Victory(BattleStateController controller) : base(controller)
    {
    }

    public override void OnStateEnter(BattleState stateLeaving)
    {
        // Display opening text
        controller.textEvent.textEvent = new TextEvent();
        controller.textEvent.textEvent.AddFrame(
            "You Won!\n{col=stat}[+" +
            $"{GameInstance.Gamestate.currentBattle.victoryLevels}" +
            $" LEVELS]\n[+${GameInstance.Gamestate.currentBattle.victoryGold}]");
           
        
        controller.textEvent.textEvent.OnFinish.AddListener(() =>
        {
            //controller.NewState(new BS_PlayerAction(controller));
        });
        controller.textEvent.CallEvent();
    }

    public override void OnStateUpdate()
    {
        if (!GameInstance.Get<GI_TextboxManager>().textEventActive)
        {
            GameInstance.Gamestate.LeaveBattle();
        }
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
    }
}