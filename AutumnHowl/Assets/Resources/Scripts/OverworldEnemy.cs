using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemy : AutoGUIDObject<OverworldEnemy.SaveData>
{
    public BattleData battleData;

    bool isDefeated = false;
    bool hasShownDeathAnimation = false;
    Vector3 enteredBattlePosition;

    public void EnterBattle()
    {
        isDefeated = true;
        hasShownDeathAnimation = false;
        enteredBattlePosition = transform.position;

        GameInstance.Gamestate.currentBattle = battleData;
        GameInstance.Get<GI_WorldLoader>().Load(battleData.mapID);
    }

    public override void OnLoadInstance(SaveData saveData)
    {
        isDefeated = saveData.isDefeated;
        hasShownDeathAnimation = saveData.hasShownDeathAnimation;
        enteredBattlePosition = saveData.enteredBattlePosition;
    }
    public override void OnNewInstance() => isDefeated = false;
    public override SaveData OnSaveInstance() => new SaveData()
    {
        isDefeated = isDefeated,
        hasShownDeathAnimation = hasShownDeathAnimation,
        enteredBattlePosition = enteredBattlePosition
    };

    public class SaveData
    {
        public bool isDefeated;
        public bool hasShownDeathAnimation;
        public Vector3 enteredBattlePosition;
    }
}
