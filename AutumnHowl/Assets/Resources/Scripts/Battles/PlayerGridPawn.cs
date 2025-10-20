using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGridPawn : GridPawn
{
    public bool canMove;
    private void Update ()
    {
        if (!canMove) return;
        
        if (Input.GetKeyDown (KeyCode.LeftArrow))
        {
            if (BattleGrid.Instance.ValidTile (position.x - 1, position.y) && ! BattleGrid.Instance.IsOccupied(position.x - 1, position.y))
            {
                MoveToTile (position.x - 1, position.y);
            }
        }
        if (Input.GetKeyDown (KeyCode.RightArrow))
        {
            if (BattleGrid.Instance.ValidTile (position.x + 1, position.y) && !BattleGrid.Instance.IsOccupied (position.x + 1, position.y))
            {
                MoveToTile (position.x + 1, position.y);
            }
        }
        if (Input.GetKeyDown (KeyCode.UpArrow))
        {
            if (BattleGrid.Instance.ValidTile (position.x, position.y + 1) && !BattleGrid.Instance.IsOccupied (position.x, position.y + 1))
            {
                MoveToTile (position.x, position.y + 1);
            }
        }
        if (Input.GetKeyDown (KeyCode.DownArrow))
        {
            if (BattleGrid.Instance.ValidTile (position.x, position.y - 1) && !BattleGrid.Instance.IsOccupied (position.x, position.y - 1))
            {
                MoveToTile (position.x, position.y - 1);
            }
        }
    }
}
