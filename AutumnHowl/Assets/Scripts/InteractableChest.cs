using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChest : MonoBehaviour
{
    public Animator animator;
    public string openChestTrigger;
    public void TriggerOpenChest()
    {
        animator.SetTrigger(openChestTrigger);
    }
}
