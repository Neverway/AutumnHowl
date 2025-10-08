using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPawn : MonoBehaviour
{
    public Vector2Int position { get; private set; }
    private bool initialized = false;

    public void Start ()
    {
        //If position isn't set, assume this object hasn't been added to a BattleGrid and call the initialize coroutine.
        if (initialized == false)
        {
            StartCoroutine (AutoInitPawn ());
        }
    }
    /// <summary>
    /// Waits until BattleGrid Instance is not null before adding itself to the Battle Grid based on it's position.
    /// This is because BattleGrid's Start function needs to be called before we add Pawns.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoInitPawn ()
    {
        yield return new WaitUntil(()=>BattleGrid.Instance != null);
        transform.SetParent (BattleGrid.Instance.gameObject.transform);
        position = new Vector2Int (Mathf.FloorToInt (transform.localPosition.x), Mathf.FloorToInt (transform.localPosition.y));
        BattleGrid.Instance.AddPawnToGrid (this);
    }

    /// <summary>
    /// BattleGrid calls this in the case that the pawn doesn't need to initialize itself.
    /// </summary>
    public void InitPawn ()
    {
        initialized = true;
    }

    public void MoveToTile (int _x, int _y)
    {
        if (BattleGrid.Instance.ValidTile(_x, _y) == false)
        {
            Debug.LogError ("GridPawn " + gameObject.name + " tried to move out of bounds to" + _x + "," + _y);
            return;
        }
        BattleGrid.Instance.MovePawn (position.x, position.y, _x, _y, this);
    }

    public void SetPosition (Vector2Int _position)
    {
        position = _position;
        transform.localPosition = new Vector3 (_position.x, _position.y, transform.localPosition.z);
    }
}
