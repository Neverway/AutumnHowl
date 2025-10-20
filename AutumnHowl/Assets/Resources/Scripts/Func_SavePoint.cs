//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_SavePoint : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private Func_TextEvent textEvent;

    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public void SaveGame(bool _displaySaveText = true)
    {
        if (_displaySaveText && textEvent)
        {
            var playtime = GameInstance.Get<GI_AuHoGameState>().currentGameState.playtime;
            var formatedTime = TimeSpan.FromSeconds(playtime);
            textEvent.textEvent.frames[0].chatContent = $"[ File 1 ] \n {formatedTime:hh':'mm':'ss} \n Game has been saved!";
            textEvent.CallEvent();
        }        
        GI_SaveSystem.SaveGame();
    }
    

    #endregion
}
