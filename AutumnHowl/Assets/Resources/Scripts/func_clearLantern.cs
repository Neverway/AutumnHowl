using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class func_clearLantern : MonoBehaviour
{
    public void ClearLantern()
    {
        GameInstance.Gamestate.currentLanternTime = 0;
    }
}
