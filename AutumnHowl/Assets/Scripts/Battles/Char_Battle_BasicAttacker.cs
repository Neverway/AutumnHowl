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

public class Char_Battle_BasicAttacker : Char_Battle
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private bool TestTile(Vector2Int checkPos, int lowestTileNumber)
    {
        if (battleGrid.IsMoveable(checkPos.x, checkPos.y))
        {
            if (gridPather.grid[checkPos.x, checkPos.y] < lowestTileNumber)
            {
                return true;
            }
        }

        return false;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private Vector2Int GetLowestTileToTarget()
    {
        var x = gridPawnController.position.x;
        var y = gridPawnController.position.y;
        var lowestTileNumber = 9999;
        var lowestTile = new Vector2Int(-1,-1);
        
        Vector2Int checkPos;

        // Check surrounding tiles
        checkPos = new Vector2Int(x + 1, y);
        if (TestTile(checkPos, lowestTileNumber))
        {
            lowestTileNumber = gridPather.grid[checkPos.x, checkPos.y];
            lowestTile = checkPos;
        }
        checkPos = new Vector2Int(x - 1, y);
        if (TestTile(checkPos, lowestTileNumber))
        {
            lowestTileNumber = gridPather.grid[checkPos.x, checkPos.y];
            lowestTile = checkPos;
        }
        checkPos = new Vector2Int(x, y + 1);
        if (TestTile(checkPos, lowestTileNumber))
        {
            lowestTileNumber = gridPather.grid[checkPos.x, checkPos.y];
            lowestTile = checkPos;
        }
        checkPos = new Vector2Int(x, y - 1);
        if (TestTile(checkPos, lowestTileNumber))
        {
            lowestTileNumber = gridPather.grid[checkPos.x, checkPos.y];
            lowestTile = checkPos;
        }

        return lowestTile;
    }
    
    private void TakeTurn()
    {
        print ($"Pos {gridPawnController.position} | Tar {GetLowestTileToTarget()}");
        if (!TryMoveTo(GetLowestTileToTarget()))
        {
            print($"{gameObject.name} couldn't find a path to target, skipping turn");
            battleStateController.NextTurnStep();
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public override void SetTurnActive(bool _isTurnActive)
    {
        canMove = _isTurnActive;
        if (_isTurnActive)
        {
            TakeTurn();
        }
    }


    #endregion
}
