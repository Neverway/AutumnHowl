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
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Item/New Consumable", fileName = "item_consumable_")]
public class Item_Consumable : Item
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Polymorphic, SerializeReference] public EffectAction effectsOnConsume;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public override string GetDescription()
    {
        if (effectsOnConsume == null) return description;

        return effectsOnConsume.DescribeFormatted() + " " + description;
    }
    public override bool Use(Character user, int _atIndex, int _inList=0)
    {
        GameInstance.Get<GI_AuHoGameState>().currentGameState.inventory.TryRemoveItem(_atIndex, _inList);
        effectsOnConsume.ApplyEffect(user);
        return true;
    }

    #endregion
}
