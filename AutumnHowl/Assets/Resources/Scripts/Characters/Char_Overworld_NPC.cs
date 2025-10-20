//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Overworld_NPC : Character
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public bool frozen;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Update()
    {
        currentMoveSpeed = Stats.walkSpeed;
        if (frozen || isDead)
        {
            animator.SetBool("walking", false);
            //animator.SetFloat("idleX", movement.x);
            //animator.SetFloat("idleY", movement.y);
            return;
        }

        UpdateMovementInput();
    }

    private void FixedUpdate()
    {
        if (frozen || isDead)
        {
            _rigidbody.velocity = new Vector2();
            return;
        }
        _rigidbody.velocity = movement * currentMoveSpeed;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateMovementInput()
    {
        animator.SetFloat("walkX", movement.x);
        animator.SetFloat("walkY", movement.y);
        animator.SetBool("walking", movement.x != 0 || movement.y != 0);
        if (animator.GetBool("walking"))
        {
            animator.SetFloat("idleX", movement.x);
            animator.SetFloat("idleY", movement.y);
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
