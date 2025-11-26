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
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Text_GameStateValue : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public string textDecoratorStart = "$ ";
    public string textDecoratorEnd = "";
    public GameStateVariable gameStateVariable;
    public enum GameStateVariable
    {
        level,
        health,
        power,
        corruption,
        attack,
        defense,
        money
    }


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private string textContent;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private TMP_Text text;
    private GI_AuHoGameState gameState;
    private CharacterStats PlayerStats => gameState.currentGameState.player.Stats;

    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        text = GetComponent<TMP_Text>();
        gameState = GameInstance.Get<GI_AuHoGameState>();
    }

    private void Update()
    {
        if (gameState == null)
        {
            gameState = GameInstance.Get<GI_AuHoGameState>();
            return;
        }

        switch (gameStateVariable)
        {
            case GameStateVariable.level:
                textContent = PlayerStats.level.ToString();
                break;
            case GameStateVariable.health:
                textContent = PlayerStats.health + " / " + PlayerStats.maxHealth;
                break;
            case GameStateVariable.power:
                textContent = PlayerStats.power + " / " + PlayerStats.maxPower;
                break;
            case GameStateVariable.corruption:
                textContent = PlayerStats.corruption + " / " + PlayerStats.maxCorruption;
                break;
            case GameStateVariable.attack:
                textContent = PlayerStats.attack.ToString();
                break;
            case GameStateVariable.defense:
                textContent = PlayerStats.defense.ToString();
                break;
            case GameStateVariable.money:
                textContent = GameInstance.Gamestate.money.ToString();
                break;
        }

        text.text = textDecoratorStart + textContent + textDecoratorEnd;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
