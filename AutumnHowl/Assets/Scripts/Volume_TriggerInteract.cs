//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Volume_TriggerInteract : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public bool repeatable;
    public UnityEvent OnInteract = new UnityEvent();
    public bool activated;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    public bool inTrigger;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Update()
    {
        if (inTrigger && !activated)
        {
            if (GameInstance.Inputs.Interact.WasPressedThisFrame())
            {
                activated = true;
                OnInteract.Invoke();
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
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void ResetActive()
    {
        StartCoroutine(CO_ResetActive());
    }

    #endregion
}
