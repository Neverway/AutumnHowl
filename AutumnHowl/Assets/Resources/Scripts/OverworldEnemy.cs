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
    public int homeCycle = -1;

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
        if (saveData.homeCycle >= 0 && GameInstance.Gamestate.currentCycle != saveData.homeCycle)
        {
            Destroy(gameObject);
            return;
        }
        homeCycle = saveData.homeCycle;
        transform.position = saveData.homePosition;

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
    public override void OnNewInstance() => OnLoadInstance(new SaveData() 
    { 
        homePosition = transform.position,
        homeCycle = homeCycle
    } );
    public override SaveData OnSaveInstance() => new SaveData()
    {
        isDefeated = isDefeated,
        hasShownDeathAnimation = hasShownDeathAnimation,
        enteredBattlePosition = enteredBattlePosition,

        homePosition = transform.position,
        homeCycle = homeCycle,
    };

    [Serializable]
    public class SaveData
    {
        public int homeCycle = -1;
        public Vector3 homePosition;

        public bool isDefeated = false;
        public bool hasShownDeathAnimation = false;
        public Vector3 enteredBattlePosition;
    }
}
