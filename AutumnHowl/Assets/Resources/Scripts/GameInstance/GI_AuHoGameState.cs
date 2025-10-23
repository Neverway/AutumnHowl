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
using System.Linq;
using UnityEngine;

public class GI_AuHoGameState : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [Box] public AuHoGameState currentGameState;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    
    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}

[Serializable]
public class AuHoGameState
{
    public CharacterIdentifier player;
    public BattleData currentBattle;

    //Saved Values --------------------------------------------------
    [Box] public Inventory inventory = new Inventory();

    public string map = "Town";
    public Vector2 overworldPosition;

    public int money = 0;
    public int kills = 0;
    public int deaths = 0;
    public float playtime = 0;
    public float currentLanternTime = 1200;
    public float lanternDuration = 1200;

    [InvokeBeforeSave]
    public static void OnGameSave()
    {
        var gameState = GameInstance.Get<GI_AuHoGameState>().currentGameState;

        GameStateSaveData gameStateData = new GameStateSaveData()
        {
            inventorySaveData = gameState.inventory.OnSaveData(),

            money = gameState.money,
            kills = gameState.kills,
            deaths = gameState.deaths,
            playtime = gameState.playtime,

            currentLanternTime = gameState.currentLanternTime,
            lanternDuration = gameState.lanternDuration,

            map = gameState.map,
            overworldPosition = gameState.overworldPosition,
        };

        GI_SaveSystem.SaveValue(gameStateData, "AuHoGameState");
    }
    [InvokeAfterLoad]
    public static void OnGameLoad()
    {
        var gameState = GameInstance.Get<GI_AuHoGameState>().currentGameState;

        GameStateSaveData data = GI_SaveSystem.LoadValue<GameStateSaveData>(null, "AuHoGameState");
        
        if (data != null)
        {
            gameState.inventory.OnLoadData(data.inventorySaveData);

            gameState.money = data.money;
            gameState.kills = data.kills;
            gameState.deaths = data.deaths;
            gameState.playtime = data.playtime;
            
            gameState.currentLanternTime = data.currentLanternTime;
            gameState.lanternDuration = data.lanternDuration;

            //Load the map and character position if currently loading a file
            if (GI_SaveSystem.CurrentSavingType == GI_SaveSystem.SavingType.SavingOrLoadingFile)
            {
                gameState.map = data.map;
                Vector2 characterPostiion = data.overworldPosition;
                string map = gameState.map;
                GameInstance.SendCoroutine(CoLoadMapFromLoadingGame(characterPostiion, map));
            }
        }
    }
    public static IEnumerator CoLoadMapFromLoadingGame(Vector2 characterPostiion, string mapID)
    {
        var gameState = GameInstance.Get<GI_AuHoGameState>().currentGameState;
        var worldLoader = GameInstance.Get<GI_WorldLoader>();

        //Wait for any previously loading maps to finish loading
        while (worldLoader.IsLoading) yield return null;
        //Load new map
        worldLoader.Load(mapID);
        //Wait for the new map to finish loading
        while (worldLoader.IsLoading) yield return null;

        //Look for the player, and wait until they are found
        GameObject player;
        do {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("Trying to load map, Looking for player...");
        } while(player == null);

        //Teleport player to given character position
        player.transform.root.position = characterPostiion;
    }

    [Serializable]
    public class GameStateSaveData
    {
        public Inventory.SaveData inventorySaveData;

        public int money;
        public int kills;
        public int deaths;
        public float playtime;
        
        public float currentLanternTime;
        public float lanternDuration;

        public string map = "Town";
        public Vector2 overworldPosition;
    }
}