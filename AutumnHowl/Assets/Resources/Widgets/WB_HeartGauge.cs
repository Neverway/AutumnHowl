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

public class WB_HeartGauge : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private GI_AuHoGameState gameState;
    [Tooltip("The UI images that represent the different levels for this stat")]
    [SerializeField] private Image heartImage, powerImage, corruptionImage;
    [Tooltip("The sprites that represent the different levels for this stat")]
    [SerializeField] private List<Sprite> heartSprites, powerSprites, corruptionSprites;
    [Tooltip("The animator that controls the beating of the heart (used to speed it up on low health)")]
    [SerializeField] private Animator heartAnimator;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
    }

    private void Update()
    {
        if (gameState.currentGameState.player == null) return;
        var stats = gameState.currentGameState.player.Stats;
        
        // Set heartbeat stuff
        // Set the heart sprite according to the player's health
        float percentHealth = stats.health / stats.maxHealth;
        int index = Mathf.FloorToInt(heartSprites.Count * (1f-percentHealth));
        if (index == heartSprites.Count) index--;
        heartImage.sprite = heartSprites[index];
        
        // Set the heart sprite according to the player's power
        float percentPower = stats.power / (float)stats.maxPower;
        int index2 = Mathf.FloorToInt(powerSprites.Count * (percentPower));
        if (index2 == powerSprites.Count) index2--;
        powerImage.sprite = powerSprites[index2];
        
        // Set the heart sprite according to the player's corruption
        float percentCorruption = stats.corruption / (float)stats.maxCorruption;
        int index3 = Mathf.FloorToInt(corruptionSprites.Count * (percentCorruption));
        if (index3 == corruptionSprites.Count) index3--;
        corruptionImage.sprite = corruptionSprites[index3];

        // Set how fast the heart is beating based on how low the player's health is
        float lowHealthSpeed = 3;
        float maxHealthSpeed = 1;
        float animationSpeed = Mathf.Lerp(lowHealthSpeed , maxHealthSpeed, percentHealth);
        
        heartAnimator.speed = animationSpeed;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
