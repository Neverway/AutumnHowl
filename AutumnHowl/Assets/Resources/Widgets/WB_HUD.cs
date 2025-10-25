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
    private bool hasLightFaded;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Image lanternFill;
    public GI_AuHoGameState gameState;
    private PlayerLightController playerLightController;
    private Controller_Overworld_Player player;


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

        if (!player)
        {
            player = FindObjectOfType<Controller_Overworld_Player>();
            return;
        }

        // Don't deplete the lantern or take corruption damage when in a light zone
        if (player.inLightZone)
        {
            return;
        }
        
        UpdateTimer();
        UpdateLanternMeter();
    }

    /// <summary>
    /// Update the time remaining in the lantern
    /// </summary>
    private void UpdateTimer()
    {
        if (gameState.currentGameState.currentLanternTime > 0)
        {
            gameState.currentGameState.currentLanternTime -= Time.deltaTime;
        }
        else if (inflictCorruptionCoroutine == null)
        {
            inflictCorruptionCoroutine = StartCoroutine(InflictCorruption());
            if (!hasLightFaded) FadeLights();
        }
    }

    /// <summary>
    /// Update the visuals for the lantern meter
    /// </summary>
    private void UpdateLanternMeter()
    {
        var lanternDuration = gameState.currentGameState.lanternDuration;
        var currentTime = gameState.currentGameState.currentLanternTime;
        lanternFill.fillAmount = currentTime / lanternDuration;
    }

    /// <summary>
    /// Deal corruption damage to the player when their light is out
    /// </summary>
    /// <returns></returns>
    private IEnumerator InflictCorruption()
    {
        yield return new WaitForSeconds(3f);
        
        // Sanity check to absolutely make sure lightzones protect player from corruption
        if (player.inLightZone)
        {
            inflictCorruptionCoroutine = null;
            yield break;
        }
        gameState.currentGameState.player.Stats.ModifyCorruption(+5f);
        inflictCorruptionCoroutine = null;
    }
    
    private void FadeLights()
    {
        hasLightFaded = true;
        playerLightController = FindObjectOfType<PlayerLightController>();
        playerLightController.SetLanternLightState(true);
        GameInstance.Get<GI_AudioManager>().SetMusicPitch(0.6f);
    }

    private void OnDestroy()
    {
        GameInstance.Get<GI_AudioManager>().SetMusicPitch(1f);
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
