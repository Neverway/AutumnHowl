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
using DG.Tweening;
using UnityEngine;

public class Char_Battle_Player : Char_Battle , IsPlayerCharacter
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool inTheProcessOfDying; // Used to block inputs while the player's death animation is playing out
    private bool inputDelay; // Used to block inputs when the player first gains control to avoid accidental inputs


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
        if (isDead && !inTheProcessOfDying)
        {
            inTheProcessOfDying = true;
            StartCoroutine(Die());
            return;
        }
        animator.SetFloat("idleX", movement.x);
        animator.SetFloat("idleY", movement.y);
        if (!canMove) return;
        UpdateMovementInput();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateMovementInput()
    {
        if (inputDelay) return;
        
        // MOVEMENT
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

        // DEFEND
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
        
        // PASS TURN
        if (GameInstance.Inputs.Select.WasPressedThisFrame())
        {
            battleStateController.NextTurnStep(0.5f);
        }
    }

    /// <summary>
    /// Play a little death animation and switch to the game over screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator Die()
    {
        gameObject.transform.DORotate(new Vector3(45, 0, 0), 0.25f);
        yield return new WaitForSeconds(1);
        GameInstance.Get<GI_WorldLoader>().Load("GameOver");
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void PerformAttack(int _attackType, bool mirrorX = false, bool mirrorY = false)
    {
        if (isDead) battleStateController.NextTurnStep();
        
        SetAttackDamageToCurrentATK();
        
        //Try first 4 AttackSequences
        for (int i = 0; i < 4; i++)
            TryAttackSequence(AttackSequences[i], mirrorX, mirrorY);
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

    /// <summary>
    /// Blocks inputs when the player's turn starts to avoid accidental inputs from being registered
    /// </summary>
    public IEnumerator InputDelay()
    {
        inputDelay = true;
        yield return new WaitForSeconds(0.1f);
        inputDelay = false;
    }

    #endregion
}
