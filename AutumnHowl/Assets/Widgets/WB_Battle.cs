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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WB_Battle : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [Header("Heartbeat Stuff")]
    public Image heartImage;
    public Image powerImage, corruptionImage;
    public List<Sprite> heartSprites, powerSprites, corruptionSprites;
    public Animator heartAnimator;
    private GI_AuHoGameState gameState;
    [Header("Action Stuff")] 
    public Animator actionBarAnimator;
    public GameObject attackBar;
    public Image attackBarLeft, attackBarRight;
    [Header("Step Stuff")] 
    public TMP_Text stepCountText;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        gameState = GameInstance.Get<GI_AuHoGameState>();
        SetActionBarVisible(false);
    }

    public void Update()
    {
        var stats = gameState.currentGameState.player.Stats;
        
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
    public void SetActionBarVisible(bool _isVisible)
    {
        switch (_isVisible)
        {
            case true:
                actionBarAnimator.GetComponent<WidgetNavigator>().SetIsNavigating(true);
                actionBarAnimator.Play("Open");
                break;
            case false:
                actionBarAnimator.GetComponent<WidgetNavigator>().SetIsNavigating(false);
                actionBarAnimator.Play("Close");
                break;
        }
    }
    public void SetAttackBarVisible(bool _isVisible)
    {
        switch (_isVisible)
        {
            case true:
                attackBar.SetActive(true);
                break;
            case false:
                attackBar.SetActive(true);
                break;
        }
    }


    #endregion
}
