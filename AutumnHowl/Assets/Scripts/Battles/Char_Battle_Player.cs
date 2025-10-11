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

public class Char_Battle_Player : Char_Battle
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
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
            TryMoveInDirection(Vector2Int.up);
        }
        else if (GameInstance.Inputs.MoveDown.WasPressedThisFrame())
        {
            TryMoveInDirection(Vector2Int.down);
        }
        else if (GameInstance.Inputs.MoveLeft.WasPressedThisFrame())
        {
            TryMoveInDirection(Vector2Int.left);
        }
        else if (GameInstance.Inputs.MoveRight.WasPressedThisFrame())
        {
            TryMoveInDirection(Vector2Int.right);
        }
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


    #endregion
}
