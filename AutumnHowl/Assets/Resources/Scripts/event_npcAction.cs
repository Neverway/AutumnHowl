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
using UnityEngine.Events;

public class event_npcAction : AutoGUIDObject<bool>
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public bool flagValue;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public UnityEvent onFlagTrue;
    public UnityEvent onFlagFalse;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void SetFlag(bool _flag)
    {
        flagValue = _flag;
    }


    #endregion

    public override bool OnSaveInstance()
    {
        return flagValue;
    }

    public override void OnLoadInstance(bool data)
    {
        flagValue = data;
        if (flagValue)
        {
            onFlagTrue.Invoke();
        }
        else
        {
            onFlagFalse.Invoke();
        }
    }

    public override void OnNewInstance()
    {
    }
}
