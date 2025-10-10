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

public abstract class Char_Battle : Character
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public bool canMove;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GridPawn gridPawnController;
    public BattleGridPather gridPather;
    public BattleGrid battleGrid;
    public BattleStateController battleStateController;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public override void Start()
    {
        base.Start();
        gridPather = FindObjectOfType<BattleGridPather>();
        battleGrid = FindObjectOfType<BattleGrid>();
        battleStateController = FindObjectOfType<BattleStateController>();
    }
    
    private void Update()
    {
        if (isDead) return;
        if (!canMove) return;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    protected virtual bool TryMoveInDirection(Vector2Int _direction)
    {
        var testPos = gridPawnController.position + _direction;
        if (BattleGrid.Instance.ValidTile (testPos.x, testPos.y) && !BattleGrid.Instance.IsOccupied(testPos.x, testPos.y))
        {
            gridPawnController.MoveToTile (testPos.x, testPos.y);
            battleStateController.NextTurnStep();
            return true;
        }

        return false;
    }
    
    protected virtual bool TryMoveTo(Vector2Int _direction)
    {
        if (BattleGrid.Instance.ValidTile (_direction.x, _direction.y) && !BattleGrid.Instance.IsOccupied(_direction.x, _direction.y))
        {
            gridPawnController.MoveToTile (_direction.x, _direction.y);
            battleStateController.NextTurnStep();
            return true;
        }

        return false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual void SetTurnActive(bool _isTurnActive)
    {
        canMove = _isTurnActive;
    }


    #endregion
}
