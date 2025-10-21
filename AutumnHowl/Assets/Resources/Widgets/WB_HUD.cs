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
using UnityEngine.UI;

public class WB_HUD : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private Coroutine inflictCorruptionCoroutine;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Image lanternFill;
    public GI_AuHoGameState gameState;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    public void LateUpdate()
    {
        if (gameState == null)
        {
            gameState = GameInstance.Get<GI_AuHoGameState>();
            return;
        }

        UpdateTimer();
        UpdateLanternMeter();
    }

    private void UpdateTimer()
    {
        if (gameState.currentGameState.currentLanternTime > 0)
        {
            gameState.currentGameState.currentLanternTime -= Time.deltaTime;
        }
        else if (inflictCorruptionCoroutine == null)
        {
            inflictCorruptionCoroutine = StartCoroutine(InflictCorruption());
        }
    }

    private void UpdateLanternMeter()
    {
        var lanternDuration = gameState.currentGameState.lanternDuration;
        var currentTime = gameState.currentGameState.currentLanternTime;
        lanternFill.fillAmount = currentTime / lanternDuration;
    }

    private IEnumerator InflictCorruption()
    {
        yield return new WaitForSeconds(3f);
        gameState.currentGameState.player.Stats.ModifyCorruption(+5f);
        inflictCorruptionCoroutine = null;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
