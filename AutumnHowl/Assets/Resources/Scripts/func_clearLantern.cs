using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class func_clearLantern : MonoBehaviour
{
    public void ClearLantern()
    {
        GameInstance.Get<GI_AudioManager>().SetAmbiencePitch(0.2f);
        GameInstance.Gamestate.currentLanternTime = 0;
    }
}
