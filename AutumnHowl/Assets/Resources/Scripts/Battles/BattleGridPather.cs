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

    private Vector2Int swordPosition;

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/

    private BattleGrid battleGrid;
    private const int UnassignedTileNumber=99;
    private const int SwordCost = 4;

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
        if (battleGrid.IsPathable (checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            //Make the sword cost extra
            if (checkPos == swordPosition)
            {
                grid[checkPos.x, checkPos.y] = n + SwordCost;
            }
            foundTile = true;
        }
        checkPos = new Vector2Int(x - 1, y);
        if (battleGrid.IsPathable(checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            if (checkPos == swordPosition)
            {
                grid[checkPos.x, checkPos.y] = n + SwordCost;
            }
            foundTile = true;
        }
        checkPos = new Vector2Int(x, y + 1);
        if (battleGrid.IsPathable (checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            if (checkPos == swordPosition)
            {
                grid[checkPos.x, checkPos.y] = n + SwordCost;
            }
            foundTile = true;
        }
        checkPos = new Vector2Int(x, y - 1);
        if (battleGrid.IsPathable (checkPos.x, checkPos.y) && grid[checkPos.x, checkPos.y] == UnassignedTileNumber)
        {
            grid[checkPos.x, checkPos.y] = n+1;
            if (checkPos == swordPosition)
            {
                grid[checkPos.x, checkPos.y] = n + SwordCost;
            }
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
    
    /// <summary>
    /// Prints the path grid for debugging
    /// </summary>
    private void PrintDistances ()
    {
        string p = "DISTANCES:\n";
        for (int y = battleGrid.height-1; y > -1; y--)
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
        swordPosition = GetSwordPosition (_targetGridPawn);
        RecursivePather(1);
        //PrintDistances();
    }

    /// <summary>
    /// find the position of the tile where the player's sword should be (can be out-of-bounds)
    /// </summary>
    /// <param name="_targetGridPawn"></param>
    /// <returns></returns>
    public Vector2Int GetSwordPosition (GridPawn _targetGridPawn)
    {
        //If we failed getting a Character for some reason, return (-1,-1),
        //which is out-of-bounds and won't affect pathing.
        var character = _targetGridPawn.GetComponent<Character> ();
        if (character == null)
        {
            Debug.LogError ("You shouldn't be seeing this.");
            return new Vector2Int(-1,-1);
        }
        //-movement gives the relative position of big sword
        Vector2 movement = -character.movement;
        Vector2Int movementInt = new Vector2Int ((int)movement.x, (int)movement.y);
        return _targetGridPawn.position + movementInt;
    }


    #endregion
}
