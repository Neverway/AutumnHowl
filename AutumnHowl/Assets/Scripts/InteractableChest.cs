using UnityEngine;

[SelectionBase]
public class InteractableChest : MonoBehaviour
{
    public Animator animator;
    public string openChestTrigger;
    public GameObject sparkles;
    public void TriggerOpenChest()
    {
        sparkles.SetActive(false);
        Debug.Log("Triggered!");
        animator.SetTrigger(openChestTrigger);
    }
}
