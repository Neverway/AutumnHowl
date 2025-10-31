using System.Collections;
using System.Linq;
using UnityEngine;

public class StorageBox : AutoGUIDObject<string>
{
    public Animator animator;
    public string animParam_IsChestEmpty = "ChestIsEmpty";
    public string animParam_OnLoad = "ChestLoaded";

    [Reload] public static StorageBox currentlyUsedBox;
    public float interactRange = 2;
    public Item storedItem;

    public void Update()
    {
        animator.SetBool(animParam_IsChestEmpty, storedItem == null || currentlyUsedBox == this);
        if (currentlyUsedBox != null) return;
        if (GameInstance.Get<GI_TextboxManager>().HasActiveTextEvent) return;
        if (Vector3.Distance(transform.position, GameInstance.Playerbody.transform.position) > interactRange) return;

        if (GameInstance.Inputs.Interact.WasPressedThisFrame())
        {
            currentlyUsedBox = this;
            if (storedItem == null) GameInstance.SendCoroutine(Co_PutItemInBox());
            else GameInstance.SendCoroutine(Co_TakeItemFromBox());
        }
    }
    public IEnumerator Co_PutItemInBox()
    {
        var inventory = GameInstance.Gamestate.inventory;
        string[] itemNames = inventory.items.Select(i => i.displayName).ToArray();

        if (itemNames.IsEmptyOrNull())
        {
            currentlyUsedBox = null;
            yield return Co_DisplayText($"If I had items, I could store one here to save for later");
            yield break;
        }

        yield return WB_TextChoice.WaitForChoice("Which item would you like to store in the box for safekeeping?", true, itemNames);
        int selectedIndex = WB_TextChoice.GetLastSelectedChoice();

        if (inventory.items.IsIndexInRange(selectedIndex))
        {
            Item itemToStore = inventory.items[selectedIndex];
            if (inventory.TryRemoveItem(selectedIndex))
            {
                storedItem = itemToStore;
            }
            else
                yield return Co_DisplayText($"I can't store that");
        }

        currentlyUsedBox = null;
    }
    public IEnumerator Co_TakeItemFromBox()
    {
        //Initialize some variables for easy access and for storing information
        var inventory = GameInstance.Gamestate.inventory;

        //Try to give all items from contained items, and create a text event for each successfully given item
        string itemName = storedItem.displayName;
        if (GameInstance.Gamestate.inventory.TryAddItem(storedItem))
        {
            storedItem = null;
            yield return Co_DisplayText($"You took your {itemName} from storage!");
        }
        else
            yield return Co_DisplayText($"A {itemName} is stored here, but my inventory is too full to take it");

        currentlyUsedBox = null;
    }
    public IEnumerator Co_DisplayText(string text)
    {
        TextEvent textEvent = new TextEvent();
        textEvent.AddFrame(text);
        textEvent.TryDisplay();

        while (GameInstance.Get<GI_TextboxManager>().currentTextEvent == textEvent)
            yield return null;
    }

    public override string OnSaveInstance()
    {
        if (storedItem == null) return null;

        if (storedItem is Item_Money money)
            return money.ConvertToDataString();

        return storedItem.UniqueID;
    }
    public override void OnLoadInstance(string itemID)
    {
        animator.SetTrigger(animParam_OnLoad);
        if (string.IsNullOrWhiteSpace(itemID))
            return;

        if (IDToObj<Item>.TryGet(itemID, out Item item))
        {
            storedItem = item;
            return;
        }
        if (Item_Money.TryConvertDataStringToMoney(itemID, out var money))
        {
            storedItem = money;
            return;
        }
        Debug.LogWarning($"Could not find item ID ({itemID}) for loading items in InteractableChest {name}", this);
    }

    public override void OnNewInstance() { }

































    public void OnDrawGizmos()
    {
        DrawGizmosCircle(Color.magenta, transform.position, interactRange);
    }
    private void DrawGizmosCircle(Color color, Vector3 center, float radius)
    {
        Gizmos.color = color;
        int segments = 32;
        // Build rotation matrix to orient circle
        Quaternion rotation = Quaternion.LookRotation(Vector3.up);
        Vector3 prevPoint = center + rotation * (Vector3.right * radius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 nextPoint = center + rotation * (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }


}
