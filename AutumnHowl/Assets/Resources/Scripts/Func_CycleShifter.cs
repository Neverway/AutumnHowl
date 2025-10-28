using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_CycleShifter : MonoBehaviour
{
    public void ShiftToNextCycle()
    {
        GameInstance.Get<GI_AuHoGameState>().currentGameState.NextCycle();
    }
    public void SetCurrentCycle()
    {
        GameInstance.Get<GI_AuHoGameState>().currentGameState.SetCurrentCycle();
    }
}
