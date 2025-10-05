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
    private bool performingMarkup;


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
        currentTextTypeDelay = normalTextTypeDelay;
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
            print($"Textbox wasn't found setting to {textbox}");
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
            var substringCharacter = _fullTextContent[i];
            if (substringCharacter == '<') performingMarkup = true;
            if (substringCharacter == '>') performingMarkup = false;
            
            currentTextContent = _fullTextContent.Substring(0, i+1);
            if (!performingMarkup) yield return new WaitForSeconds(currentTextTypeDelay);
        }
        currentlyPrinting = false;

        _onFrameCompleted.Invoke();
        
        if (currentTextEvent.frames[currentFrame].autoProgressOnComplete) PrintNextFrame();
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
        currentTextEvent.OnFinish.AddListener(()=>print("OnFinished Invoked!"));
        currentTextEvent.OnFinish.Invoke();
        return false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public bool TryStartTextEvent(TextEvent _textEvent, bool _overrideExistingEvents = false)
    {
        if (textEventActive is false || _overrideExistingEvents)
        {   
            Reset();
            currentTextEvent = _textEvent;
            StartTextEvent();
            return true;
        }
        
        // Failed to start, an event was already running
        return false;
    }

    public void Reset()
    {
        textEventActive = false;
        performingMarkup = false;
        currentTextContent = "";
    }

    #endregion
}

[Serializable]
public class TextFrames
{
    public string name;
    [TextArea] public string chatContent;
    public Sprite portrait;
    public UnityEvent OnFrameCompleted;
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
    public UnityEvent OnFinish;
}

[Serializable]
public enum TextboxDisplayMode
{
    monologue,
    dialogue,
    shopMono,
    shopDia,
}
