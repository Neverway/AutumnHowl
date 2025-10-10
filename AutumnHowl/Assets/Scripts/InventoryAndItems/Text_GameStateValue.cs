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
        currentHealth,
        maxHealth,
        power,
        corruption,
        attack,
        defense,
        money
    }


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private TMP_Text text;
    private GI_AuHoGameState gameState;


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
        if (!gameState)
        {
            gameState = GameInstance.Get<GI_AuHoGameState>();
            return;
        }

        switch (gameStateVariable)
        {
            case GameStateVariable.level:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.level + textDecoratorEnd;
                break;
            case GameStateVariable.currentHealth:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.health + textDecoratorEnd;
                break;
            case GameStateVariable.maxHealth:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.health + textDecoratorEnd;
                break;
            case GameStateVariable.power:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.power + textDecoratorEnd;
                break;
            case GameStateVariable.corruption:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.corruption + textDecoratorEnd;
                break;
            case GameStateVariable.attack:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.attack + textDecoratorEnd;
                break;
            case GameStateVariable.defense:
                text.text = textDecoratorStart + gameState.currentGameState.playerStats.defense + textDecoratorEnd;
                break;
            case GameStateVariable.money:
                text.text = textDecoratorStart + gameState.currentGameState.money + textDecoratorEnd;
                break;
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
