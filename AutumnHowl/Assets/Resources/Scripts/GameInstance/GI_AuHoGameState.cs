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
        return;
        currentGameState = new AuHoGameState()
        {
            player = currentGameState.player,
            currentCycle = currentGameState.currentCycle,
        };
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
        gameSeed = new System.Random().Next(int.MinValue, int.MaxValue);
    }

    public CharacterIdentifier player;
    public BattleData currentBattle;

    //Saved Values --------------------------------------------------
    [Box] public Inventory inventory = new Inventory();

    public string map = "Town";
    public Vector2 overworldPosition;
    public int gameSeed { get; private set; }

    public bool leavingBattle;
    public string enteredBattleFromMap;
    public Vector2 enteredBattleFromLocation;

    public int money = 0;
    public int kills = 0;
    public int deaths = 0;
    public float playtime = 0;

    public float currentLanternTime = 180;
    public float lanternDuration = 180;

    public int currentCycle = 1;
    public int lastDisplayedCycle = 0;

    /// <summary>
    /// Shift to the next cycle, update the actors that should appear, display the title card
    /// </summary>
    public void NextCycle()
    {
        currentCycle++;
        SetCurrentCycle();
        SetCycleAppearances();
    }
    
    /// <summary>
    /// Just update the actors that should appear and display the title card
    /// </summary>
    public void SetCurrentCycle()
    {
        GameInstance.Get<GI_WidgetManager>().AddWidget("WB_CycleInfo");
        lastDisplayedCycle = currentCycle;
        currentLanternTime = lanternDuration;
    }
    
    public int GetSubSeed(string id)
    {
        string seedString = $"{gameSeed}{id}";
        return seedString.GetHashCode();
    }
    public int GetCycleSubSeed(string id)
    {
        string seedString = $"{gameSeed}{currentCycle}{id}";
        return seedString.GetHashCode();
    }
    public void EnterBattle(BattleData battleData)
    {
        //Store previous map and location before entering battle
        enteredBattleFromMap = map;
        enteredBattleFromLocation = overworldPosition;

        //Setup battledata and load the battle map
        currentBattle = battleData;
        GameInstance.Get<GI_WorldLoader>().Load(battleData.mapID);
    }
    public void LeaveBattle()
    {
        //Unset the battle
        currentBattle = null;

        //Use default locations if there was no previous map you came from
        if (string.IsNullOrEmpty(enteredBattleFromMap))
        {
            enteredBattleFromMap = "Town";
            enteredBattleFromLocation = new Vector2(0, 8); //In front of fountain as default position
        }

        //load the player into the map and location they were before the battle
        GameInstance.SendCoroutine(CoLoadMapFromLoadingGame(enteredBattleFromLocation, enteredBattleFromMap));
    }
    public bool IsInBattle => currentBattle != null;

    [InvokeBeforeSave(int.MaxValue - 100)]
    public static void OnGameSave()
    {
        var gameState = GameInstance.Gamestate;

        GameStateSaveData gameStateData = new GameStateSaveData()
        {
            inventorySaveData = gameState.inventory.OnSaveData(),

            map = gameState.map,
            overworldPosition = gameState.overworldPosition,
            gameSeed = gameState.gameSeed,

            enteredBattleFromMap = gameState.enteredBattleFromMap,
            enteredBattleFromLocation = gameState.enteredBattleFromLocation,

            money = gameState.money,
            kills = gameState.kills,
            deaths = gameState.deaths,
            playtime = gameState.playtime,

            currentLanternTime = gameState.currentLanternTime,
            lanternDuration = gameState.lanternDuration,

            currentCycle = gameState.currentCycle,
            lastDisplayedCycle = gameState.lastDisplayedCycle

        };

        GI_SaveSystem.SaveValue(gameStateData, "AuHoGameState");
    }
    
    [InvokeAfterLoad(int.MaxValue - 100)]
    public static void OnGameLoad()
    {
        Debug.Log("Kevin is a stinky lil guy");
        var gameState = GameInstance.Gamestate;

        GameStateSaveData data = GI_SaveSystem.LoadValue<GameStateSaveData>(null, "AuHoGameState");

        if (data != null)
        {
            gameState.inventory.OnLoadData(data.inventorySaveData);

            //Load the map and character position if currently loading a file
            if (GI_SaveSystem.CurrentSavingType == GI_SaveSystem.SavingType.SavingOrLoadingFile)
            {
                gameState.map = data.map;
                Vector2 characterPostiion = data.overworldPosition;
                string map = gameState.map;
                GameInstance.SendCoroutine(CoLoadMapFromLoadingGame(characterPostiion, map));
            }
            gameState.gameSeed = data.gameSeed;

            gameState.enteredBattleFromMap = data.enteredBattleFromMap;
            gameState.enteredBattleFromLocation = data.enteredBattleFromLocation;

            gameState.money = data.money;
            gameState.kills = data.kills;
            gameState.deaths = data.deaths;
            gameState.playtime = data.playtime;

            gameState.currentLanternTime = data.currentLanternTime;
            gameState.lanternDuration = data.lanternDuration;

            gameState.currentCycle = data.currentCycle;
            gameState.lastDisplayedCycle = data.lastDisplayedCycle;
        }

        if (gameState.lastDisplayedCycle != gameState.currentCycle)
        {
            GameInstance.SendCoroutine(CoLoadCycleData());
        }
            
        Debug.Log("Appear");
        SetCycleAppearances();
    }
    [Reload] static bool isLoadingMap = false;
    public static IEnumerator CoLoadMapFromLoadingGame(Vector2 characterPostiion, string mapID)
    {
        if (isLoadingMap) yield break;
        isLoadingMap = true;
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
            player = GameInstance.Playerbody.gameObject;
            Debug.Log("Trying to load map, Looking for player...");
        } while(player == null);

        //Teleport player to given character position
        player.transform.root.position = characterPostiion;
        isLoadingMap = false;
    }
    
    public static IEnumerator CoLoadCycleData()
    {
        var gameState = GameInstance.Gamestate;
        var worldLoader = GameInstance.Get<GI_WorldLoader>();

        //Wait for any previously loading maps to finish loading
        while (worldLoader.IsLoading) yield return null;
        
        Debug.Log("Kevin is a STINK STINK STINK STINKY STINKER");
        // Set map flags
        gameState.SetCurrentCycle();
    }

    [Serializable]
    public class GameStateSaveData
    {
        public Inventory.SaveData inventorySaveData;

        public string map = "Town";
        public Vector2 overworldPosition;
        public int gameSeed;

        public string enteredBattleFromMap;
        public Vector2 enteredBattleFromLocation;

        public int money;
        public int kills;
        public int deaths;
        public float playtime;
        
        public float currentLanternTime;
        public float lanternDuration;

        public int currentCycle = 1;
        public int lastDisplayedCycle;

    }

    private static void SetCycleAppearances()
    {
        var gameState = GameInstance.Gamestate;
        foreach (var target in GameObject.FindObjectsOfType<Object_CycleAppearanceSelector>(true))
        {
            if (target.appearsOnCycle.Count > gameState.currentCycle) target.gameObject.SetActive(target.appearsOnCycle[gameState.currentCycle]);
            else target.gameObject.SetActive(target.appearsOnCycle[0]);
        }
    }
}