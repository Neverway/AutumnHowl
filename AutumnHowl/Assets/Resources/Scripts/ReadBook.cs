using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadBook : MonoBehaviour
{
    public string bookName;
    public string[] pages;
    public Volume_TriggerInteract interactTrigger;

    private string IntroduceBook => $"There is a book here titled <color=>\"{bookName}\".</color> Would you like to read it?";

    private string pageIndex;
    public IEnumerator Co_ReadBook()
    {
        //yield return WB_TextChoice.WaitForChoice(, true, itemNames);
        //int selectedIndex = WB_TextChoice.GetLastSelectedChoice();
        //
        //if (inventory.items.IsIndexInRange(selectedIndex))
        //{
        //    Item itemToStore = inventory.items[selectedIndex];
        //    if (inventory.TryRemoveItem(selectedIndex))
        //    {
        //        storedItem = itemToStore;
        //    }
        //    else
        //        yield return Co_DisplayText($"I can't store that");
        //}

        yield return interactTrigger.CO_ResetActive();
    }

    public IEnumerator Co_DisplayText(string text)
    {
        TextEvent textEvent = new TextEvent();
        textEvent.AddFrame(text);
        textEvent.TryDisplay();

        while (GameInstance.Get<GI_TextboxManager>().currentTextEvent == textEvent)
            yield return null;
    }
}
