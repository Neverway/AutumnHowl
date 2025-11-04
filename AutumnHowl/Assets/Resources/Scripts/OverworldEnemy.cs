using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class OverworldEnemy : AutoGUIDObject<OverworldEnemy.SaveData>
{
    public BattleData battleData;
    [Box, Polymorphic, SerializeReference] public ItemsReference itemsOnDefeat;
    [Space]
    public Transform enemyWander;
    public Transform enemyDefeatAnimation;
    public Transform enemyRemains;

    bool isDefeated = false;
    bool hasShownDeathAnimation = false;
    Vector3 enteredBattlePosition;
    public int homeCycle = -1;

    private SaveData DEBUG_LastLoadedData;

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

    public IEnumerator CoGivePlayerDefeatLoot()
    {
        yield return null;
        yield return null;

        if (itemsOnDefeat == null || (!itemsOnDefeat.CanGetItems()))
            yield break;

        Item[] generatedItems = itemsOnDefeat.GetItemsGUIDGameSeed(this);

        if (generatedItems.Length == 0) yield break;

        //Initialize some variables for easy access and for storing information
        var inventory = GameInstance.Get<GI_AuHoGameState>().currentGameState.inventory;
        Dictionary<Item, int> givenItems = new Dictionary<Item, int>();

        //Try to give all items from contained items, and create a text event for each successfully given item
        foreach (Item item in generatedItems)
            if (inventory.TryAddItem(item))
            {
                //Also keep track of dictionary for counting number of each unique items
                if (givenItems.ContainsKey(item))
                    givenItems[item] += 1;
                else
                    givenItems.Add(item, 1);
            }

        //Add text to text event for all uniqely added items (combining multiple of same type into one text)
        TextEvent gotItemsTextEvent = new TextEvent();
        foreach (var uniqueItem in givenItems)
        {
            if (uniqueItem.Value == 1)
                gotItemsTextEvent.AddFrame($"You got {uniqueItem.Key.displayName}!");
            else
                gotItemsTextEvent.AddFrame($"You got {uniqueItem.Value}x {uniqueItem.Key.displayName}!");
        }

        //Display text event to the screen
        gotItemsTextEvent.TryDisplay();
    }


    public override void OnLoadInstance(SaveData saveData)
    {
        DEBUG_LastLoadedData = saveData;
        if (saveData == null)
            return;

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
            GameInstance.SendCoroutine(CoGivePlayerDefeatLoot());
            enemyDefeatAnimation.transform.position = enteredBattlePosition;
            hasShownDeathAnimation = true;
        }
        enemyRemains.transform.position = enteredBattlePosition;
        enemyRemains.gameObject.SetActive(saveData.isDefeated && saveData.hasShownDeathAnimation);

    }
    public override void OnNewInstance() { }
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
