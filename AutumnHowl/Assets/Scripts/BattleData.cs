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
using UnityEngine;
using Random=UnityEngine.Random;

/// <summary>
/// The Scriptable Object sent by the enemy to the GI so that the battle system knows who you are fighting
/// </summary>
[CreateAssetMenu(menuName = "AuHo/New Battle Data", fileName = "Battle_")]
public class BattleData : ScriptableObject
{
    public TextEvent openingText;
    public BattleSequence battleSequence;
    public GameObject enemyPrefab;
    public Vector2Int enemyStartPosition = new Vector2Int(3,6);
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class BattleSequence
{
    [Polymorphic, SerializeReference] public BattleWaveSelector[] waves;

    public BattleWave GetBattleWave()
    {
        return waves[0].GetBattleWave();
    }
}

/// <summary>
///
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
    public TextEvent waveText;
    public GameObject waveAttack;
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

