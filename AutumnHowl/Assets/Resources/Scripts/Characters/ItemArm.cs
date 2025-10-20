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

public class ItemArm : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private Animator charAnimator;
    public GameObject charObject;
    public Animator itemAnimator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
    }

    public void Update()
    {
        if (!charAnimator)
        {
            print("Char anim not found");
            //charAnimator = charObject.GetComponent<Animator>();
            print(charAnimator);
            return;
        }
        itemAnimator.SetFloat("idleX", charAnimator.GetFloat("idleX"));
        itemAnimator.SetFloat("idleY", charAnimator.GetFloat("idleY"));
        itemAnimator.SetFloat("walkX", charAnimator.GetFloat("walkX"));
        itemAnimator.SetFloat("walkY", charAnimator.GetFloat("walkY"));
        itemAnimator.SetBool("walking", charAnimator.GetBool("walking"));
    }



    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
