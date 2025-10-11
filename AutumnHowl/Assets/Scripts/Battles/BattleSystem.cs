//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

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

    public int stepsRemaining;
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

    public IEnumerator CoNextTurnStep()
    {
        // Disable movement for the current character
        turnOrder[currentTurn].SetTurnActive(false);
        
        yield return null;
        
        // If there are more characters waiting for their turn
        if (currentTurn + 1 < turnOrder.Count)
        {
            currentTurn++;
            // Enable movement for the next character
            turnOrder[currentTurn].SetTurnActive(true);
        }
        
        // If there are no more characters waiting for their turn, end the step
        else
        {
            stepsRemaining--;
            currentTurn = 0;
            turnOrder[0].SetTurnActive(true);
        }
    }

    public void NextTurnStep()
    {
        StartCoroutine(CoNextTurnStep());
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
        // Add the player to be first in the turn order
        controller.turnOrder.Add(controller.battlePlayer);
        
        // Create the enemy on the grid
        var newEnemy = controller.battleGrid.InstantiatePawn(
            controller.gameState.currentGameState.currentBattle.enemyStartPosition,
            controller.gameState.currentGameState.currentBattle.enemyPrefab);
        
        // Add the enemy to be next in the turn order
        controller.turnOrder.Add(newEnemy.GetComponent<Char_Battle>());
        
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
                break;
        }
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
        }
        controller.battlePlayer.canMove = false;
    }
}