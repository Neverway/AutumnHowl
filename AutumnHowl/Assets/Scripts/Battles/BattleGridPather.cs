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
using UnityEngine;

public class BattleGridPather : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public int[,] grid;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private BattleGrid battleGrid;
    private const int UnassignedTileNumber=99;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public void Start()
    {
        battleGrid = FindObjectOfType<BattleGrid>();
        grid = new int[battleGrid.width, battleGrid.height];
        SetDefaultTileWeights();
    }



    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// The lower the number, the closer to the target, so this sets the default tile values to something really high
    /// </summary>
    private void SetDefaultTileWeights()
    {
        for (int y = 0; y < battleGrid.height;y++)
        {
            for (int x = 0; x < battleGrid.width; x++)
            {
                grid[x, y] = UnassignedTileNumber;
            }
        }
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="n"></param>
    /// <returns>Returns true if an un-numbered tile was found</returns>
    private bool CheckSurrondingTiles(int x, int y, int n)
    {
        if (grid[x,y] != n) return false;
        
        bool foundTile = false;
        
        Vector2Int checkPos;

        // Check surrounding tiles
        checkPos = new Vector2Int(x + 1, y);
        if (battleGrid.IsMoveable(checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            foundTile = true;
        }
        checkPos = new Vector2Int(x - 1, y);
        if (battleGrid.IsMoveable(checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            foundTile = true;
        }
        checkPos = new Vector2Int(x, y + 1);
        if (battleGrid.IsMoveable(checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            foundTile = true;
        }
        checkPos = new Vector2Int(x, y - 1);
        if (battleGrid.IsMoveable(checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            foundTile = true;
        }
        
        return foundTile;
    }
    
    private void RecursivePather(int n)
    {
        bool shouldContinue = false;
        
        for (int ix = 0; ix < battleGrid.width; ix++)
        {
            for (int iy = 0; iy < battleGrid.height; iy++)
            {
                if (CheckSurrondingTiles(ix, iy, n))
                {
                    shouldContinue = true;
                }
            }
        }
        
        if (!shouldContinue) return;
        
        n++;
        RecursivePather(n);
    }
    
    private void PrintDistances ()
    {
        string p = "\n";
        for (int y = 0; y < battleGrid.height;y++)
        {
            for (int x = 0; x < battleGrid.width; x++)
            {
                p += grid[x, y].ToString ("D2")+",";
            }
            p += "\n";
        }
        print (p);
    }

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void GetPathToTarget(GridPawn _targetGridPawn)
    {
        grid = new int[battleGrid.width, battleGrid.height];
        SetDefaultTileWeights();
        
        grid[_targetGridPawn.position.x, _targetGridPawn.position.y]=1;
        RecursivePather(1);
        PrintDistances();
    }


    #endregion
}
