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
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WB_Shop : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Func_TextEvent buyDescriptionTextEvent;
    public List<Item> buyableItems;
    public List<WidgetSelectable_TMPText> buySlots;
    public List<WidgetSelectable_TMPText> sellSlots;
    public Text_Inventory inventoryList;
    public UnityEvent OnStart;
    private GI_AuHoGameState gameState;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        UpdateBuyables();
        UpdateSellables();
        OnStart.Invoke();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateBuyables()
    {
        for (int i = 0; i < buySlots.Count; i++)
        {
            if (i < buyableItems.Count && buyableItems[i] != null)
            {
                buySlots[i].SetText($"$" + $"{buyableItems[i].buyCost} - " + $"{buyableItems[i].displayName}");
                buySlots[i].OnInteracted.RemoveAllListeners();
                buySlots[i].OnSelected.RemoveAllListeners();
                var itemIndex = i; // Cache this value so calling the listener doesn't break
                buySlots[i].OnInteracted.AddListener(()=> { BuyItem(itemIndex); });
                buySlots[i].OnSelected.AddListener(()=> { SetBuyDescription(itemIndex); });
                buySlots[i].OnSelected.AddListener(()=> { buyDescriptionTextEvent.CallEvent(); });
            }
            else
            {
                buySlots[i].SetText($"---");
            }
        }
    }    
    
    private void UpdateSellables()
    {
        for (int i = 0; i < sellSlots.Count; i++)
        {
            sellSlots[i].OnInteracted.RemoveAllListeners();
            var itemIndex = i; // Cache this value so calling the listener doesn't break
            sellSlots[i].OnInteracted.AddListener(()=> { SellItem(itemIndex); });
        }
    }
    
    private IEnumerator ExitCoroutine()
    {
        GameInstance.Get<GI_TextboxManager>().Clear();
        GameInstance.Get<GI_TransitionManager>().Fadeout();
        yield return new WaitForSeconds(0.5f);
        GameInstance.Get<GI_WorldLoader>().Load("Town", "Shop");
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void Exit()
    {
        StartCoroutine(ExitCoroutine());
    }

    public void BuyItem(int _index)
    {
        if (gameState == null) gameState = GameInstance.Get<GI_AuHoGameState>();
        
        // See if player has enough moolaa
        if (gameState.currentGameState.money >= buyableItems[_index].buyCost)
        {
            // See if the player has too many stinkin items!
            if (gameState.currentGameState.inventory.TryAddItem(buyableItems[_index]))
            {
                // All good, yoink their dubloons
                gameState.currentGameState.money -= buyableItems[_index].buyCost;
            }
        }
    }

    public void SetBuyDescription(int _index)
    {
        print($"Set description to index {_index}");
        buyDescriptionTextEvent.textEvent.frames[0].chatContent = buyableItems[_index].GetDescription();
    }

    public void SellItem(int _index)
    {
        if (gameState == null) gameState = GameInstance.Get<GI_AuHoGameState>();
        var inventory = gameState.currentGameState.inventory;
        var item = inventory.GetItem(_index);
        
        // See if player has item at index
        if (!item) return;
        
        // See if the player can discard
        if (item.canNotDiscard) return;
        
        // All good, un-yoink their dubloons
        gameState.currentGameState.money += item.sellCost;
        inventory.TryRemoveItem(_index);
        
        // Update the list
        inventoryList.UpdateItemList();
    }


    #endregion
}
