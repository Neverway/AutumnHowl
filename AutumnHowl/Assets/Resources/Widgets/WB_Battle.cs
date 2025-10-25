//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M., Errynei, Connorses
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WB_Battle : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private GI_AuHoGameState gameState;
    [Header("Inventory Stuff")] 
    public Text_Inventory items;
    public Text_Inventory spells;
    [SerializeField] private WidgetNavigator ItemListNavigator;
    [SerializeField] private WidgetNavigator SpellListNavigator;
    [SerializeField] private Func_TextEvent inspectTextEvent;
    [Header("Action Stuff")] 
    public Animator actionBarAnimator;
    public WidgetNavigator actionBarNavigator;
    public BattleAttackCompass attackCompass;
    [Header("Step Stuff")] 
    public TMP_Text stepCountText;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
        // Hide the action bar when the battle begins
        // since the first thing that will be on screen is the intro text
        // & the action bar is normally visible by default
        SetActionBarVisible(false);
        
        // Set Inventory Stuff
        items.UpdateItemList();
        spells.UpdateItemList();
        
        // Assign each of the buttons in the items menu to fire the USE function from the same index in the player's inventory
        for (int i = 0; i < ItemListNavigator.selectableElements.Count; i++)
        {
            var selectable = ItemListNavigator.selectableElements[i];
            var cachedIndex = i;
            selectable.OnInteracted.AddListener(() => { Use("Items", cachedIndex);});
        }
        
        // Assign each of the buttons in the spells menu to fire the USE function from the same index in the player's inventory
        for (int i = 0; i < SpellListNavigator.selectableElements.Count; i++)
        {
            var selectable = SpellListNavigator.selectableElements[i];
            var cachedIndex = i;
            selectable.OnInteracted.AddListener(() => { Use("Spells", cachedIndex);});
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Set the visibility of the action menu
    /// </summary>
    public void SetActionBarVisible(bool _isVisible)
    {
        switch (_isVisible)
        {
            case true:
                actionBarNavigator.SetIsNavigating(true);
                actionBarAnimator.Play("Open");
                break;
            case false:
                actionBarNavigator.SetIsNavigating(false);
                actionBarAnimator.Play("Close");
                break;
        }
    }
    
    /// <summary>
    /// Set the visibility of the attack compass
    /// </summary>
    public void SetAttackBarVisible(bool _isVisible)
    {
        switch (_isVisible)
        {
            case true:
                attackCompass.gameObject.SetActive(true);
                break;
            case false:
                attackCompass.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Trys to use the selected inventory item (or falls back to some default text if it's null item)
    /// </summary>
    /// <param name="_itemList">The name of the item list we want to check (Items or Spells)</param>
    /// <param name="_index">The index of the item we want to get</param>
    public void Use(string _itemList, int _index)
    {
        // Store if this is the items or spells list (items = 0, spells = 1)
        int itemList = -1;
        if (_itemList == "Items") itemList = 0;
        else if (_itemList == "Spells") itemList = 1;
        else { Debug.LogError("In WB_Inventory.cs Use() you tried passing in a navigator widget, but it wasn't named 'Items' or 'Spells'."); return; }
        
        var itemAtIndex = gameState.currentGameState.inventory.GetItem(_index, itemList);
        
        // If there is an item at the selected index
        if (itemAtIndex)
        {
            Character player = FindObjectOfType<Char_Battle_Player>();
        
            if (gameState.currentGameState.inventory.TryUseItem(_index, player.Identifier, itemList))
            {
                switch (itemList)
                {
                    case 0:
                        ItemListNavigator.GetComponent<Text_Inventory>().UpdateItemList();
                        ItemListNavigator.SetIsNavigating(true);
                        break;
                    case 1:
                        SpellListNavigator.GetComponent<Text_Inventory>().UpdateItemList();
                        SpellListNavigator.SetIsNavigating(true);
                        break;
                }
            }
        }
        // If there is NOT an item at the selected index
        else
        {
            // Stop navigation of the items or spells list
            //ItemListNavigator.SetIsNavigating(false);
            //SpellListNavigator.SetIsNavigating(false);
            // Assign and show the textbox with the default "can't use" text
            inspectTextEvent.textEvent.frames[0].chatContent = "*There is no item here to use!";
            inspectTextEvent.CallEvent();
        }
    }


    #endregion
}
