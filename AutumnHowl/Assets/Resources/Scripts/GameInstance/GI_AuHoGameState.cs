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
using Random = UnityEngine.Random;

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
    public void Awake()
    {
        currentGameState = new AuHoGameState() { currentBattle = currentGameState.currentBattle };
    }
    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    [ContextMenu("NEXT CYCLE")]
    private void NextCycleTEST()
    {
        currentGameState.NextCycle();
    }

    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/


    #endregion
}

[Serializable]
public class AuHoGameState
{
    public AuHoGameState()
    {
        currentCycleSeed = GetRandomSeedInt();
        nextCycleSeed = GetRandomSeedInt();
    }

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
    public int currentCycle = 0;
    public int currentCycleSeed;
    public int nextCycleSeed;
    public void NextCycle()
    {
        currentCycle++;
        NewSeed();
    }
    public void NewSeed()
    {
        currentCycleSeed = nextCycleSeed;
        nextCycleSeed = GetRandomSeedInt();
    }
    public int GetRandomSeedInt() => new System.Random().Next(int.MinValue, int.MaxValue);

    [InvokeBeforeSave]
    public static void OnGameSave()
    {
        var gameState = GameInstance.Gamestate;

        GameStateSaveData gameStateData = new GameStateSaveData()
        {
            inventorySaveData = gameState.inventory.OnSaveData(),

            money = gameState.money,
            kills = gameState.kills,
            deaths = gameState.deaths,
            playtime = gameState.playtime,

            currentLanternTime = gameState.currentLanternTime,
            lanternDuration = gameState.lanternDuration,

            currentCycle = gameState.currentCycle,
            currentCycleSeed = gameState.currentCycleSeed,
            nextCycleSeed = gameState.nextCycleSeed,

            map = gameState.map,
            overworldPosition = gameState.overworldPosition,
        };

        GI_SaveSystem.SaveValue(gameStateData, "AuHoGameState");
    }
    [InvokeAfterLoad]
    public static void OnGameLoad()
    {
        var gameState = GameInstance.Gamestate;
        gameState.NewSeed();


        GameStateSaveData data = GI_SaveSystem.LoadValue<GameStateSaveData>(null, "AuHoGameState");

        if (data != null)
        {
            gameState.inventory.OnLoadData(data.inventorySaveData);

            gameState.money = data.money;
            gameState.kills = data.kills;
            gameState.deaths = data.deaths;
            gameState.playtime = data.playtime;

            gameState.currentCycle = data.currentCycle;
            gameState.currentCycleSeed = data.currentCycleSeed;
            gameState.nextCycleSeed = data.nextCycleSeed;

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
        var gameState = GameInstance.Gamestate;
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

        public int currentCycle;
        public int currentCycleSeed;
        public int nextCycleSeed;

        public string map = "Town";
        public Vector2 overworldPosition;
    }
}