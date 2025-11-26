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
        money,
        unassignedSkillPoints,
        healthSP,
        powerSP,
        corruptionSP,
        attackSP,
        defenseSP,
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

        if (gameStateVariable == GameStateVariable.level) textContent = PlayerStats.level.ToString();
        if (gameStateVariable == GameStateVariable.health) textContent = PlayerStats.health + " / " + PlayerStats.maxHealth;
        if (gameStateVariable == GameStateVariable.power) textContent = PlayerStats.power + " / " + PlayerStats.maxPower;
        if (gameStateVariable == GameStateVariable.corruption) textContent = PlayerStats.corruption + " / " + PlayerStats.maxCorruption;
        if (gameStateVariable == GameStateVariable.attack) textContent = PlayerStats.attack.ToString();
        if (gameStateVariable == GameStateVariable.defense) textContent = PlayerStats.defense.ToString();
        if (gameStateVariable == GameStateVariable.money) textContent = GameInstance.Gamestate.money.ToString();
        /*
        if (gameStateVariable == GameStateVariable.unassignedSkillPoints) textContent = GameInstance.Gamestate.unassignedSkillPoints.ToString();
        if (gameStateVariable == GameStateVariable.healthSP) textContent = GameInstance.Gamestate.healthSP.ToString();
        if (gameStateVariable == GameStateVariable.powerSP) textContent = GameInstance.Gamestate.powerSP.ToString();
        if (gameStateVariable == GameStateVariable.corruptionSP) textContent = GameInstance.Gamestate.corruptionSP.ToString();
        if (gameStateVariable == GameStateVariable.attackSP) textContent = GameInstance.Gamestate.attackSP.ToString();
        if (gameStateVariable == GameStateVariable.defenseSP) textContent = GameInstance.Gamestate.defenseSP.ToString();*/

        text.text = textDecoratorStart + textContent + textDecoratorEnd;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
