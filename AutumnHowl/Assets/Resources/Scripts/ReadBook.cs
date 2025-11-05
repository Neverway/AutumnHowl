using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ReadBook : MonoBehaviour
{
    public string bookName;
    [TextArea] public string[] pages;
    public Volume_TriggerInteract interactTrigger;
    public AudioSource pageOpenSound;
    private string IntroduceBook => $"There is a book here titled <color=#ffad2f>\"{bookName}\".</color> Would you like to read it?";

    public void StartReadingBook()
    {
        GameInstance.SendCoroutine(Co_ReadBook().UsingSeed(100));
    }

    public IEnumerator Co_ReadBook()
    {
        yield return WB_TextChoice.WaitForChoice(IntroduceBook, true, "Yes", "No");
        int selectedIndex = WB_TextChoice.GetLastSelectedChoice();

        yield return null;
        if (selectedIndex == 0)
        {
            int pageIndex = 0;
            while (true)
            {
                Debug.Log($"Random Number = {Random.Range(0, 1000)}");
                string optionLeft = (pages.IsIndexInRange(pageIndex - 1) ? "Previous Page" : "Close Book");
                string optionRight = (pages.IsIndexInRange(pageIndex + 1) ? "Next Page" : "Close Book");
                yield return WB_TextChoice.WaitForChoice(pages[pageIndex], true, optionLeft, optionRight);
                int pageOptionIndex = WB_TextChoice.GetLastSelectedChoice();

                if (pageOptionIndex == 0) pageIndex--;
                else if (pageOptionIndex == 1) pageIndex++;
                else break;

                if (pages.IsIndexOutOfRange(pageIndex))
                    break;
                
                if (pageOpenSound != null)
                    pageOpenSound.Play();
            }
        }
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
