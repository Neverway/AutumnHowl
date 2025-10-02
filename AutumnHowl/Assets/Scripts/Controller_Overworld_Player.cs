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

public class Controller_Overworld_Player : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public float movementSpeed=3;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private Vector2 movement;
    private bool inMenu;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private InputActions.TopDownActions inputActions;
    private Rigidbody2D _rigidbody;
    [SerializeField] private GameObject inventoryWidget;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        // Setup inputs
        inputActions = new InputActions().TopDown;
        inputActions.Enable();

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Menu pausing
        if (inputActions.Select.WasPressedThisFrame())
        {
            inventoryWidget.SetActive(!inventoryWidget.activeInHierarchy);
            inMenu = inventoryWidget.activeInHierarchy;
        }
        
        if (inMenu) return;
        UpdateMovementInput();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = movement*movementSpeed;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateMovementInput()
    {
        if (inputActions.MoveUp.IsPressed()) movement.y = 1;
        else if (inputActions.MoveDown.IsPressed()) movement.y = -1;
        else movement.y = 0;
        
        if (inputActions.MoveLeft.IsPressed()) movement.x = -1;
        else if (inputActions.MoveRight.IsPressed()) movement.x = 1;
        else movement.x = 0;
    }
    

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
