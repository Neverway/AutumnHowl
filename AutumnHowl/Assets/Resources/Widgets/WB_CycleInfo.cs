using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WB_CycleInfo : MonoBehaviour
{
    public List<CycleInfo> cycleInfo;
    public string titleText;
    public string subtitleText;
    public TMP_Text title;
    public TMP_Text subtitle;
    public string currentTextContent;
    public float currentTextTypeDelay;
    public Char_ChatterVoice voice;
    public Animator animator;
    public bool printingSubtitle;
    
    // Start is called before the first frame update
    void Start()
    {
        DisplayCycleInfo();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (printingSubtitle)
        {
            subtitle.text = currentTextContent;
        }
        else
        {
            title.text = currentTextContent;
        }
    }
    
    

    private IEnumerator TypeText(string _fullTextContent, bool _printingSubtitle = false)
    {
        var textboxManager = GameInstance.Get<GI_TextboxManager>();
        textboxManager.OverideSetChatterVoice(voice);
        printingSubtitle = _printingSubtitle;
        
        if (!_printingSubtitle) yield return new WaitForSeconds(1);
        
        currentTextContent = "";
        for (int i = 0; i < _fullTextContent.Length; i++)
        {
            textboxManager.PlayChatterSound(currentTextContent.Length, _fullTextContent[i]);
            currentTextContent += _fullTextContent[i];
            yield return new WaitForSeconds(currentTextTypeDelay);
        }

        if (_printingSubtitle == false)
        {
            yield return new WaitForSeconds(currentTextTypeDelay);
            StartCoroutine(TypeText(subtitleText, true));
            print(animator.gameObject.name + " has finished");
            StartCoroutine(CloseCycleInfoAfterDelay());
        }
    }

    private IEnumerator CloseCycleInfoAfterDelay()
    {
        yield return new WaitForSeconds(2);
        animator.Play("Close");
        Destroy(gameObject, 3);
    }

    public void DisplayCycleInfo()
    {
        // Clear text elements
        title.text = "";
        subtitle.text = "";

        GetCurrentCyclesInfo();
        
        animator.Play("Open");
        StartCoroutine(TypeText(titleText));
    }

    /// <summary>
    /// Use the list of cycle info's to display text for the start of each cycle
    /// </summary>
    public void GetCurrentCyclesInfo()
    {
        var gameState = GameInstance.Get<GI_AuHoGameState>().currentGameState;
        if (cycleInfo.Count > gameState.currentCycle)
        {
            titleText = cycleInfo[gameState.currentCycle].title;
            subtitleText = cycleInfo[gameState.currentCycle].subtitle;
        }
        else
        {
            // Fallback to the first cycle info when over indexing past the intended cycles
            titleText = cycleInfo[0].title;
            subtitleText = cycleInfo[0].subtitle;
        }
    }
}

[Serializable]
public class CycleInfo
{
    public string title;
    public string subtitle;
}