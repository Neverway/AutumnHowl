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

public class Controller_Overworld_NPC : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public float walkSpeed = 2;
    public float sprintSpeed = 4;
    public Vector2 movement;
    public bool frozen;
    /*
    public FaceDirection faceDirection;
    public enum FaceDirection
    {
        up,
        down,
        left,
        right
    }*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private float currentMoveSpeed;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private Rigidbody2D _rigidbody;
    [SerializeField] private Animator animator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        currentMoveSpeed = walkSpeed;
        if (frozen)
        {
            animator.SetBool("walking", false);
            animator.SetFloat("idleX", movement.x);
            animator.SetFloat("idleY", movement.y);
            return;
        }

        UpdateMovementInput();
    }

    private void FixedUpdate()
    {
        if (frozen)
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
