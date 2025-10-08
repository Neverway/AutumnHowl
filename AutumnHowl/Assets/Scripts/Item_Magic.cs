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
    public List<UsingEffect> effectsWhenCast;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public override bool Use(Character user, Character target, int _atIndex, int _inList=0)
    {
        foreach (var effect in effectsWhenCast)
        {
            switch (effect.affected)
            {
                case Affected.user:
                    ApplyEffect(effect.effect, effect.amount, user);
                    break;
                case Affected.target:
                    ApplyEffect(effect.effect, effect.amount, target);
                    break;
                case Affected.all:
                    ApplyEffect(effect.effect, effect.amount, user);
                    ApplyEffect(effect.effect, effect.amount, target);
                    break;
            }
        }
        return true;
    }

    #endregion
}
