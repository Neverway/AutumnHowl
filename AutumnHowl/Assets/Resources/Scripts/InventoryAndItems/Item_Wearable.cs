//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

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
    protected override bool OnUse(CharacterIdentifier user, int _atIndex, int _inList = Inventory.ITEMS_LIST_ID)
    {
        var inventory = GameInstance.Get<GI_AuHoGameState>().currentGameState.inventory;

        //Equip item if this is from the items list
        if (_inList == Inventory.ITEMS_LIST_ID)
            return inventory.TryEquipItem(_atIndex);

        //Unequip item if this si from the equipment list
        if (_inList == Inventory.EQUIPMENT_LIST_ID)
            return inventory.TryUnequipItem(_atIndex);

        return false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/

    #endregion
}