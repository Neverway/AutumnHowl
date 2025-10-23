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

public class Char_Battle_Badger : Char_Battle
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/

    //percent chance that the character tries to back away instead of attacking.
    [SerializeField] private int randomRetreat;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/

    public bool skipFirstTurn = false;

    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/

    private bool canLunge = false;

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
        var lowestTileNumber = 9999;
        var lowestTile = new Vector2Int(-1,-1);

        // Check surrounding tiles
        DirectionUtility.ForEachDirection((direction) =>
        {
            Vector2Int checkPos = gridPawnController.position + direction.Info().direction;
            if (TestTile(checkPos, lowestTileNumber))
            {
                lowestTileNumber = gridPather.grid[checkPos.x, checkPos.y];
                lowestTile = checkPos;
            }
        });
        return lowestTile;
    }

    protected Direction? GetTarget()
    {
        Direction? targetDirection = null;
        DirectionUtility.ForEachDirection(direction =>
        {
            if (TestForEnemy(gridPawnController.position + direction.Info().direction))
            {
                targetDirection = direction;
                canLunge = false;
            }
            //For this enemy we test 2 tiles at ahead as well, because he can lunge forwards
            if (TestForEnemy(gridPawnController.position + direction.Info().direction * 2))
            {
                if ( battleGrid.IsOccupied( gridPawnController.position + direction.Info().direction )==false)
                {
                    targetDirection = direction;
                    canLunge = true;
                }
            }
        });

        return targetDirection;
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

        // If target is in range, randomly decide to attack or back away
        Direction? targetDirection = GetTarget();
        if (targetDirection != null)
        {
            //Only retreat if we can't do the lunge attack AND we do the dice roll
            if (!(canLunge==false && Random.Range(0, 100) < randomRetreat))
            {
                GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.growl);
                switch (targetDirection)
                {
                    case Direction.North:
                        {
                            TryMoveInDirection(Vector2Int.up, false);
                            TryAttackSequence(AttackSequences[0]);
                            break;
                        }
                    case Direction.South:
                        {
                            TryMoveInDirection(Vector2Int.down, false);
                            TryAttackSequence(AttackSequences[1]);
                            break;
                        }
                    case Direction.East:
                        {
                            TryMoveInDirection(Vector2Int.right, false);
                            TryAttackSequence(AttackSequences[2]);
                            break;
                        }
                    case Direction.West:
                        {
                            TryMoveInDirection(Vector2Int.left, false);
                            TryAttackSequence(AttackSequences[3]);
                            break;
                        }
                }
                return;
            }

            Vector2Int toPosition = gridPawnController.position;
            toPosition += targetDirection.Value.Info().direction * -1;

            if (TryMoveTo(toPosition)) 
                return;
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
