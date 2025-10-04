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
public class Text_GoldCounter : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public string textDecoratorStart = "$ ";
    public string textDecoratorEnd = "";


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
        
        text.text = textDecoratorStart + gameState.currentGameState.money + textDecoratorEnd;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}
