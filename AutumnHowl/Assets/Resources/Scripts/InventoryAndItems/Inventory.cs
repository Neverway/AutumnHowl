//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Inventory
{
    public const int ITEMS_LIST_ID = 0;
    public const int SPELLS_LIST_ID = 1;
    public const int EQUIPMENT_LIST_ID = 2;


    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public List<Item> items = new List<Item>();
    public List<Item_Magic> spells = new List<Item_Magic>();
    public List<Item_Wearable> equippedWearables = new List<Item_Wearable>();

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/

    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private int maxItems = 8;
    private int maxSpells = 4;
    private int maxEquipment = 4;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public bool TryAddItem(Item _item)
    {
        if (_item is Item_Magic itemMagic)
        {
            if (spells.Count < maxSpells)
            {
                if (spells.Contains(itemMagic) && itemMagic.allowMultiple is false) return false;
                spells.Add(itemMagic);
                return true;
            }
        }
        else
        {
            if (items.Count < maxItems)
            {
                if (items.Contains(_item) && _item.allowMultiple is false) return false;
                items.Add(_item);
                return true;
            }
        }

        return false;
    }
    
    public Item GetItem(int _atIndex, int _inList=0)
    {
        switch (_inList)
        {
            case 0:
                if (_atIndex < items.Count) return items[_atIndex];
                else return null;
            case 1:
                if (_atIndex < spells.Count) return spells[_atIndex];
                else return null;
            case 2:
                if (_atIndex < equippedWearables.Count) return equippedWearables[_atIndex];
                else return null;
        }

        return null;
    }
    
    public bool TryRemoveItem(int _atIndex, int _inList=0)
    {
        switch (_inList)
        {
            case 0:
                if (_atIndex < items.Count)
                {
                    if (items[_atIndex].canNotDiscard) { return false; }
                    else { items.Remove(items[_atIndex]); return true; }
                }
                break;
            case 1:
                if (_atIndex < spells.Count)
                {
                    if (spells[_atIndex].canNotDiscard) { return false; }
                    else { spells.Remove(spells[_atIndex]); return true; }
                }
                break;
            case 2:
                if (_atIndex < equippedWearables.Count)
                {
                    if (equippedWearables[_atIndex].canNotDiscard) { return false; }
                    else { equippedWearables.Remove(equippedWearables[_atIndex]); return true; }
                }
                break;
        }

        return false;
    }
    
    public bool TryUseItem(int _atIndex, CharacterIdentifier user, int _inList = 0)
    {
        switch (_inList)
        {
            case ITEMS_LIST_ID:
                if (items.IsIndexInRange(_atIndex))
                    return items[_atIndex].TryUse(user, _atIndex, ITEMS_LIST_ID);
                break;
            case SPELLS_LIST_ID:
                if (spells.IsIndexInRange(_atIndex))
                    return spells[_atIndex].TryUse(user, _atIndex, SPELLS_LIST_ID);
                break;
            case EQUIPMENT_LIST_ID:
                if (equippedWearables.IsIndexInRange(_atIndex))
                    return equippedWearables[_atIndex].TryUse(user, _atIndex, EQUIPMENT_LIST_ID);
                break;
        }

        return false;
    }

    public bool TryUnequipItem(int indexInEquipment)
    {
        //Make sure index is in range
        if (equippedWearables.IsIndexOutOfRange(indexInEquipment)) return false;

        //Cant unequip if items are full
        if (items.Count >= maxItems) return false;

        //Get item and remove it from list
        Item_Wearable item = equippedWearables[indexInEquipment];
        equippedWearables.RemoveAt(indexInEquipment);

        //Remove modifiers of equipment
        var player = GameInstance.Gamestate.player;
        var modID = EquipSlotModID(indexInEquipment);
        item.effectWhenEquipped.UnregisterFrom(EquipSlotModID(indexInEquipment));

        //Add to items
        items.Add(item);

        return true;
    }

    public bool TryEquipItem(int indexInItems)
    {
        //Make sure index is in range
        if (items.IsIndexOutOfRange(indexInItems)) return false;

        //Get item and make sure it is equipment
        Item item = items[indexInItems];
        if (item is not Item_Wearable wearable) return false;

        //Try to equip the item
        if (!TryEquipItem(wearable)) return false;

        //If equip succeeds, remove from items list
        items.RemoveAt(indexInItems);
        return true;
    }
    public bool TryEquipItem(Item_Wearable equipment)
    {
        //Cant equip if equipment is full
        if (equippedWearables.Count >= maxEquipment) return false;

        //Add to equipped items
        equippedWearables.Add(equipment);

        //Apply equip modifiers from object
        var player = GameInstance.Gamestate.player;
        var modID = EquipSlotModID(equippedWearables.Count - 1);
        equipment.effectWhenEquipped.RegisterTo_Flexible(modID, new TargetSelf().GetTargetsFrom(player));

        return true;
    }

    /// <summary>ID of the slot to register and unregistermodifiers to</summary>
    public string EquipSlotModID(int equipSlot) => $"player_equipslot_{equipSlot}";

    #endregion


    public SaveData OnSaveData() => new SaveData()
    {
        itemIDs = items.Select(item => item.UniqueID).ToArray(),
        spellIds = spells.Select(item => item.UniqueID).ToArray(),
        wearableIDs = equippedWearables.Select(item => item.UniqueID).ToArray(),
    };
    public void OnLoadData(SaveData toLoad)
    {
        //Load Items
        items = new List<Item>();
        if (toLoad.itemIDs == null) Debug.LogWarning($"Items list was null");
        else
        {
            foreach (var itemID in toLoad.itemIDs)
            {
                if (IDToObj<Item>.TryGet(itemID, out var item))
                    items.Add(item);
                else
                    Debug.LogWarning($"Unable to find item to load from UniqueID : {itemID}");
            }
        }

        //Load Spells
        {
            spells = new List<Item_Magic>();
            foreach (var itemID in toLoad.spellIds)
            {
                if (IDToObj<Item>.TryGet(itemID, out var item))
                    spells.Add(item as Item_Magic);
                else
                    Debug.LogWarning($"Unable to find item to load from UniqueID : {itemID}");
            }
        }

        //Load Wearables
        {
            //Unregister all previously applied modifiers
            for (int i = 0; i < equippedWearables.Count; i++)
            {
                equippedWearables[i].effectWhenEquipped.UnregisterFrom(EquipSlotModID(i));
            }

            //Load items into a new list of equipment
            equippedWearables = new List<Item_Wearable>();
            foreach (var itemID in toLoad.wearableIDs)
            {
                if (IDToObj<Item>.TryGet(itemID, out var item))
                {
                    if (item is not Item_Wearable wearable)
                    {
                        Debug.LogWarning($"Item saved to equipment was not Item_Wearable : {itemID}");
                        continue;
                    }
                    if(!TryEquipItem(wearable))
                    {
                        Debug.LogWarning($"Was unable to equip loaded item for some reason : {itemID}");
                        continue;
                    }
                }
                else
                    Debug.LogWarning($"Unable to find item to load from UniqueID : {itemID}");
            }
        }

    }
    [Serializable]
    public struct SaveData
    {
        public string[] itemIDs;
        public string[] spellIds;
        public string[] wearableIDs;
    }
}
