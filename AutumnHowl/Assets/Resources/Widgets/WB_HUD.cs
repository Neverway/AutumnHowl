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
using Random = UnityEngine.Random;

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

    public Image lanternFlame;
    //public Transform lanternParticles;
    public Color onLanternDrain;
    public Color onLanternSafe;

    private PlayerLightController playerLightController;
    private Controller_Overworld_Player player;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    public void LateUpdate()
    {
        if (GameInstance.Gamestate == null) return;
        if (player == null)
        {
            if (GameInstance.Playerbody is Controller_Overworld_Player playerbody)
                player = playerbody;
            else
                player = FindObjectOfType<Controller_Overworld_Player>();
            return;
        }

        //Set color of lantern and activate particles based on if player is in light
        lanternFill.color = player.inLightZone ? onLanternSafe : onLanternDrain;
        //lanternParticles.gameObject.SetActive(!player.inLightZone);

        // Don't deplete the lantern or take corruption damage when in a light zone
        if (player.inLightZone) return;
        
        UpdateTimer();
        UpdateLanternMeter();
    }

    /// <summary>
    /// Update the time remaining in the lantern
    /// </summary>
    private void UpdateTimer()
    {
        var flameScale = GameInstance.Gamestate.currentLanternTime / GameInstance.Gamestate.lanternDuration;
        lanternFlame.transform.localScale = new Vector3(flameScale, flameScale, flameScale);
        
        if (GameInstance.Gamestate.currentLanternTime > 0)
        {
            GameInstance.Gamestate.currentLanternTime -= Time.deltaTime;
            if (hasLightFaded) UnfadeLights();
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
        var lanternDuration = GameInstance.Gamestate.lanternDuration;
        var currentTime = GameInstance.Gamestate.currentLanternTime;
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
        GameInstance.Gamestate.player.Stats.ModifyCorruption(+5f);
        inflictCorruptionCoroutine = null;
    }
    
    private void FadeLights()
    {
        hasLightFaded = true;
        playerLightController = FindObjectOfType<PlayerLightController>();
        playerLightController.SetLanternLightState(true);
        GameInstance.Get<GI_AudioManager>().SetMusicPitch(0.6f);
    }    
    
    private void UnfadeLights()
    {
        hasLightFaded = false;
        playerLightController = FindObjectOfType<PlayerLightController>();
        playerLightController.SetLanternLightState(false);
        GameInstance.Get<GI_AudioManager>().SetMusicPitch(1f);
    }

    private void OnDestroy()
    {
        GameInstance.Get<GI_AudioManager>().SetMusicPitch(1f);
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    //public struct LanternParticle
    //{
    //    public Transform transform;
    //    public Vector2 velocity;
    //    public float size;
    //    public float timer;
    //    public float maxTimer;
    //
    //    public void InitParticle(Transform transform)
    //    {
    //        Random.State oldState = Random.state;
    //
    //        Random.InitState(new System.Random().Next());
    //        this.transform = transform;
    //        velocity = Vector2.up * Random.Range(-1, -3);
    //        velocity = Vector2.right * Random.Range(-2, 2);
    //
    //        Random.state = oldState;
    //    }
    //    public void UpdateParticle()
    //    {
    //
    //    }
    //}

    #endregion
}
