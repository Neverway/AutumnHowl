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

public abstract class Item : ScriptableObject
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public string id;
    public string displayName;
    [TextArea, SerializeField] protected string description;
    public bool canNotDiscard;
    public bool allowMultiple=true;
    public int buyCost;
    public int sellCost;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /*
    public virtual void ApplyEffect(Character _user)
    {

        
        switch (_effect)
        {
            case Effect.heal:
                _target.ModifyHealth(_amount);
                break;
            case Effect.damage:
                _target.ModifyHealth(-_amount);
                break;
            case Effect.corrupt:
                _target.currentStats.corruption+=(int)_amount;
                break;
            case Effect.attack:
                _target.currentStats.attack+=(int)_amount;
                break;
            case Effect.strength:
                _target.currentStats.power+=(int)_amount;
                break;
            case Effect.defense:
                _target.currentStats.defense+=(int)_amount;
                break;
        }
        
    }
        // */

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual string GetDescription() => description;
    public abstract bool Use(Character user, int _atIndex, int _inList = 0);


    #endregion
}
