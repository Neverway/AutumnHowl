using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;
/// <summary>
/// <b> For Polymorphic Serialization</b>
/// <br/> Used to refer to some set of items
/// <br/> Uses:
/// <br/> <see cref="ItemsRef_Item"/> : single item
/// <br/> <see cref="ItemsRef_Items"/> : multiple items
/// <br/> <see cref="ItemsRef_GroupOfItemRefs"/> : use multiple references
/// <br/> <see cref="ItemsRef_LootTable"/> : refer to ScriptableObject of items
/// <br/> <see cref="ItemsRef_WeightedItems"/> : select 1 of multiple item refs randomly by weight
/// </summary>
[Serializable]
public abstract class ItemsReference
{
    public ItemsReferenceSettings settings = new();

    [Serializable]
    public class ItemsReferenceSettings
    {
        [Min(1)] public int repeatGetItems = 1;
        [Range(0f, 1f)] public float chanceToGet = 1f;
        public int itemCountCap = -1;
        [Box, Polymorphic, SerializeReference] public ItemsReference toUseInsteadOfEmptyResult;
    }
    public Item[] GetItemsUnityRandomSeed() => GetItems(Random.Range(int.MinValue, int.MaxValue));
    public Item[] GetItemsGUIDGameSeed(GUIDComponent guidComponent) 
        => GetItems(GameInstance.Gamestate.GetSubSeed(guidComponent.GetGUID()));
    public Item[] GetItems(int newSeed)
    {
        Random.State oldSeedState = Random.state; //Store seed state to restore seed before return
        Random.InitState(newSeed);
        try
        {
            if (!CanGetItems()) return GetFailedItemsSeedSafe(oldSeedState);

            //Get items repeated specified number of times in settings (usually just 1)
            List<Item> totalItems = new List<Item>();
            for (int i = 0; i < settings.repeatGetItems; i++)
            {
                if (SucceedsRandomChance())
                    totalItems.AddRange(OnGetItems());
            }
            //Cap total count of items to specified amount
            if (settings.itemCountCap > 0 && settings.itemCountCap < totalItems.Count)
                totalItems = totalItems.GetRange(0, settings.itemCountCap);

            //Returned failed set of items (empty unless specified) if total count is 0
            if (totalItems.Count == 0) return GetFailedItemsSeedSafe(oldSeedState);

            Random.state = oldSeedState;
            return totalItems.ToArray();
        }
        catch (Exception e) { Debug.LogError("Error trying to generate items somehow"); Debug.LogException(e); }
        
        return GetFailedItemsSeedSafe(oldSeedState); //Set seed back to old state in case of error
    }
    private Item[] GetFailedItemsSeedSafe(Random.State oldSeedState)
    {
        Item[] failedItems = GetFailedItems();
        Random.state = oldSeedState;
        return failedItems;
    }
    public Item[] GetFailedItems()
    {
        if (settings.toUseInsteadOfEmptyResult != null && settings.toUseInsteadOfEmptyResult.CanGetItems())
            return settings.toUseInsteadOfEmptyResult.GetItemsUnityRandomSeed();
        return new Item[0];
    }
    protected abstract Item[] OnGetItems();
    public abstract bool CanGetItems();

    protected bool SucceedsRandomChance() => settings.chanceToGet == 1f || Random.value <= settings.chanceToGet;

}
[Serializable]
public class ItemsRef_Item : ItemsReference
{
    public Item item;
    public override bool CanGetItems() => item != null;
    protected override Item[] OnGetItems() => new Item[] { item };
}
[Serializable]
public class ItemsRef_Items : ItemsReference
{
    public Item[] items;
    public override bool CanGetItems() => items.IsNotEmptyOrNull();
    protected override Item[] OnGetItems() => items;
}

[Serializable]
public class ItemsRef_GroupOfItemRefs : ItemsReference
{
    [Box, Polymorphic, SerializeReference] 
    public ItemsReference[] itemRefs = new ItemsReference[0];

    public override bool CanGetItems()
    {
        //Cant get items if item refs is null or empty
        if (itemRefs.IsEmptyOrNull()) return false;

        //If any single item ref can get items, then we can get items too
        foreach (ItemsReference itemRef in itemRefs)
            if (itemRef.CanGetItems()) return true;

        //Otherwise, we have no item ref that can give items, so we cant either
        return false;
    }
    protected override Item[] OnGetItems()
    {
        List<Item> allItems = new List<Item>();

        foreach (ItemsReference itemRef in itemRefs)
            if (itemRef.CanGetItems())
                allItems.AddRange(itemRef.GetItemsUnityRandomSeed());

        return allItems.ToArray();
    }
}

[Serializable]
public class ItemsRef_LootTable : ItemsReference
{
    public LootTable lootTable;
    public override bool CanGetItems() => lootTable != null && lootTable.itemsToGrant != null;
    protected override Item[] OnGetItems() => lootTable.GetLootUnityRandomSeed();
}

[Serializable]
public class ItemsRef_WeightedItems : ItemsReference
{
    public bool ignoreInvalidItems = true;
    [Unbox] public WeightedItem[] items;
    public override bool CanGetItems() => items.IsNotEmptyOrNull();
    protected override Item[] OnGetItems()
    {
        //Get items we are going to use weights for 
        WeightedItem[] itemsToCheckFor;
        if (ignoreInvalidItems)
            itemsToCheckFor = items.Where(i => i.item != null && i.item.CanGetItems()).ToArray();
        else
            itemsToCheckFor = items;

        //Get total weights of all valid items
        float totalWeights = 0;
        foreach (WeightedItem item in itemsToCheckFor)
            totalWeights += item.weight;

        //Select a random number inbetween 0 and the total of all weights
        float selectedWeightPosition = Random.Range(0, totalWeights);

        //Loop through all items
        foreach (WeightedItem item in itemsToCheckFor)
        {
            selectedWeightPosition -= item.weight;
            if (selectedWeightPosition <= 0)
            {
                if (item.item == null) return new Item[0];
                return item.item.GetItemsUnityRandomSeed();
            }
        }
        Debug.LogError("There was a problem with the logic for weighted items");
        return GetFailedItems();
    }

    [Serializable]
    public struct WeightedItem
    {
        public float weight;
        [Box, Polymorphic, SerializeReference] 
        public ItemsReference item;
    }
}

[Serializable]
public class ItemsRef_Money : ItemsReference
{
    public Vector2Int moneyRange;
    public int roundToNearest = 10;
    public override bool CanGetItems() => true;

    protected override Item[] OnGetItems()
    {
        int min = Mathf.Min(moneyRange.x, moneyRange.y);
        int max = Mathf.Max(moneyRange.x, moneyRange.y);
        int generatedAmount = Random.Range(min, max);
        generatedAmount = (generatedAmount / roundToNearest) * roundToNearest;
        if (generatedAmount <= 0) return new Item[0];

        Item_Money money = ScriptableObject.CreateInstance<Item_Money>();

        money.moneyValue = generatedAmount;
        money.displayName = "{col=stat}" + generatedAmount + "${col=}";

        return new Item[1] { money };
    }
}