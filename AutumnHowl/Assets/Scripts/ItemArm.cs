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
    public Animator charAnimator;
    public Animator itemAnimator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    public void Update()
    {
        itemAnimator.SetFloat("idleX", charAnimator.GetFloat("idleX"));
        itemAnimator.SetFloat("idleY", charAnimator.GetFloat("idleY"));
        itemAnimator.SetFloat("walkX", charAnimator.GetFloat("walkX"));
        itemAnimator.SetFloat("walkY", charAnimator.GetFloat("walkY"));
        itemAnimator.SetBool("walking", charAnimator.GetBool("walking"));
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
