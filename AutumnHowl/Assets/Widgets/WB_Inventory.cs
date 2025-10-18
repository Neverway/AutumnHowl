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
using Unity.VisualScripting;
using UnityEngine;

public class WB_Inventory : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private WidgetNavigator ItemListNavigator;
    [SerializeField] private WidgetNavigator SpellListNavigator;
    [SerializeField] private WidgetNavigator inspectListNavigator;
    [SerializeField] private Func_TextEvent inspectTextEvent;
    private GI_AuHoGameState gameState;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
        
        for (int i = 0; i < ItemListNavigator.selectableElements.Count; i++)
        {
            var selectable = ItemListNavigator.selectableElements[i];
            var cachedIndex = i;
            selectable.OnInteracted.AddListener(() => { SetupInspectMenu(ItemListNavigator, cachedIndex);});
        }
        
        for (int i = 0; i < SpellListNavigator.selectableElements.Count; i++)
        {
            var selectable = SpellListNavigator.selectableElements[i];
            var cachedIndex = i;
            selectable.OnInteracted.AddListener(() => { SetupInspectMenu(SpellListNavigator, cachedIndex);});
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Allows each entry to create the inspect menu and updates the inspect menu to perform action on the currently selected item
    /// This function is bound to each item entry in the item and spell lists
    /// </summary>
    /// <param name="_parentNavigator"></param>
    /// <param name="_index">The index of the entry in the item list that this function is bound to</param>
    private void SetupInspectMenu(WidgetNavigator _parentNavigator, int _index)
    {
        // un-navigate the parent navigator
        _parentNavigator.SetIsNavigating(false);
        
        // Enable and navigate the inspect menu
        inspectListNavigator.gameObject.SetActive(true);
        inspectListNavigator.SetIsNavigating(true);
        
        // Set the buttons in the inspect menu for the current item
        for (int i = 0; i < inspectListNavigator.selectableElements.Count; i++)
        {
            inspectListNavigator.selectableElements[i].OnInteracted.RemoveAllListeners();
        }
        inspectListNavigator.selectableElements[0].OnInteracted.AddListener(()=> { Inspect(_parentNavigator.name, _index); });
        inspectListNavigator.selectableElements[1].OnInteracted.AddListener(()=> { Use(_parentNavigator.name, _index); });
        inspectListNavigator.selectableElements[2].OnInteracted.AddListener(()=> { Discard(_parentNavigator.name, _index); });
        
        // Set the inspect menu to reactive parent nav on back
        inspectListNavigator.OnBack.RemoveAllListeners();
        inspectListNavigator.OnBack.AddListener(() => {
            _parentNavigator.SetIsNavigating(true);
            inspectListNavigator.SetIsNavigating(false);
            inspectListNavigator.gameObject.SetActive(false);
        });
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Prints the selected item's description (or falls back to some default text if it's null item)
    /// </summary>
    /// <param name="_itemList">The name of the item list we want to check (Items or Spells)</param>
    /// <param name="_index">The index of the item we want to get</param>
    public void Inspect(string _itemList, int _index)
    {
        // Store if this is the items or spells list (items = 0, spells = 1)
        int itemList = -1;
        if (_itemList == "Items") itemList = 0;
        else if (_itemList == "Spells") itemList = 1;
        else { Debug.LogError("In WB_Inventory.cs Inspect() you tried passing in a navigator widget, but it wasn't named 'Items' or 'Spells'."); return; }
        
        var itemAtIndex = gameState.currentGameState.inventory.GetItem(_index, itemList);
        
        // If there is an item at the selected index
        if (itemAtIndex)
        {
            // Stop navigation of the items or spells list
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            // Assign and show the textbox with the selected item's description
            inspectTextEvent.textEvent.frames[0].chatContent = itemAtIndex.GetDescription();
            inspectTextEvent.CallEvent();
        }
        // If there is NOT an item at the selected index
        else
        {
            // Stop navigation of the items or spells list
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            // Assign and show the textbox with the default "can't inspect" text
            inspectTextEvent.textEvent.frames[0].chatContent = "*You inspected nothing.{spd=0.2} {spd=}It's quite captivating.";
            inspectTextEvent.CallEvent();
        }
    }

    /// <summary>
    /// Trys to use the selected item (or falls back to some default text if it's null item)
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
            Character player = FindObjectOfType<Controller_Overworld_Player>();
        
            if (gameState.currentGameState.inventory.TryUseItem(_index, player.Identifier, itemList))
            {
                inspectListNavigator.SetIsNavigating(false);
                inspectListNavigator.gameObject.SetActive(false);
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
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            // Assign and show the textbox with the default "can't use" text
            inspectTextEvent.textEvent.frames[0].chatContent = "*You attempt to use nothing.{col=key,spd=0.2} {spd=}It did a trick!{col=,spd=1} {spd=}Never mind, it did nothing.";
            inspectTextEvent.CallEvent();
        }
    }

    /// <summary>
    /// Trys to discard the selected item (or falls back to some default text if it's null item)
    /// </summary>
    /// <param name="_itemList">The name of the item list we want to check (Items or Spells)</param>
    /// <param name="_index">The index of the item we want to get</param>
    public void Discard(string _itemList, int _index)
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
            // Try to discard it
            if (gameState.currentGameState.inventory.TryRemoveItem(_index, itemList))
            {
                inspectListNavigator.SetIsNavigating(false);
                inspectListNavigator.gameObject.SetActive(false);
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
            // Item can't be discarded because it's a key item
            else if (gameState.currentGameState.inventory.GetItem(_index, itemList))
            { 
                inspectTextEvent.textEvent.frames[0].chatContent = "*You probably shouldn't discard this item.";
                inspectTextEvent.CallEvent();
            }
            // Item can't be discarded because... IT DISAPPEARED BETWEEN THE FIRST CHECK AND NOW??? HOW???!!
            else
            {
                inspectTextEvent.textEvent.frames[0].chatContent = "*You probably shouldn't discard...{spd=0.5} {spd=}wait...{spd=0.5} {spd=}there is nothing here?!";
                inspectTextEvent.CallEvent();
            }
        }
        // If there is NOT an item at the selected index
        else
        {
            // Stop navigation of the items or spells list
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            // Assign and show the textbox with the default "can't discard" text
            inspectTextEvent.textEvent.frames[0].chatContent = "*You attempted to discard nothing,{spd=0.5} {spd=}but there is still nothing here.{spd=0.5} {spd=}Did you succeed?";
            inspectTextEvent.CallEvent();
        }
    }


    #endregion
}
