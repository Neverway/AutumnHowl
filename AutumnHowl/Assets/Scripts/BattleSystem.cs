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


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private BattleState currentBattleState;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GI_AuHoGameState gameState;
    public Func_TextEvent textEvent;
    public Animator ChoiceBoxAnimator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
        currentBattleState = new BS_Start(this);
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
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
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
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
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
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateLeave(BattleState stateEntering)
    {
    }
}