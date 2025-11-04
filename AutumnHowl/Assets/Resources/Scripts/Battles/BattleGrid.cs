using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    public int width;
    public int height;
    private BattleTile[,] grid;
    public static BattleGrid Instance { get; private set; }

    [Reload]
    public static List<Vector2Int> playerAttackPositions; //Stores tiles recently attacked by the player, used for reflecting boss projetiles...
    private const float playerAttackPositionLifetime = 0.2f;

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
        playerAttackPositions = new List<Vector2Int> ();
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = transform.position + new Vector3(x, y, 0);
                Vector3 corner_NE = pos + new Vector3(+0.5f, +0.5f, 0);
                Vector3 corner_SE = pos + new Vector3(-0.5f, +0.5f, 0);
                Vector3 corner_NW = pos + new Vector3(+0.5f, -0.5f, 0);
                Vector3 corner_SW = pos + new Vector3(-0.5f, -0.5f, 0);

                Gizmos.DrawLine(corner_NE, corner_SE);
                Gizmos.DrawLine(corner_NW, corner_SW);
                Gizmos.DrawLine(corner_NE, corner_NW);
                Gizmos.DrawLine(corner_SE, corner_SW);
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

    public void AddPlayerAttackPosition (Vector2Int _pos)
    {
        print ("AddPlayerAttackPosition " + _pos);
        StartCoroutine (TemporarilyAddAttackPosition (_pos));
    }

    private IEnumerator TemporarilyAddAttackPosition(Vector2Int _pos)
    {
        playerAttackPositions.Add (_pos);
        yield return new WaitForSeconds (playerAttackPositionLifetime);
        playerAttackPositions.Remove(_pos);
    }
}

public class BattleTile
{
    public List<GridPawn> pawns = new List<GridPawn>();
}
