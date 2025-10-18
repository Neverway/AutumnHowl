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
using System.IO;
using UnityEngine;

public class Char_Battle_Player : Char_Battle , IsPlayerCharacter
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private new void Start()
    {
        movement = new Vector2(0, 1);
        base.Start();
    }
    
    private void Update()
    {
        animator.SetFloat("idleX", movement.x);
        animator.SetFloat("idleY", movement.y);
        if (isDead)
        {
            GameInstance.Get<GI_WorldLoader>().Load("GameOver");
            return;
        }
        if (!canMove) return;
        UpdateMovementInput();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateMovementInput()
    {
        if (GameInstance.Inputs.MoveUp.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.up)) movement = new Vector2(0, 1);
            return;
        }
        else if (GameInstance.Inputs.MoveDown.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.down)) movement = new Vector2(0, -1);
            return;
        }
        else if (GameInstance.Inputs.MoveLeft.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.left)) movement = new Vector2(-1, 0);
            return;
        }
        else if (GameInstance.Inputs.MoveRight.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.right)) movement = new Vector2(1, 0);
            return;
        }

        if (isDefenseActive)
        {
            if (GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                SpinBlock ("left");
            }
            else if (GameInstance.Inputs.Action.WasPressedThisFrame ())
            {
                SpinBlock("right");
            }
        }
    }

    protected override bool TryMoveInDirection (Vector2Int _direction, bool doNextTurn = true, GridPawn _pathTargetPawn = null)
    {
        bool oldResult = base.TryMoveInDirection(_direction, doNextTurn, gridPawnController);
        return oldResult;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void PerformAttack(int _attackType, bool mirrorX = false, bool mirrorY = false)
    {
        if (isDead) battleStateController.NextTurnStep();
        
        SetAttackDamageToCurrentATK();
        
        switch (_attackType)
        {
            case 0:
                TryAttackSequence(AttackSequences[0], mirrorX, mirrorY);
                break;
            case 1:
                TryAttackSequence(AttackSequences[1], mirrorX, mirrorY);
                break;
            case 2:
                TryAttackSequence(AttackSequences[2], mirrorX, mirrorY);
                break;
            case 3:
                TryAttackSequence(AttackSequences[3], mirrorX, mirrorY);
                break;
        }
    }
    
    public void PerformGeneratedAttack()
    {
        if (isDead) battleStateController.NextTurnStep();
        
        SetAttackDamageToCurrentATK();
        TryAttackSequence(AttackSequences[0]);
        //We regenerate paths since the sword can affect pathing
        gridPather.GetPathToTarget (gridPawnController);
    }

    /// <summary>
    /// Skip this turn of the battle.
    /// </summary>
    public void SkipTurn ()
    {
        battleStateController.NextTurnStep (0.5f);
    }

    private void SpinBlock(string _direction)
    {
        switch (_direction)
        {
            case "left":
                {
                    //rotate 90 degrees left
                    movement = Vector2.Perpendicular (movement);
                    battleStateController.NextTurnStep (0.5f);
                    break;
                }
            case "right":
                {
                    //rotate 90 degrees right
                    movement = -Vector2.Perpendicular (movement);
                    battleStateController.NextTurnStep (0.5f);
                    break;
                }
        }
    }

    #endregion
}
