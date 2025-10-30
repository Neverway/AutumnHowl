using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    public int width;
    public int height;
    private BattleTile[,] grid;
    public static BattleGrid Instance { get; private set; }

    void Start ()
    {
        if (Instance != null)
        {
            Debug.LogError ("Only one BattleGrid should exist at a time");
            Destroy (this);
            return;
        }
        Instance = this;
        grid = new BattleTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new BattleTile();
            }
        }
    }

    public GameObject InstantiatePawn (Vector2Int _position, GameObject _pawn) {
        var newObject = Instantiate (_pawn, transform);
        GridPawn newPawn = newObject.GetComponent<GridPawn>();
        //print(newPawn);
        newPawn.SetPosition(_position);
        grid[_position.x, _position.y].pawns.Add(newPawn);

        newPawn.InitPawn ();
        return newObject;
    }

    internal void MovePawn (int _x, int _y, int _x2, int _y2, GridPawn gridPawn)
    {
        grid[_x,_y].pawns.Remove(gridPawn);
        grid[_x2,_y2].pawns.Add(gridPawn);
        gridPawn.SetPosition(new Vector2Int (_x2, _y2));
    }
    public bool IsOccupied (int _x, int _y)
    {
        if (ValidTile (_x, _y) == false)
        {
            return false;
        }
        if (grid[_x, _y].pawns.Count > 0)
        {
            if (grid[_x, _y].pawns[0].type == GridPawn.GridPawnType.attack) return false;
            else return true;
        }
        return false;
    }
    /// <summary>
    /// Allows using a Vector2Int for IsOccupied
    /// </summary>
    /// <param name="_tile"></param>
    /// <returns></returns>
    public bool IsOccupied(Vector2Int _tile)
    {
        return IsOccupied(_tile.x, _tile.y);
    }
    public GridPawn GetIsOccupied (Vector2Int _gridPosition)
    {
        if (ValidTile (_gridPosition.x, _gridPosition.y) == false)
        {
            return null;
        }
        if (grid[_gridPosition.x, _gridPosition.y].pawns.Count > 0)
        {
            return grid[_gridPosition.x, _gridPosition.y].pawns[0];
        }
        return null;
    }
    
    public bool IsMoveable (int _x, int _y)
    {
        if (ValidTile (_x, _y) == false)
        {
            return false;
        }
        if (grid[_x, _y].pawns.Count > 0)
        {
            return false;
        }
        return true;
    }

    internal bool ValidTile(int _x, int _y)
    {
        if (_x < 0
            || _y < 0
            || _x >= width
            || _y >= height)
        {
            return false;
        }
        return true;
    }

    public bool IsPathable (int _x, int _y)
    {
        if (ValidTile (_x, _y) == false)
        {
            return false;
        }
        if (grid[_x, _y].pawns.Count > 0)
        {
            //Return true for tiles with enemies.
            var c = grid[_x, _y].pawns[0].GetComponent<Character> ();
            if (c != null && c.HasTag (CharacterTags.Enemy)) {
                return true;
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Add the pawn to the grid based on the pawn's position.
    /// </summary>
    /// <param name="gridPawn"></param>
    internal void AddPawnToGrid (GridPawn gridPawn)
    {
        grid[gridPawn.position.x, gridPawn.position.y].pawns.Add (gridPawn);
    }

    internal void RemovePawnFromGrid (GridPawn gridPawn)
    {
        grid[gridPawn.position.x, gridPawn.position.y].pawns.Remove (gridPawn);
    }

    internal GridPawn GetPawn(int x, int y)
    {
        if (ValidTile(x, y) && grid[x, y].pawns.Count > 0)
        {
            return grid[x, y].pawns[0];
        }
        return null;
    }
}

public class BattleTile
{
    public List<GridPawn> pawns = new List<GridPawn>();
}
