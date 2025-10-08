//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/New Magic Item", fileName = "item_magic_")]
public class Item_Magic : Item
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Polymorphic, SerializeReference] public EffectAction effectsWhenCast;
    public int powerCost;

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
        string fullDescription = "";
        if (effectsWhenCast == null) return fullDescription;
        if (powerCost != 0)
            fullDescription += $"[Costs {powerCost} Power]";
        return effectsWhenCast.DescribeFormatted() + " " + description;
    }
    public override bool Use(Character user, int _atIndex, int _inList=0)
    {
        effectsWhenCast.ApplyEffect(user);
        return true;
    }

    #endregion
}
