using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class InteractableChest : AutoGUIDObject<InteractableChest.SaveData>
{
    [Header("Per Instance Parameters")]
    [Box, Polymorphic, SerializeReference] 
    public ItemsReference itemsToGive;

    private List<Item> chestContents;
    private bool hasBeenFullyLooted;

    [Space(50), Header("Functionality Parameters")]
    public Animator animator;
    public string animator_openChestTrigger = "OpenChest";
    public string animator_chestIsEmpty = "ChestIsEmpty";
    public string animator_chestLoadTrigger = "ChestLoaded";
    public GameObject sparkles;

    private TextEvent textEvent = new();

    public void Awake()
    {
        //Initialize chest variables
        chestContents = null;
    }

    public void OpenChest()
    {
        //Dont do any logic if already been looted
        if (hasBeenFullyLooted) return;
        //Must progress through current text event before opening chest
        if (GameInstance.Get<GI_TextboxManager>().HasActiveTextEvent) return;
        //Generate contained items upon first interaction
        if (chestContents == null) GenerateChestContents();

        //Try to put contents of chest into players inventory. If at least one item was, trigger the opening animation
        if (TryPutContentsInPlayersInventory())
        {
            sparkles.SetActive(false);
            animator.SetTrigger(animator_openChestTrigger);
            animator.SetBool(animator_chestIsEmpty, hasBeenFullyLooted); //This bool tells animator to close the chest after opening
        }
    }
    /// <summary>Setup chest contents</summary>
    public void GenerateChestContents()
    {
        //If unable to give items, set contents to none, otherwise pull contents from given items reference
        if (itemsToGive == null || !itemsToGive.CanGetItems())
            chestContents = new List<Item>();
        else
            chestContents = itemsToGive.GetItems().ToList();

        //Set flag for chest to be closed
        hasBeenFullyLooted = false;
        animator.SetBool(animator_chestIsEmpty, hasBeenFullyLooted);
    }

    /// <summary>returns true if any items at all from chest contents were successfully given to the player</summary>
    public bool TryPutContentsInPlayersInventory()
    {
        //Clear text to display. Will be filled with description of what happens next
        textEvent.ClearFrames();
        GameInstance.Get<GI_TextboxManager>().Clear();

        //If no items were generated, explain chest was empty
        if (chestContents.Count == 0)
        {
            hasBeenFullyLooted = true;
            textEvent.AddFrame($"It was empty");
            textEvent.TryDisplay();
            return true;
        }

        //Initialize some variables for easy access and for storing information
        var inventory = GameInstance.Get<GI_AuHoGameState>().currentGameState.inventory;
        List<Item> itemsToRemove = new List<Item>();
        Dictionary<Item, int> removedItemCounts = new Dictionary<Item, int>();

        //Try to give all items from contained items, and create a text event for each successfully given item
        foreach (Item item in chestContents)
            if (inventory.TryAddItem(item))
            {
                //Record which items were removed
                itemsToRemove.Add(item);
                //Also keep track of dictionary for counting number of each unique items
                if (removedItemCounts.ContainsKey(item))
                    removedItemCounts[item] += 1;
                else
                    removedItemCounts.Add(item, 1);
            }

        //Remove items given from the contained items list, and record if chest has been fully looted
        foreach (Item item in itemsToRemove)
            chestContents.Remove(item);
        hasBeenFullyLooted = chestContents.IsEmpty();

        //Add text to text event for all uniqely added items (combining multiple of same type into one text
        foreach (var uniqueItem in removedItemCounts)
        {
            if (uniqueItem.Value == 1)
                textEvent.AddFrame($"You got {uniqueItem.Key.displayName}!");
            else
                textEvent.AddFrame($"You got {uniqueItem.Value}x {uniqueItem.Key.displayName}!");
        }
        //If there are still items remaining, tell player inventory was full
        if (!hasBeenFullyLooted)
        {
            //Change text slightly depending on if any items were taken out
            if (itemsToRemove.Count > 0)
                textEvent.AddFrame($"Inventory is full, no room for {chestContents.Count} remaining items");
            else
                textEvent.AddFrame($"Inventory is full");
        }

        //Display text event to the screen
        textEvent.TryDisplay();

        //Return whether or not any items were actually removed
        return itemsToRemove.Count > 0;
    }


    // SaveData handling ----------------------------------------------------------------------------------------
    public override SaveData OnSaveInstance()
    {
        SaveData data = new SaveData();

        data.hasItems = chestContents.IsNotEmptyOrNull();
        if (data.hasItems)
            data.heldItemsIDs = chestContents.Select(item => item.UniqueID).ToArray();

        data.looted = hasBeenFullyLooted;

        return data;
    }

    public override void OnLoadInstance(SaveData data)
    {
        hasBeenFullyLooted = data.looted;

        if (data.hasItems)
        {
            chestContents = new List<Item>();
            foreach (string id in data.heldItemsIDs)
            {
                if (IDToObj<Item>.TryGet(id, out Item item))
                {
                    chestContents.Add(item);
                }
                else
                    Debug.LogWarning($"Could not find item ID ({id}) for loading items in InteractableChest {name}", this);
            }
        }
        else
            chestContents = null;


        sparkles.SetActive(!hasBeenFullyLooted && chestContents == null);
        animator.SetBool(animator_chestIsEmpty, hasBeenFullyLooted);
        animator.SetTrigger(animator_chestLoadTrigger);
    }

    public override void OnNewInstance() { }

    [Serializable]
    public struct SaveData
    {
        public string[] heldItemsIDs;
        public bool hasItems;
        public bool looted;
    }
}
