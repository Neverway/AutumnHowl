using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Func_CycleShifter : MonoBehaviour
{
    
    private IEnumerator ShiftAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameInstance.Get<GI_AuHoGameState>().currentGameState.NextCycle();
    }
    
    public void ShiftToNextCycle()
    {
        GameInstance.Get<GI_TransitionManager>().Fadecross();
        GameInstance.SendCoroutine(ShiftAfterDelay(1f));
    }
    public void SetCurrentCycle()
    {
        GameInstance.Get<GI_AuHoGameState>().currentGameState.SetCurrentCycle();
    }
}
