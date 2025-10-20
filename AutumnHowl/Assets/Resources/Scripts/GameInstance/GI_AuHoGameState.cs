//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using DG.Tweening.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GI_AuHoGameState : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Box] public AuHoGameState currentGameState;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    
    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}

[Serializable]
public class AuHoGameState
{
    public CharacterIdentifier player;
    public string map = "Town";
    public Vector2 overworldPosition;
    public int money = 0;
    [Box] public Inventory inventory = new Inventory();
    public int kills = 0;
    public int deaths = 0;
    public float playtime = 0;
    public BattleData currentBattle;

    [InvokeBeforeSave]
    public static void OnGameSave()
    {
        var gameState = GameInstance.Get<GI_AuHoGameState>().currentGameState;

        InventorySaveData inventoryData = new InventorySaveData()
        {
            itemIDs = gameState.inventory.items.Select(item => item.UniqueID).ToArray(),
            spellIds = gameState.inventory.spells.Select(item => item.UniqueID).ToArray(),
            wearableIDs = gameState.inventory.equippedWearables.Select(item => item.UniqueID).ToArray(),
        };
        GI_SaveSystem.SaveValue(inventoryData, "PlayerInventory");
    }
    [InvokeAfterLoad]
    public static void OnGameLoad()
    {
        var gameState = GameInstance.Get<GI_AuHoGameState>().currentGameState;

        InventorySaveData data = GI_SaveSystem.LoadValue<InventorySaveData>(null, "PlayerInventory");
        if (data != null)
        {
            //Load Items
            gameState.inventory.items = new List<Item>();
            foreach (var itemID in data.itemIDs)
            {
                if (IDToObj<Item>.TryGet(itemID, out var item))
                    gameState.inventory.items.Add(item);
                else
                    Debug.LogWarning($"Unable to find item to load from UniqueID : {itemID}");
            }

            //Load Spells
            gameState.inventory.spells = new List<Item_Magic>();
            foreach (var itemID in data.spellIds)
            {
                if (IDToObj<Item>.TryGet(itemID, out var item))
                    gameState.inventory.spells.Add(item as Item_Magic);
                else
                    Debug.LogWarning($"Unable to find item to load from UniqueID : {itemID}");
            }

            //Load Wearables
            gameState.inventory.equippedWearables = new List<Item_Wearable>();
            foreach (var itemID in data.wearableIDs)
            {
                if (IDToObj<Item>.TryGet(itemID, out var item))
                    gameState.inventory.equippedWearables.Add(item as Item_Wearable);
                else
                    Debug.LogWarning($"Unable to find item to load from UniqueID : {itemID}");
            }
        }
    }

    public class InventorySaveData
    {
        public string[] itemIDs;
        public string[] spellIds;
        public string[] wearableIDs;
    }
}