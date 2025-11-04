using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerStartPos : MonoBehaviour
{
    //The BS_Start looks for this object and just moves the player to this object

    public const string REFERENCE_ID = "PlayerStartPos";

    public void Awake()
    {
        IDToObj<BattlePlayerStartPos>.ClearNew();
        IDToObj<BattlePlayerStartPos>.TryAdd(REFERENCE_ID, this);
    }
    public void OnDestroy()
    {
        IDToObj<PuzzleFlag>.ClearNew();
    }
}
