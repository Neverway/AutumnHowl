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
    [Box, Polymorphic, SerializeReference] public ICharacterStatModInstancer effectWhenEquipped;

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

    public EquippedInstance Equip(CharacterIdentifier user)
    {
        EquippedInstance newInstance = new EquippedInstance(this, user);
        return newInstance;
    }
    public void UnEquip(EquippedInstance instance)
    {
        instance.UnEquip();
    }

    #endregion

    /// <summary>This is required to keep track of unique instances of WHO is equipping the item and what modifiers its applying
    /// <br/> - Todo: Try to remove this and do a better inventory setup that lets you identify unique instances of items</summary>
    public class EquippedInstance
    {
        public Item_Wearable equippedItem;
        public Modifier appliedModifier;
        public CharacterIdentifier user;

        public EquippedInstance(Item_Wearable toEquip, CharacterIdentifier character)
        {
            user = character;
            equippedItem = toEquip;
            appliedModifier = toEquip.effectWhenEquipped
                .GetNewRegisteredModifier(new TargetSelf().GetTargetsFrom(character));
        }

        public void UnEquip() => appliedModifier.UnregisterModifier();
    }
}