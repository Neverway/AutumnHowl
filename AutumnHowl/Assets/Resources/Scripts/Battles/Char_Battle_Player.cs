//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


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
        facingDirection = new Vector2(0, 1);
        invertAttackFacingDirections = true;
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
        animator.SetFloat("idleX", facingDirection.x);
        animator.SetFloat("idleY", facingDirection.y);
        if (!canMove) return;
        UpdateMovementInput();
    }

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    private void UpdateMovementInput()
    {
        if (inputDelay) return;

        // MOVEMENT
        Tuple<InputAction, Vector2Int>[] inputToDirection =
        {
            new(GameInstance.Inputs.MoveUp,    Vector2Int.up),
            new(GameInstance.Inputs.MoveDown,  Vector2Int.down),
            new(GameInstance.Inputs.MoveLeft,  Vector2Int.left),
            new(GameInstance.Inputs.MoveRight, Vector2Int.right)
        };
        foreach (var inputToDir in inputToDirection)
        {
            InputAction input = inputToDir.Item1;
            Vector2Int direction = inputToDir.Item2;

            if (input.WasPressedThisFrame())
            {
                if (TryMoveInDirection(direction, true, gridPawnController)) facingDirection = direction;
                return;
            }
        }

        // DEFEND
        if (isDefenseActive)
        {
            if (GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                SpinBlock (SpinDirection.Left);
            }
            else if (GameInstance.Inputs.Action.WasPressedThisFrame ())
            {
                SpinBlock(SpinDirection.Right);
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
    public override void SetTurnActive(bool _isTurnActive)
    {
        canMove = _isTurnActive;
        if (_isTurnActive)
        {
            battleStateController.battleWidget.attackCompass.ReEnableCompassInputs();
        }
    }
    
    public void PerformAttack(int _attackType, bool mirrorX = false, bool mirrorY = false)
    {
        if (!canMove) return;
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
        //if (!canMove) return;
        battleStateController.NextTurnStep (0.5f);
    }

    private void SpinBlock(SpinDirection _direction)
    {
        switch (_direction)
        {
            case SpinDirection.Left:
                {
                    //rotate 90 degrees left
                    facingDirection = Vector2.Perpendicular (facingDirection);
                    battleStateController.NextTurnStep (0.5f);
                    break;
                }
            case SpinDirection.Right:
                {
                    //rotate 90 degrees right
                    facingDirection = -Vector2.Perpendicular (facingDirection);
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