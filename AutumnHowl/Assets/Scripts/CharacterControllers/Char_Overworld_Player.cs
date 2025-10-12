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

public class Controller_Overworld_Player : Character , IsPlayerCharacter
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [SerializeField] private bool canPause = true;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool inMenu;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private GameObject inventoryWidget;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Update()
    {
        if (isDead) return;
        
        // Menu pausing
        UpdatePausingInput();

        if (inMenu)
        {
            animator.SetBool("walking", false);
            return;
        }
        
        gameState.currentGameState.overworldPosition = new Vector2(transform.position.x, transform.position.y);

        UpdateMovementInput();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        _rigidbody.velocity = movement * currentMoveSpeed;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdatePausingInput()
    {
        if (GameInstance.Inputs.Select.WasPressedThisFrame() && canPause)
        {
            if (!inventoryWidget)
            {
                var widgetManager = GameInstance.Get<GI_WidgetManager>();
                widgetManager.AddWidget("WB_Inventory");
                inventoryWidget = widgetManager.GetExistingWidget("WB_Inventory");
                inventoryWidget.SetActive(!inventoryWidget.activeInHierarchy);
            }
            movement = new Vector2(0,0); // Clear Movement 
            inventoryWidget.SetActive(!inventoryWidget.activeInHierarchy);
            inMenu = inventoryWidget.activeInHierarchy;
        }
    }
    
    private void UpdateMovementInput()
    {
        if (GameInstance.Inputs.MoveUp.IsPressed()) movement.y = 1;
        else if (GameInstance.Inputs.MoveDown.IsPressed()) movement.y = -1;
        else movement.y = 0;
        
        if (GameInstance.Inputs.MoveLeft.IsPressed()) movement.x = -1;
        else if (GameInstance.Inputs.MoveRight.IsPressed()) movement.x = 1;
        else movement.x = 0;

        if (GameInstance.Inputs.Action.IsPressed()) currentMoveSpeed = Stats.runSpeed;
        else currentMoveSpeed = Stats.walkSpeed;
        
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

public interface IsPlayerCharacter
{
}