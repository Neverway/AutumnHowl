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

    //percent chance that the character tries to back away instead of attacking.
    [SerializeField] private int randomRetreat;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/

    public bool skipFirstTurn = false;

    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    protected bool TestTile(Vector2Int checkPos, int lowestTileNumber)
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

    protected bool TestForEnemy(Vector2Int checkPos)
    {
        var pawnAtTile = battleGrid.GetIsOccupied(new Vector2Int(checkPos.x, checkPos.y));
        if (pawnAtTile)
        {
            if (pawnAtTile.pawnName == "Autumn")
            {
                return true;
            }
        }

        return false;
    }

    protected Vector2Int GetLowestTileToTarget()
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

    protected string GetTarget()
    {
        var x = gridPawnController.position.x;
        var y = gridPawnController.position.y;
        Vector2Int checkPos;

        if (TestForEnemy(new Vector2Int(x, y+1))) return "north";
        if (TestForEnemy(new Vector2Int(x, y-1))) return "south";
        if (TestForEnemy(new Vector2Int(x+1, y))) return "east";
        if (TestForEnemy(new Vector2Int(x-1, y))) return "west";
        return "none";
    }
    
    private void TakeTurn()
    {
        if (isDead) battleStateController.NextTurnStep();
        else if (skipFirstTurn)
            {
                skipFirstTurn = false;
                battleStateController.NextTurnStep ();
                return;
            }
        
        SetAttackDamageToCurrentATK();
        var x = gridPawnController.position.x;
        var y = gridPawnController.position.y;
        // If target is in range, randomly decide to attack or back away
        switch (GetTarget())
        {
            case "north":
                if (Random.Range(0, 100) < randomRetreat == false)
                {
                    TryAttackSequence(AttackSequences[0]);
                    return;
                }
                if (TryMoveTo(new Vector2Int(x+0, y+-1))) { return; }
                break;
            case "south":
                if (Random.Range (0, 100) < randomRetreat == false)
                {
                    TryAttackSequence(AttackSequences[1]);
                    return;
                }
                if (TryMoveTo(new Vector2Int(x+0, y+1))) { return; }
                break;
            case "east":
                if (Random.Range (0, 100) < randomRetreat == false)
                {
                    TryAttackSequence(AttackSequences[2]);
                    return;
                }
                if (TryMoveTo(new Vector2Int(x+-1, y+0))) { return; }
                break;
            case "west":
                if (Random.Range (0, 100) < randomRetreat == false)
                {
                    TryAttackSequence(AttackSequences[3]);
                    return;
                }
                if (TryMoveTo(new Vector2Int(x+1, y+0))) { return; }
                break;
        }
        
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
