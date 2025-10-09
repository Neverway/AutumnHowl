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
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class oWB_Inventory : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private WidgetNavigator_Animator sidebar;
    public List<WidgetNavigator_TMP> submenus;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/

    private void Update()
    {
        if (sidebar.activelyNavigating)
        {
            SetSubmenuStates();
        }
        if (GameInstance.Inputs.Interact.WasPressedThisFrame())
        {
            sidebar.activelyNavigating = false;
            submenus[sidebar.currentIndex].activelyNavigating = true;
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/ 
    private void SetSubmenuStates()
    {
        foreach (var menu in submenus)
        {
            menu.gameObject.SetActive(false);
        }

        if (sidebar.currentIndex >= 0 && sidebar.currentIndex < submenus.Count)
        {
            submenus[sidebar.currentIndex].gameObject.SetActive(true);
        }
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
