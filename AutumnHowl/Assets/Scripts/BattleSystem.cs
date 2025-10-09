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


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private BattleState currentBattleState;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GI_AuHoGameState gameState;
    public Func_TextEvent textEvent;
    public WB_Battle battleWidget;
    public Char_Battle_Player battlePlayer;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
        print(gameState);
        currentBattleState = new BS_Start(this);
        currentBattleState.OnStateEnter(null);
    }

    private void Update()
    {
        currentBattleState.OnStateUpdate();
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
        Debug.Log("Left Play Act");
        controller.battleWidget.SetActionBarVisible(false);
    }
}

/// <summary>
/// 
/// </summary>
public class BS_GridAction : BattleState
{
    public BS_GridAction(BattleStateController controller) : base(controller)
    {
    }

    public override void OnStateEnter(BattleState stateLeaving)
    {
        controller.battlePlayer.canMove = true;
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
        controller.battlePlayer.canMove = false;
    }
}