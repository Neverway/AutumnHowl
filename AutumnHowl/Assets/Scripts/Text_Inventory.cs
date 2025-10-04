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
using System.Linq;
using TMPro;
using UnityEngine;

public class Text_Inventory : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private InventoryType inventoryType;
    private enum InventoryType
    {
        listItems,
        listSpells
    }
    public string textDecoratorStart = "*";
    public string textDecoratorEnd = "";


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public List<WidgetSelectable_TMPText> textElements = new List<WidgetSelectable_TMPText>();
    private GI_AuHoGameState gameState;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
    }

    private void OnEnable()
    {
        UpdateItemList();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/

    public void UpdateItemList()
    {
        textElements = GetComponentsInChildren<WidgetSelectable_TMPText>().ToList();
        if (!gameState) { gameState = GameInstance.Get<GI_AuHoGameState>(); }
        
        switch (inventoryType)
        {
            case InventoryType.listItems:
                for (int i = 0; i < textElements.Count; i++)
                {
                    
                    if (i < gameState.currentGameState.inventory.items.Count)
                    {
                        var item = gameState.currentGameState.inventory.items[i];
                        textElements[i].SetText(string.Format(textDecoratorStart + item.displayName + textDecoratorEnd, item.sellCost));
                    }
                    else
                    {
                        textElements[i].SetText("*---");
                    }
                }
                break;
            case InventoryType.listSpells:
                for (int i = 0; i < gameState.currentGameState.inventory.spells.Count; i++)
                {
                    if (i < gameState.currentGameState.inventory.spells.Count)
                    {
                        var item = gameState.currentGameState.inventory.spells[i];
                        textElements[i].SetText(textDecoratorStart + item.displayName + textDecoratorEnd);
                    }
                    else
                    {
                        textElements[i].SetText("*---");
                    }
                }
                break;
        }
    }


    #endregion
}
