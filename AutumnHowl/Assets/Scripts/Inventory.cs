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
                if (_atIndex < items.Count) return items[_atIndex];
                else return null;
            case 1:
                if (_atIndex < spells.Count) return spells[_atIndex];
                else return null;
        }

        return null;
    }
    
    public void RemoveItem(int _atIndex, int _inList=0)
    {
        switch (_inList)
        {
            case 0:
                items.Remove(items[_atIndex]);
                break;
            case 1:
                spells.Remove(spells[_atIndex]);
                break;
        }
    }


    #endregion
}
