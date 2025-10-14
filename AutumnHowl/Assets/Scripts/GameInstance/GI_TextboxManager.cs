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
using System.Data.SqlTypes;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class GI_TextboxManager : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public float normalTextTypeDelay, skippingTextTypeDelay;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [Box] public TextEvent currentTextEvent;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool textEventActive;
    private bool currentlyPrinting;
    private string currentTextContent;
    private float currentTextTypeDelay;
    private int currentFrame;
    private bool performingRegularMarkup, performingSpecialMarkup;
    private int markupStartIndex;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    private GI_WidgetManager widgetManager;
    private WB_Textbox textbox;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/

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
                if (GameInstance.Inputs.Action.WasPressedThisFrame()) currentTextTypeDelay = skippingTextTypeDelay;
                if (GameInstance.Inputs.Action.WasReleasedThisFrame()) currentTextTypeDelay = normalTextTypeDelay;
            }
            
            // Handel move next frame inputs
            if (currentEventFrame.preventTextContinuing is false)
            {
                if (GameInstance.Inputs.Interact.WasPressedThisFrame() && currentlyPrinting is false)
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
            // Check for Special { } markups and skip if inside of one
            if (CheckForSpecialMarkups(_fullTextContent, i))
                continue;

            //Check for Regular < > markups and skip if inside of one
            if (CheckForRegularMarkups(_fullTextContent, i))
                continue;

            //If there are no markups, add current character to text content and wait for text delay
            currentTextContent += _fullTextContent[i];
            yield return new WaitForSeconds(currentTextTypeDelay);
        }
        currentlyPrinting = false;

        _onFrameCompleted.Invoke();
        
        if (currentTextEvent.frames[currentFrame].autoProgressOnComplete) PrintNextFrame();
    }

    /// <returns>True if currently inside a special markup</returns>
    private bool CheckForSpecialMarkups(string _fullTextContent, int _index)
    {
        //Don't check for special markups if you're checking for regular markups
        if (performingRegularMarkup) return false;

        //If not in regular markup, check if this is the start of one, and exit function
        if (!performingSpecialMarkup)
        {
            if (_fullTextContent[_index] == '{')
            {
                markupStartIndex = _index;
                performingSpecialMarkup = true;
                return true;
            }
            return false;
        }

        //If this is the end of the markup, finish the markup and process it
        if (_fullTextContent[_index] == '}')
        {
            string fullMarkup = _fullTextContent.Substring(markupStartIndex, _index - markupStartIndex + 1);

            //Remove certain characters from the markup to make processing it easier and flexible to use
            string[] charsToRemove = { "{", "}", " " };
            foreach (var charToRemove in charsToRemove) 
                fullMarkup = fullMarkup.Replace(charToRemove, "");

            //Get all special markup commands sorted by commas, and process each one
            string[] allCommands = fullMarkup.Split(',');
            foreach (string command in allCommands)
            {
                //Split command by an "=" where the left side is the command name, and the right is the command value
                string[] commandParts = command.Split('=');
                string commandName = commandParts[0];
                string commandValue = commandParts[1];

                switch (commandName)
                {
                    case "col":
                        switch (commandValue)
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
                        switch (commandValue)
                        {
                            case "":
                                currentTextTypeDelay = normalTextTypeDelay;
                                break;
                            case "stat":
                                currentTextTypeDelay = 0.01f;
                                break;
                            default:
                                float.TryParse(commandParts[1], out currentTextTypeDelay);
                                break;
                        }
                        break;
                    case "por":

                        break;
                }
            }

            //Finish this markup
            performingSpecialMarkup = false;
        }

        return true;
    }
    
    /// <returns>True if currently inside a regular markup</returns>
    private bool CheckForRegularMarkups(string _fullTextContent, int _index)
    {
        //Don't check for regular markups if you're checking for special markups
        if (performingSpecialMarkup) return false;

        //If not in regular markup, check if this is the start of one, and exit function
        if (!performingRegularMarkup)
        {
            if (_fullTextContent[_index] == '<')
            {
                markupStartIndex = _index;
                performingRegularMarkup = true;
                return true;
            }
            return false;
        }

        //If this is the end of the markup, finish the markup and process it
        if (_fullTextContent[_index] == '>')
        {
            var fullMarkup = _fullTextContent.Substring(markupStartIndex, _index - markupStartIndex + 1);
            //Add full markup to current text content (essentially skips waiting for each character)
            currentTextContent += fullMarkup;

            //Finish this markup
            performingRegularMarkup = false;
        }

        return true;
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
    [Box] public List<TextFrames> frames;
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
