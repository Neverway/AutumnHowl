//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random=UnityEngine.Random;

/// <summary>
/// The Scriptable Object sent by the enemy to the GI so that the battle system knows who you are fighting
/// </summary>
[CreateAssetMenu(menuName = "AuHo/New Battle Data", fileName = "Battle_")]
public class BattleData : ScriptableObject
{
    [Header("Map")]
#if UNITY_EDITOR
    [Tooltip("Drag in a scene to override the default battle scene, the mapID will be gotten from this field")]
    public SceneAsset targetLevelScene;
#endif
    [Tooltip("The id of the battle scene (This gets overriden when setting targetLevelScene)")]
    public string mapID = "Battle";
    [Tooltip("Override the default battle music, leave as none to use the default battle track")]
    [FormerlySerializedAs("music")] public GI_AudioManager.Music musicOverride;
    [Tooltip("The prefab to spawn to create obstacles and background scenery")]
    [FormerlySerializedAs("layoutPrefab")] public GameObject obstacleLayoutPrefab;
    [Header("Battle")]
    [Tooltip("The dialogue to display when the battle first begins")]
    public TextEvent openingText;
    [Tooltip("What enemies to spawn on the battle grid when the battle first begins")]
    public List<EnemySpawnLocation> enemySpawnLocations;
    [Tooltip("The order of waves and turn steps (progressing through waves is usually done by the individual battle character scripts)")]
    public BattleSequence battleSequence;
    [Header("Victory")]
    [Tooltip("What condition must be met for the battle to end")]
    [Box, SerializeReference, Polymorphic] public VictoryState victoryState;
    [Tooltip("How many levels to reward the player on winning a battle")]
    public int victoryLevels;
    [Tooltip("How much gold to reward the player on winning a battle")]
    public int victoryGold;


    private void OnValidate()
    {
#if UNITY_EDITOR 
        if (targetLevelScene == null) return;
        mapID = targetLevelScene.name;
#endif
    }
}

/// <summary>
/// A list of battle waves
/// </summary>
[Serializable]
public class BattleSequence
{
    [Polymorphic, SerializeReference] public BattleWaveSelector[] waves;

    public BattleWave GetBattleWave()
    {
        var currentWave = GameInstance.Gamestate.currentBattleWave;
        Debug.LogWarning($"CURRENT WAVE IS {currentWave}");
        
        // If the last wave has been exceeded, wrap back around to wave 0
        if (currentWave > waves.Length)
        {
            currentWave = 0;
        }
        
        // Return the current wave info
        return waves[currentWave].GetBattleWave();
    }
}

/// <summary>
/// ???
/// </summary>
[Serializable]
public abstract class BattleWaveSelector
{
    public abstract BattleWave GetBattleWave();
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class BattleWave : BattleWaveSelector
{
    [Tooltip("This is just a short title for the purpose of this wave, (it's only used for info in the inspector)")]
    public string waveDescription;
    [Tooltip("The text to display when first starting this wave, (It's the first flavour text before the player chooses an action)")]
    [FormerlySerializedAs("waveText")] public TextEvent waveStartText;
    [Tooltip("The text to display when repeating this wave")]
    [FormerlySerializedAs("waveText")] public TextEvent waveRepeatText;
    [Tooltip("How many turn-steps each fighter gets during this wave")]
    public int waveSteps;
    
    public override BattleWave GetBattleWave()
    {
        return this;
    }
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class RandoBattleWave : BattleWaveSelector
{
    [Polymorphic, SerializeReference] public BattleWaveSelector[] waves = new BattleWaveSelector[0];
    
    public override BattleWave GetBattleWave()
    {
        if (waves.Length == 0) { throw new Exception("WTF??? A random battle wave is defined, but with no waves."); }
        return waves[Random.Range(0, waves.Length)].GetBattleWave();
    }
}

[Serializable]
public class EnemySpawnLocation
{
    public GameObject enemyPrefab;
    public Vector2Int enemyStartPosition = new Vector2Int (3, 6);
}

[Serializable]
public abstract class VictoryState
{
    public abstract bool victoryConditionMet(List<Char_Battle> _aliveFighters);
}

[Serializable]
public class AllEnemiesDefeated : VictoryState
{
    public override bool victoryConditionMet(List<Char_Battle> _aliveFighters)
    {
        return !_aliveFighters.Any(character => character is not IsPlayerCharacter);
    }
}

[Serializable]
public class TargetEnemyDefeated : VictoryState
{
    public CharacterTemplate characterTemplate;
    public override bool victoryConditionMet(List<Char_Battle> _aliveFighters)
    {
        return _aliveFighters.All(character => character.Identifier.TemplateCreatedFrom != characterTemplate);
    }
}

[Serializable]
public class PuzzleCompleted : VictoryState
{
    private PuzzleFlag flag;

    public override bool victoryConditionMet(List<Char_Battle> _aliveFighters)
    {
        if (flag == null)
        {
            if (!IDToObj<PuzzleFlag>.TryGet(PuzzleFlag.REFERENCE_ID, out flag) || flag == null)
                return false;
        }
        return flag.CheckForWin();
    }
}