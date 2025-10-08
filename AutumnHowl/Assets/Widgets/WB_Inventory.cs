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


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void SetupInspectMenu(WidgetNavigator _parentNavigator, int _index)
    {
        // denavigate the parent nav
        _parentNavigator.SetIsNavigating(false);
        
        // Enable and navigate the inspect menu
        inspectListNavigator.gameObject.SetActive(true);
        inspectListNavigator.SetIsNavigating(true);
        
        // Set the buttons in the inspect menu for the current item
        for (int i = 0; i < inspectListNavigator.selectableElements.Count; i++)
        {
            inspectListNavigator.selectableElements[i].OnInteracted.RemoveAllListeners();
        }
        inspectListNavigator.selectableElements[0].OnInteracted.AddListener(()=> { Inspect(_parentNavigator, _index); });
        inspectListNavigator.selectableElements[1].OnInteracted.AddListener(()=> { Use(_parentNavigator, _index); });
        inspectListNavigator.selectableElements[2].OnInteracted.AddListener(()=> { Discard(_parentNavigator, _index); });
        
        // Set the inspect menu to reactive parent nav on back
        inspectListNavigator.OnBack.RemoveAllListeners();
        inspectListNavigator.OnBack.AddListener(() => {
                _parentNavigator.SetIsNavigating(true);
                inspectListNavigator.SetIsNavigating(false);
                inspectListNavigator.gameObject.SetActive(false);
            });
    }

    public void Inspect(WidgetNavigator _navList, int _index)
    {
        int itemList = 0;
        if (_navList.gameObject.name == "Spells") itemList = 1;
        var item = gameState.currentGameState.inventory.GetItem(_index, itemList);
        
        if (item)
        {
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            inspectTextEvent.textEvent.frames[0].chatContent = item.description;
            inspectTextEvent.CallEvent();
        }
        else
        {
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            inspectTextEvent.textEvent.frames[0].chatContent = "*You inspected nothing.{spd=0.2} {spd=}It's quite captivating.";
            inspectTextEvent.CallEvent();
        }
    }

    public void Use(WidgetNavigator _navList, int _index)
    {
        int itemList = 0;
        if (_navList.gameObject.name == "Spells") itemList = 1;
        
        if (!gameState.currentGameState.inventory.GetItem(_index, itemList))
        {
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            inspectTextEvent.textEvent.frames[0].chatContent = "*You attempt to use nothing.{col=key,spd=0.2} {spd=}It did a trick!{col=,spd=1} {spd=}Never mind, it did nothing.";
            inspectTextEvent.CallEvent();
            return;
        }
        
        if (gameState.currentGameState.inventory.TryUseItem(_index, FindObjectOfType<Controller_Overworld_Player>(), null, itemList))
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

    public void Discard(WidgetNavigator _navList, int _index)
    {
        int itemList = 0;
        if (_navList.gameObject.name == "Spells") itemList = 1;
        
        if (gameState.currentGameState.inventory.TryRemoveItem(_index, itemList) is false)
        {
            inspectListNavigator.SetIsNavigating(false);
            SpellListNavigator.SetIsNavigating(false);
            if (gameState.currentGameState.inventory.GetItem(_index, itemList)) inspectTextEvent.textEvent.frames[0].chatContent = "*You probably shouldn't discard this item.";
            else inspectTextEvent.textEvent.frames[0].chatContent = "*You attempted to discard nothing,{spd=0.5} {spd=}but there is still nothing here.{spd=0.5} {spd=}Did you succeed?";
            inspectTextEvent.CallEvent();
        }
        else
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


    #endregion
}
