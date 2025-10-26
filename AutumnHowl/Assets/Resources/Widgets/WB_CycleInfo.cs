using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WB_CycleInfo : MonoBehaviour
{
    public string titleText;
    public string subtitleText;
    public TMP_Text title;
    public TMP_Text subtitle;
    public string currentTextContent;
    public float currentTextTypeDelay;
    public Char_ChatterVoice voice;

    public bool printingSubtitle;
    
    // Start is called before the first frame update
    void Start()
    {
        title.text = "";
        subtitle.text = "";
        StartCoroutine(TypeText(titleText));
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
            Destroy(gameObject, 5);
        }
    }
}
