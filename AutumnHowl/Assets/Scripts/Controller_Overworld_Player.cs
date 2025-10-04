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
    public float walkSpeed = 2;
    public float sprintSpeed = 4;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [SerializeField] private bool canPause = true;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private Vector2 movement;
    private float currentMoveSpeed;
    private bool inMenu;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private InputActions.TopDownActions inputActions;
    private Rigidbody2D _rigidbody;
    private GameObject inventoryWidget;
    private GI_AuHoGameState gameState;


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
        UpdatePausingInput();

        if (inMenu)
        {
            return;
        }

        UpdateStoredOverworldPosition();
        UpdateMovementInput();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = movement * currentMoveSpeed;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdatePausingInput()
    {
        if (inputActions.Select.WasPressedThisFrame() && canPause)
        {
            if (!inventoryWidget)
            {
                var widgetManager = GameInstance.Get<GI_WidgetManager>();
                widgetManager.AddWidget("WB_Inventory");
                inventoryWidget = widgetManager.GetExistingWidget("WB_Inventory");
            }
            movement = new Vector2(0,0); // Clear Movement 
            inventoryWidget.SetActive(!inventoryWidget.activeInHierarchy);
            inMenu = inventoryWidget.activeInHierarchy;
        }
    }

    private void UpdateStoredOverworldPosition()
    {
        if (gameState) gameState.currentGameState.overworldPosition = new Vector2(transform.position.x, transform.position.y);
        else gameState = GameInstance.Get<GI_AuHoGameState>();
    }
    
    private void UpdateMovementInput()
    {
        if (inputActions.MoveUp.IsPressed()) movement.y = 1;
        else if (inputActions.MoveDown.IsPressed()) movement.y = -1;
        else movement.y = 0;
        
        if (inputActions.MoveLeft.IsPressed()) movement.x = -1;
        else if (inputActions.MoveRight.IsPressed()) movement.x = 1;
        else movement.x = 0;

        if (inputActions.Action.IsPressed()) currentMoveSpeed = sprintSpeed;
        else currentMoveSpeed = walkSpeed;
    }
    

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
