//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Item/New Magic", fileName = "item_magic_")]
public class Item_Magic : Item
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public int powerCost;
    [Box, Polymorphic, SerializeReference] public EffectAction effectsWhenCast;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public override string GetDescription()
    {
        //Start description with stat colors and speed
        string fullDescription = "{col=stat,spd=stat}";

        //Add power cost to description if cost is not 0
        if (powerCost != 0)
            fullDescription += $"[Costs {powerCost} PWR] ";
        else
            fullDescription += "[No cast cost] ";

        //Try to add effects to description
        if (effectsWhenCast != null)
        {
            try { fullDescription += $"{effectsWhenCast.DescribeNoFormat()}"; }
            catch (Exception e) { 
                Debug.LogException(e);  
                fullDescription += "{col=err}[ERROR]{col=} "; 
            }
        }

        //End stat colors and speed
        fullDescription += "{col=,spd=}";

        //Add the item's basic description afterwards and return result
        fullDescription += $"{description}";
        return fullDescription;
    }
    protected override bool OnUse(CharacterIdentifier user, int _atIndex, int _inList=0)
    {
        //Don't use if the user cannot afford power cost
        if (powerCost != 0 && user.Stats.power < powerCost)
            return false;

        //Spend power and apply the effect
        user.Stats.power -= powerCost;
        effectsWhenCast.ApplyEffect(user);
        return true;
    }

    #endregion
}
