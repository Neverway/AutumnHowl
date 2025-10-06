//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GI_TextboxManager : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public float normalTextTypeDelay, skippingTextTypeDelay;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public TextEvent currentTextEvent;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool textEventActive;
    private bool currentlyPrinting;
    private string currentTextContent;
    private float currentTextTypeDelay;
    private int currentFrame;
    private bool performingRegularMarkup, performingSpecialMarkup;
    private Vector2Int specialMarkupIndex;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private InputActions.TopDownActions inputActions;
    private GI_WidgetManager widgetManager;
    private WB_Textbox textbox;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        // Setup inputs
        inputActions = new InputActions().TopDown;
        inputActions.Enable();
    }

    public void Update()
    {
        if (textEventActive)
        {
            if (textbox == null)
            {
                return;
            }

            var currentEventFrame = currentTextEvent.frames[currentFrame];
            
            // Send current frame to textbox
            textbox.portrait.sprite = currentEventFrame.portrait;
            textbox.name.text = currentEventFrame.name;
            textbox.chat.text = currentTextContent;
            
            // Handel pressing the skip text button
            if (currentEventFrame.preventTextSkipping is false)
            {
                if (inputActions.Action.WasPressedThisFrame()) currentTextTypeDelay = skippingTextTypeDelay;
                if (inputActions.Action.WasReleasedThisFrame()) currentTextTypeDelay = normalTextTypeDelay;
            }
            
            // Handel move next frame inputs
            if (currentEventFrame.preventTextContinuing is false)
            {
                if (inputActions.Interact.WasPressedThisFrame() && currentlyPrinting is false)
                {
                    PrintNextFrame();
                }
            }
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void StartTextEvent()
    {
        // Open or get the textbox
        GetTextbox();
        
        // Display the first frame
        currentFrame = -1;
        PrintNextFrame();
        
        // Enable inputs to move next
        textEventActive = true;
    }

    /// <summary>
    /// Find or create the textbox widget
    /// </summary>
    private void GetTextbox()
    {
        if (!widgetManager) widgetManager = GameInstance.Get<GI_WidgetManager>();
        if (!textbox)
        {
            widgetManager.AddWidget("WB_Textbox");
            textbox = widgetManager.GetExistingWidget("WB_Textbox").GetComponent<WB_Textbox>();
        }
    }

    private void PrintNextFrame()
    {
        if (MoveNext())
        {
            var currentEventFrame = currentTextEvent.frames[currentFrame];
            StartCoroutine(TypeText(currentEventFrame.chatContent, currentEventFrame.OnFrameCompleted));
        }
    }

    private IEnumerator TypeText(string _fullTextContent, UnityEvent _onFrameCompleted)
    {
        // Set Displaymode
        textbox.displayMode = currentTextEvent.frames[currentFrame].displayMode;
        
        currentTextContent = "";
        currentlyPrinting = true;
        for (int i = 0; i < _fullTextContent.Length; i++)
        {
            // Check for markups
            CheckForMarkups(_fullTextContent, i, out i);
            if (i >= _fullTextContent.Length) break;
            
            var currentChar = _fullTextContent[i];
            if (!performingSpecialMarkup) currentTextContent += currentChar;

            if (!performingSpecialMarkup && !performingRegularMarkup)
            {
                yield return new WaitForSeconds(currentTextTypeDelay);
            }
        }
        currentlyPrinting = false;

        _onFrameCompleted.Invoke();
        
        if (currentTextEvent.frames[currentFrame].autoProgressOnComplete) PrintNextFrame();
    }

    private void CheckForMarkups(string _fullTextContent, int _index, out int _outIndex)
    {
        var outIndexResult = _index;
        var substringCharacter = _fullTextContent[_index];
        if (substringCharacter == '<') performingRegularMarkup = true;
        if (substringCharacter == '>') performingRegularMarkup = false;
        if (substringCharacter == '{')
        {
            specialMarkupIndex.x = _index+1;
            performingSpecialMarkup = true;
        }

        if (substringCharacter == '}')
        {
            specialMarkupIndex.y = _index;
            if (performingSpecialMarkup)
            {
                var totalCommands = _fullTextContent.Substring(specialMarkupIndex.x, specialMarkupIndex.y - specialMarkupIndex.x).Trim(' ').Split(',');
                foreach (var command in totalCommands)
                {
                    var specialCommand = command.Trim(' ').Split('=');
                    switch (specialCommand[0])
                    {
                        case "col":
                            switch (specialCommand[1])
                            {
                                case "":
                                    currentTextContent += "<color=#ffffff>";
                                    break;
                                case "key":
                                    currentTextContent += "<color=#ffe04d>";
                                    break;
                                case "stat":
                                    currentTextContent += "<color=#ffad2f>";
                                    break;
                                case "err":
                                    currentTextContent += "<color=#ff1111>";
                                    break;
                            }
                            break;
                        case "spd":
                            switch (specialCommand[1])
                            {
                                case "stat":
                                    currentTextTypeDelay = 0.01f;
                                    break;
                                default:
                                    float.TryParse(specialCommand[1], out currentTextTypeDelay);
                                    break;
                            }
                            break;
                        case "por":
                    
                            break;
                    }
                }
            }
            performingSpecialMarkup = false;
            outIndexResult += 1;
        }
        _outIndex = outIndexResult;
    }

    private bool MoveNext()
    {
        if (currentFrame < currentTextEvent.frames.Count-1)
        {
            currentFrame++;
            return true;
        }
        
        textEventActive = false;
        Destroy(textbox.gameObject);
        currentTextEvent.OnFinish.Invoke();
        Clear();
        return false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public bool TryStartTextEvent(TextEvent _textEvent, bool _overrideExistingEvents = false)
    {
        if (textEventActive is false || _overrideExistingEvents)
        {   
            Clear();
            currentTextEvent = _textEvent;
            StartTextEvent();
            return true;
        }
        
        // Failed to start, an event was already running
        return false;
    }

    public void Clear()
    {
        StopAllCoroutines();
        textEventActive = false;
        performingRegularMarkup = false;
        performingSpecialMarkup = false;
        currentTextContent = "";
        currentTextEvent = null;
        currentTextTypeDelay = normalTextTypeDelay;
    }

    #endregion
}

[Serializable]
public class TextFrames
{
    public string name;
    [TextArea] public string chatContent;
    public Sprite portrait;
    public UnityEvent OnFrameCompleted = new UnityEvent();
    [Header("Frame Settings")] 
    public TextboxDisplayMode displayMode;
    public bool preventTextSkipping;
    public bool preventTextContinuing;
    public bool autoProgressOnComplete;
}

[Serializable]
public class TextEvent
{
    public List<TextFrames> frames;
    public UnityEvent OnFinish = new UnityEvent();
}

[Serializable]
public enum TextboxDisplayMode
{
    monologue,
    dialogue,
    shopMono,
    shopDia,
}
