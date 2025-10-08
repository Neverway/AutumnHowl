//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M., Errynei, Connorses
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

public class WB_Battle : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public Image heartImage, powerImage, corruptionImage;
    public List<Sprite> heartSprites, powerSprites, corruptionSprites;
    public GI_AuHoGameState gameState;
    public Animator heartAnimator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
    }

    public void Update()
    {
        var stats = gameState.currentGameState.playerStats;
        
        float percentHealth = stats.health / stats.maxHealth;
        int index = Mathf.FloorToInt(heartSprites.Count * (1f-percentHealth));
        if (index == heartSprites.Count) index--;
        heartImage.sprite = heartSprites[index];
        
        float percentPower = stats.power / (float)stats.maxPower;
        int index2 = Mathf.FloorToInt(powerSprites.Count * (percentPower));
        if (index2 == powerSprites.Count) index2--;
        powerImage.sprite = powerSprites[index2];
        
        float percentCorruption = stats.corruption / (float)stats.maxCorruption;
        int index3 = Mathf.FloorToInt(corruptionSprites.Count * (percentCorruption));
        if (index3 == corruptionSprites.Count) index3--;
        corruptionImage.sprite = corruptionSprites[index3];

        float lowHealthSpeed = 3;
        float maxHealthSpeed = 1;
        float animationSpeed = Mathf.Lerp(lowHealthSpeed , maxHealthSpeed, percentHealth);
        
        heartAnimator.speed = animationSpeed;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
