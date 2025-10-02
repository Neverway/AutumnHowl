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
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WidgetNavigator_TMP : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Tooltip("Which directional inputs to use to navigate the menu")]
    [SerializeField] private NavigationMode navigationMode;
    private enum NavigationMode
    {
        Vertical,
        Horizontal
    }
    [Tooltip("If enabled, reaching either end of the button list will wrap back around when navigating")]
    [SerializeField] private bool enableWrapping;

    public bool hideIndicatorOnInactive;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [Tooltip("The current position in the menu")]
    public int currentIndex = 0;
    [Tooltip("")] 
    public bool activelyNavigating;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private InputActions.TopDownActions inputActions;
    [SerializeField] private List<TMP_Text> buttons;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        // Setup inputs
        inputActions = new InputActions().TopDown;
        inputActions.Enable();
    }

    private void Update()
    {
        SetButtonStates();
        if (activelyNavigating)
        {
            GetIndexInputs();
        }
        else if (hideIndicatorOnInactive)
        {
            buttons[currentIndex].text = "  ";
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void SetButtonStates()
    {
        foreach (var button in buttons)
        {
            button.text = "  ";
        }

        if (currentIndex < buttons.Count)
        {
            buttons[currentIndex].text = "> ";
        }
    }

    private void GetIndexInputs()
    {
        switch (navigationMode)
        {
            case NavigationMode.Vertical:
                CheckMove(inputActions.MoveUp, -1);
                CheckMove(inputActions.MoveDown, 1);
                break;
            case NavigationMode.Horizontal:
                CheckMove(inputActions.MoveLeft, -1);
                CheckMove(inputActions.MoveRight, 1);
                break;
        }
    }

    private void CheckMove(InputAction inputAction, int incrementIndex)
    {
        if (inputAction.WasPressedThisFrame())
        {
            if (enableWrapping)
            {
                currentIndex += incrementIndex;
                if (currentIndex < 0) currentIndex = buttons.Count-1;
                if (currentIndex >= buttons.Count) currentIndex = 0;
            }
            else
            {
                currentIndex += incrementIndex;
                currentIndex = Mathf.Clamp(currentIndex, 0, buttons.Count-1);
            }
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
