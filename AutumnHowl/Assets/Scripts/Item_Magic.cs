//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/New Magic Item", fileName = "item_magic_")]
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

        //Add effects to description if there is defined effects
        if (effectsWhenCast != null)
            fullDescription += $"{effectsWhenCast.DescribeNoFormat()}";

        //End stat colors and speed
        fullDescription += "{col=,spd=}";

        //Add the item's basic description afterwards and return result
        fullDescription += $"{description}";
        return fullDescription;
    }
    protected override bool OnUse(Character user, int _atIndex, int _inList=0)
    {
        //Don't use if the user cannot afford power cost
        if (powerCost != 0 && user.currentStats.power < powerCost)
            return false;

        //Spend power and apply the effect
        user.currentStats.power -= powerCost;
        effectsWhenCast.ApplyEffect(user);
        return true;
    }

    #endregion
}
