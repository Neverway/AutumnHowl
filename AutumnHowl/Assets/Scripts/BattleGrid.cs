using System;
using System.Collections;
using System.Collections.Generic;
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
        print(newPawn);
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
            return true;
        }
        return false;
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

    internal void AddPawnToGrid (GridPawn gridPawn)
    {
        grid[gridPawn.position.x, gridPawn.position.y].pawns.Add (gridPawn);
    }
}

public class BattleTile
{
    public List<GridPawn> pawns = new List<GridPawn>();
}
