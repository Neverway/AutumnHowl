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

public class GI_WidgetManager : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private const string CANVAS_GAMEOBJECT_TAG = "UserInterface";
    private GameObject _canvas;
    private GameObject Canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = GameObject.FindWithTag(CANVAS_GAMEOBJECT_TAG);
            return _canvas;
        }
    }
    public List<GameObject> widgets;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    
    // Returns the widget prefab corresponding to the inputted widget name
    private GameObject GetWidgetPrefab(string _widgetName)
    {
        foreach (var widget in widgets)
            if (widget.name == _widgetName) 
                return widget;

        throw new Exception($"No widget named \"{_widgetName}\" exists. " +
                            $"(check if widget is added to {nameof(GI_WidgetManager)} on {name})");
    }

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    
    /// <summary>
    /// Adds the specified widget to the user interface if it's in the widget list
    /// </summary>
    /// <param name="_widgetName"></param>
    /// <returns>Returns true if we added the widget and false if it failed to be added (or if it was already present and _allowDuplicates is set to false)</returns>
    public bool AddWidget(string _widgetName, bool _allowDuplicates = false) => AddWidget(GetWidgetPrefab(_widgetName), _allowDuplicates);
    public bool AddWidget(GameObject _widgetObject, bool _allowDuplicates = false)
    {
        //Do not add widget if no canvas exists
        if (Canvas == null) return false;

        //Do not allow adding a new widget if it already exists (unless duplicates are allowed)
        if (_allowDuplicates is false)
            if (GetExistingWidget(_widgetObject.name) != null)
                return false;

        var newWidget = Instantiate(_widgetObject, Canvas.transform, false);
        newWidget.transform.localScale = Vector3.one;
        newWidget.name = _widgetObject.name;
        return true;
    }
    
    /// <summary>
    /// Adds the specified widget if it's no present on the interface, or removes it if it already is
    /// </summary>
    /// <param name="_widgetName"></param>
    /// <returns>Returns true if we added the widget and false if we destroyed it</returns>
    public bool ToggleWidget(string _widgetName)
    {
        // If the widget already exists, destroy it
        GameObject existingWidget = GetExistingWidget(_widgetName);
        if (existingWidget != null)
        {
            Destroy(existingWidget);
            return false;
        }
        // If it does not exist, create it
        AddWidget(_widgetName);
        return true;
    }

    /// <summary>
    /// Returns the specified widget object if the widget is present on the interface
    /// </summary>
    /// <param name="_widgetName"></param>
    /// <returns>Returns the widget if it's present on the user interface</returns>
    public GameObject GetExistingWidget(string _widgetName)
    {
        if (Canvas == null) return null;

        foreach (Transform child in Canvas.transform)
            if (child.name == _widgetName) return child.gameObject;

        return null;
    }


    #endregion
}
