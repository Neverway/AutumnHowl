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
        if (isDead) return;
        if (!canMove) return;
        UpdateMovementInput();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateMovementInput()
    {
        if (GameInstance.Inputs.MoveUp.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.up)) movement = new Vector2(0, 1);
            
        }
        else if (GameInstance.Inputs.MoveDown.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.down)) movement = new Vector2(0, -1);
            
        }
        else if (GameInstance.Inputs.MoveLeft.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.left)) movement = new Vector2(-1, 0);
            
        }
        else if (GameInstance.Inputs.MoveRight.WasPressedThisFrame())
        {
            if (TryMoveInDirection(Vector2Int.right)) movement = new Vector2(1, 0); 
            
        }
        animator.SetFloat("idleX", movement.x);
        animator.SetFloat("idleY", movement.y);
    }
    
    protected override bool TryMoveInDirection(Vector2Int _direction)
    {
        gridPather.GetPathToTarget(gridPawnController);
        bool oldResult = base.TryMoveInDirection(_direction);
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
    }


    #endregion
}
