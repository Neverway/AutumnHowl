using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=========== Created by Connorses ===========/

/// <summary>
/// Script to attach to enemies I want to sit still ("sleep") until the player gets near.
/// </summary>
public class OverworldSleeperEnemy : MonoBehaviour
{
    //=========== Reference Variables ===========/
    [Tooltip ("The animator of the wake up animation.")]
    [SerializeField] private Animator animator;
    [Tooltip("The original enemy sprite; gets turned off until enemy wakes up.")]
    [SerializeField] private SpriteRenderer originalSprite;
    [Tooltip("list of things that turn on when this enemy wakes up")]
    [SerializeField] private GameObject[] objectsToActivate;

    private Controller_Overworld_Player autumnController;
    private Rigidbody2D rb;

    //=========== Private Variables ===========/

    [SerializeField] private float wakeDistance = 4f;

    private bool awake=false;

    // Start is called before the first frame update
    void Start()
    {
        autumnController = FindObjectOfType<Controller_Overworld_Player>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        originalSprite.enabled = false;

        animator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (awake) return;
        if ((autumnController.transform.position - transform.position).magnitude < wakeDistance)
        {
            WakeUp();
        }
    }

    public void WakeUp ()
    {
        StartCoroutine (WakeUpRoutine ());
    }

    /// <summary>
    /// Brush teeth, put on pants, get out of bed (in that order).
    /// </summary>
    /// <returns></returns>
    private IEnumerator WakeUpRoutine ()
    {
        animator.speed = 1f;
        yield return new WaitForSeconds (0.3f);
        foreach (GameObject obj in objectsToActivate) {
            obj.SetActive (true);
        }
        yield return new WaitForSeconds (1f);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        originalSprite.enabled = true;
        animator.gameObject.SetActive (false);
        awake = true;
    }


}
