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

[Serializable]
public class Inventory
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public List<Item> items = new List<Item>();
    public List<Item_Magic> spells = new List<Item_Magic>();
    public List<Item_Wearable> equippedWearables = new List<Item_Wearable>();


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private int maxItems = 8;
    private int maxSpells = 4;


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
                if (_atIndex < items.Count)
                {
                    return items[_atIndex];
                }
                else return null;
            case 1:
                if (_atIndex < spells.Count) return spells[_atIndex];
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
        }

        return false;
    }
    
    public bool TryUseItem(int _atIndex, Character user, Character target, int _inList=0)
    {
        switch (_inList)
        {
            case 0:
                if (_atIndex < items.Count)
                {
                    if (items[_atIndex].Use(user, target, _atIndex, _inList))
                    {
                        return true;
                    }

                    return false;
                }
                break;
            case 1:
                if (_atIndex < spells.Count)
                {
                    if (spells[_atIndex].Use(user, target, _atIndex, _inList))
                    {
                        return true;
                    }

                    return false;
                }
                break;
        }

        return false;
    }


    #endregion
}
