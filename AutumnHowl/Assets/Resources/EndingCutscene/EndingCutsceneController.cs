using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCutsceneController : MonoBehaviour
{
    public bool waitingForInput = false;
    public Animator cutsceneAnimator;
    public string triggerAnimator = "ThrowSword";
    void Update()
    {
        if (waitingForInput && GameInstance.Inputs.Interact.WasPressedThisFrame())
        {
            cutsceneAnimator.SetTrigger(triggerAnimator);
            waitingForInput = false;
        }

    }

    public void RollCredits()
    {
        GameInstance.Get<GI_WorldLoader>().Load("End Credits");
    }
}
