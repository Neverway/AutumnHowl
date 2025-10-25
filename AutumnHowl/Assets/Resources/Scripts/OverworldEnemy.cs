using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemy : AutoGUIDObject<OverworldEnemy.SaveData>
{
    public BattleData battleData;
    [Space]
    public Transform enemyWander;
    public Transform enemyDefeatAnimation;
    public Transform enemyRemains;

    bool isDefeated = false;
    bool hasShownDeathAnimation = false;
    Vector3 enteredBattlePosition;

    public void EnterBattle()
    {
        //Setup values to show death animation when they reenter the scene
        isDefeated = true;
        hasShownDeathAnimation = false;
        enteredBattlePosition = enemyWander.transform.position;

        //Set current battle and load battle scene from battle data
        GameInstance.Gamestate.EnterBattle(battleData);
    }
    public void FinishDeathAnimation()
    {
        isDefeated = true;
        hasShownDeathAnimation = true;

        enemyDefeatAnimation.gameObject.SetActive(false);

        enemyRemains.transform.position = enteredBattlePosition;
        enemyRemains.gameObject.SetActive(true);
    }

    public override void OnLoadInstance(SaveData saveData)
    {
        isDefeated = saveData.isDefeated;
        hasShownDeathAnimation = saveData.hasShownDeathAnimation;
        enteredBattlePosition = saveData.enteredBattlePosition;

        enemyWander.gameObject.SetActive(!saveData.isDefeated);
        bool showDeathAnimation = saveData.isDefeated && !saveData.hasShownDeathAnimation;
        enemyDefeatAnimation.gameObject.SetActive(showDeathAnimation);
        if (showDeathAnimation)
        {
            enemyDefeatAnimation.transform.position = enteredBattlePosition;
            hasShownDeathAnimation = true;
        }
        enemyRemains.transform.position = enteredBattlePosition;
        enemyRemains.gameObject.SetActive(saveData.isDefeated && saveData.hasShownDeathAnimation);
    }
    public override void OnNewInstance() => OnLoadInstance(new SaveData());
    public override SaveData OnSaveInstance() => new SaveData()
    {
        isDefeated = isDefeated,
        hasShownDeathAnimation = hasShownDeathAnimation,
        enteredBattlePosition = enteredBattlePosition
    };

    [Serializable]
    public class SaveData
    {
        public bool isDefeated = false;
        public bool hasShownDeathAnimation = false;
        public Vector3 enteredBattlePosition;
    }
}
