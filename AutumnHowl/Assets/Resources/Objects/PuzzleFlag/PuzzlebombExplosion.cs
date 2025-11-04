using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlebombExplosion : MonoBehaviour
{
    public void Start()
    {
        GameInstance.Playerbody.Stats.ModifyHealth(-9999);
    }
}
