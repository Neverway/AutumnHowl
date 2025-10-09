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

public class Char_Battle_Player : Character
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public bool canMove;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private GridPawn gridPawnController;


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
            TryMove(Vector2Int.up);
        }
        if (GameInstance.Inputs.MoveDown.WasPressedThisFrame())
        {
            TryMove(Vector2Int.down);
        }
        if (GameInstance.Inputs.MoveLeft.WasPressedThisFrame())
        {
            TryMove(Vector2Int.left);
        }
        if (GameInstance.Inputs.MoveRight.WasPressedThisFrame())
        {
            TryMove(Vector2Int.right);
        }
    }

    private void TryMove(Vector2Int _direction)
    {
        var testPos = gridPawnController.position + _direction;
        if (BattleGrid.Instance.ValidTile (testPos.x, testPos.y) && !BattleGrid.Instance.IsOccupied(testPos.x, testPos.y))
        {
            gridPawnController.MoveToTile (testPos.x, testPos.y);
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
