//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Item/New Wearable", fileName = "item_wearable_")]
public class Item_Wearable : Item
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Box, Polymorphic, SerializeReference] public SerializedModifier effectWhenEquipped;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    protected override bool OnUse(CharacterIdentifier user, int _atIndex, int _inList = 0)
    {
        return false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/

    public void Equip(CharacterIdentifier user)
    {
        effectWhenEquipped.RegisterTo_Flexible(user, new TargetSelf().GetTargetsFrom(user));
    }
    public void UnEquip(CharacterIdentifier user)
    {
        effectWhenEquipped.UnregisterFrom(user);
    }
    #endregion
}