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
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Uses button inputs to navigate through selectable elements on a widget
/// </summary>
public class WidgetNavigator : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Tooltip("If enabled, this menu is currently able to be navigated using button inputs")]
    public bool activelyNavigating;
    [Tooltip("Which directional inputs to use to navigate the menu")]
    [SerializeField] private NavigationMode navigationMode;
    private enum NavigationMode { Vertical, Horizontal }
    [Tooltip("If enabled, reaching either end of the button list will wrap back around when navigating")]
    [SerializeField] private bool enableWrapping;
    [Tooltip("If enabled, all elements will appear unselected when this menu is not set as activelyNavigating")]
    [SerializeField] private bool hideIndicatorOnInactive;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [Tooltip("The current position in the menu")]
    public int currentIndex = 0;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool initialized;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private InputActions.TopDownActions inputActions;
    public List<WidgetSelectable> selectableElements;
    public UnityEvent OnBack;


    #endregion


    #region=======================================( Functions )======================================================= //
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        // Setup inputs
        inputActions = new InputActions().TopDown;
        inputActions.Enable();
    }

    private void Update()
    {
        if (!initialized)
        {
            initialized = true;
            // Initialize the element states
            SetElementStates();
        }
        
        if (activelyNavigating)
        {
            GetIndexingInputs();
            selectableElements[currentIndex].SetSelected(true); // This line is required here to fix a bug caused by the hideIndicatorOnInactive statement below
        }
        else if (hideIndicatorOnInactive)
        {
            selectableElements[currentIndex].SetSelected(false);
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Update the appearances of the elements
    /// </summary>
    private void SetElementStates()
    {
        foreach (var selectable in selectableElements)
        {
            selectable.SetSelected(false);
        }
        selectableElements[currentIndex].SetSelected(true);
    }

    /// <summary>
    /// Get the button inputs for navigating through the index
    /// </summary>
    private void GetIndexingInputs()
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

        if (inputActions.Interact.WasPressedThisFrame())
        {
            selectableElements[currentIndex].Interact();
        }

        if (inputActions.Action.WasPressedThisFrame())
        {
            OnBack.Invoke();
        }
    }    
    
    private void CheckMove(InputAction inputAction, int incrementIndex)
    {
        if (inputAction.WasPressedThisFrame())
        {
            if (enableWrapping)
            {
                currentIndex += incrementIndex;
                if (currentIndex < 0) currentIndex = selectableElements.Count-1;
                if (currentIndex >= selectableElements.Count) currentIndex = 0;
            }
            else
            {
                currentIndex += incrementIndex;
                currentIndex = Mathf.Clamp(currentIndex, 0, selectableElements.Count-1);
            }
            
            // Update the selectable elements
            SetElementStates();
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void SetIsNavigating(bool _isNavigating)
    {
        activelyNavigating = _isNavigating;
    }


    #endregion
}
