//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Volume_TriggerInteract : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public bool repeatable;
    public UnityEvent OnInteract = new UnityEvent();


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool activated;
    private bool inTrigger;
    private Coroutine resetCoroutine;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Update()
    {
        if (inTrigger && !activated)
        {
            if (GameInstance.Get<GI_TextboxManager>().textEventActive)
                return;

            if (GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                activated = true;
                OnInteract?.Invoke();
                if (repeatable && resetCoroutine == null) ResetActive();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    public IEnumerator CO_ResetActive()
    {
        yield return new WaitForSeconds(0.2f);
        activated = false;
        resetCoroutine = null;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void ResetActive()
    {
        resetCoroutine = GameInstance.SendCoroutine(CO_ResetActive());
    }

    #endregion
}
