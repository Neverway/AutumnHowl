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
using UnityEngine;

public class WB_Inventory : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private WidgetNavigator ToNavigateToOnMenuOpen;
    [Space]
    [SerializeField] private WidgetNavigator ItemListNavigator;
    [SerializeField] private WidgetNavigator SpellListNavigator;
    [SerializeField] private WidgetNavigator GearListNavigator;
    [SerializeField] private WidgetNavigator inspectListNavigator;
    [SerializeField] private Func_TextEvent inspectTextEvent;
    [SerializeField] private AudioSource audioSource;
    private GI_AuHoGameState gameState;
    [SerializeField] private AudioClip bookOpen, bookClose;

    public WidgetNavigator[] allNavigators => new[] { ItemListNavigator, SpellListNavigator, GearListNavigator, inspectListNavigator, ToNavigateToOnMenuOpen };
    public WidgetNavigator[] allListNavigators => new[] { ItemListNavigator, SpellListNavigator, GearListNavigator };

    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameState = GameInstance.Get<GI_AuHoGameState>();

        foreach(var navigator in allListNavigators)
        {
            for (int i = 0; i < navigator.selectableElements.Count; i++)
            {
                var selectable = navigator.selectableElements[i];
                var cachedIndex = i;
                selectable.OnInteracted.AddListener(() => { SetupInspectMenu(navigator, cachedIndex);});
            }
        }
    }

    private void OnDisable()
    {
        // Move to navigating InspectMenu
        SetNavigationTo(ToNavigateToOnMenuOpen);
        StopAllCoroutines();
    }

    private int WidgetToItemListID(WidgetNavigator navigator)
    {
        if (navigator == ItemListNavigator) return 0;
        if (navigator == SpellListNavigator) return 1;
        if (navigator == GearListNavigator) return 2;
        throw new NotImplementedException();
    }
    private void SetNavigationTo(WidgetNavigator navigatorToEnable)
    {
        foreach (var navigator in allNavigators)
        {
            navigator.GetComponent<Text_Inventory>()?.UpdateItemList();

            //Set navigation to given navigator (but only if there is a change in navigation state)
            bool shouldBeNavigating = navigator == navigatorToEnable;
            if (shouldBeNavigating ^ navigator.activelyNavigating)
                navigator.SetIsNavigating(shouldBeNavigating);

        }
        //Enable/Disable the Inspect Menu
        inspectListNavigator.gameObject.SetActive(navigatorToEnable == inspectListNavigator);
    }
    private IEnumerator CoSetNavigationTo(WidgetNavigator navigatorToEnable)
    {
        SetNavigationTo(null);
        while (GameInstance.Get<GI_TextboxManager>().textEventActive)
        {
            yield return null;
        }
        SetNavigationTo(navigatorToEnable);
    }
    private void DisplayTextEvent(string textToDisplay)
    {
        inspectTextEvent.textEvent.frames[0].chatContent = textToDisplay;
        inspectTextEvent.CallEvent();
    }

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Allows each entry to create the inspect menu and updates the inspect menu to perform action on the currently selected item
    /// This function is bound to each item entry in the item and spell lists
    /// </summary>
    /// <param name="_parentNavigator"></param>
    /// <param name="_index">The index of the entry in the item list that this function is bound to</param>
    private void SetupInspectMenu(WidgetNavigator _parentNavigator, int _index)
    {
        // Move to navigating InspectMenu
        SetNavigationTo(inspectListNavigator);

        // Set the buttons in the inspect menu for the current item
        for (int i = 0; i < inspectListNavigator.selectableElements.Count; i++)
        {
            inspectListNavigator.selectableElements[i].OnInteracted.RemoveAllListeners();
        }
        inspectListNavigator.selectableElements[0].OnInteracted.AddListener(() => Inspect(_parentNavigator, _index));
        inspectListNavigator.selectableElements[1].OnInteracted.AddListener(() => Use(_parentNavigator, _index));
        inspectListNavigator.selectableElements[2].OnInteracted.AddListener(() => Discard(_parentNavigator, _index));

        //Set the text for the "Use" button to be relative to what item you're trying to use
        string useItemText = "Use";
        if (_parentNavigator == ItemListNavigator)
        {
            Item item = GameInstance.Gamestate.inventory.GetItem(_index, 0);
            if (item != null && item is Item_Wearable)
                useItemText = "Equip";
        }
        if (_parentNavigator == SpellListNavigator) useItemText = "Cast";
        if (_parentNavigator == GearListNavigator) useItemText = "Unequip";
        ((WidgetSelectable_TMPText)inspectListNavigator.selectableElements[1]).SetText(useItemText);

        // Set the inspect menu to reactive parent nav on back
        inspectListNavigator.OnBack.RemoveAllListeners();
        inspectListNavigator.OnBack.AddListener(() => SetNavigationTo(_parentNavigator));
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Prints the selected item's description (or falls back to some default text if it's null item)
    /// </summary>
    /// <param name="_itemList">The name of the item list we want to check (Items or Spells)</param>
    /// <param name="_index">The index of the item we want to get</param>
    public void Inspect(WidgetNavigator _parentWidget, int _index)
    {
        // Get selected item list and item
        int itemList = WidgetToItemListID(_parentWidget);
        Item itemAtIndex = gameState.currentGameState.inventory.GetItem(_index, itemList);

        //Set chat content for selected index (use default text if there is no item there)
        DisplayTextEvent(itemAtIndex ? itemAtIndex.GetDescription() : "*You inspected nothing.{spd=0.2} {spd=}It's quite captivating.");

        //Switch navigation to parent widget
        StartCoroutine(CoSetNavigationTo(_parentWidget));
    }

    /// <summary>
    /// Trys to use the selected item (or falls back to some default text if it's null item)
    /// </summary>
    /// <param name="_itemList">The name of the item list we want to check (Items or Spells)</param>
    /// <param name="_index">The index of the item we want to get</param>
    public void Use(WidgetNavigator _parentWidget, int _index)
    {
        // Get selected item list and item
        int itemList = WidgetToItemListID(_parentWidget);
        Item itemAtIndex = GameInstance.Gamestate.inventory.GetItem(_index, itemList);
        
        if (itemAtIndex != null)
        {
            if (!GameInstance.Gamestate.inventory.TryUseItem(_index, GameInstance.Gamestate.player, itemList))
                DisplayTextEvent("*Not able to use that");
        }
        else
        {
            // display default "can't use" text
            DisplayTextEvent("*You attempt to use nothing.{col=key,spd=0.2} {spd=}It did a trick!{col=,spd=1} {spd=}Never mind, it did nothing.");
        }

        //Switch navigation to parent widget
        StartCoroutine(CoSetNavigationTo(_parentWidget));
    }

    /// <summary>
    /// Trys to discard the selected item (or falls back to some default text if it's null item)
    /// </summary>
    /// <param name="_itemList">The name of the item list we want to check (Items or Spells)</param>
    /// <param name="_index">The index of the item we want to get</param>
    public void Discard(WidgetNavigator _parentWidget, int _index)
    {
        // Get selected item list and item
        int itemList = WidgetToItemListID(_parentWidget);
        Item itemAtIndex = gameState.currentGameState.inventory.GetItem(_index, itemList);
        
        // If there is an item at the selected index
        if (itemAtIndex != null)
        {
            // Try to discard it
            if (GameInstance.Gamestate.inventory.TryRemoveItem(_index, itemList))
            {

            }
            else
            {
                // Item can't be discarded because it's a key item (Or because... IT DISAPPEARED BETWEEN THE FIRST CHECK AND NOW??? HOW???!!)
                if (itemAtIndex == null)
                    DisplayTextEvent("*You probably shouldn't discard...{spd=0.5} {spd=}wait...{spd=0.5} {spd=}there is nothing here?!");
                else
                    DisplayTextEvent("*You probably shouldn't discard this item.");
            }   
        }
        // If there is NOT an item at the selected index
        else
        {
            // Assign and show the textbox with the default "can't discard" text
            DisplayTextEvent("*You attempted to discard nothing,{spd=0.5} {spd=}but there is still nothing here.{spd=0.5} {spd=}Did you succeed?");
        }

        //Switch navigation to parent widget
        StartCoroutine(CoSetNavigationTo(_parentWidget));
    }

    public void CloseMenu()
    {
        
    }

    public void AddSkillPoint(int _skillPointStat)
    {
        if (GameInstance.Playerbody.Stats.unassignedSkillPoints <= 0) return;
        
        if (_skillPointStat == 0) GameInstance.Playerbody.Stats.healthSP += 1;
        if (_skillPointStat == 1) GameInstance.Playerbody.Stats.powerSP += 1;
        if (_skillPointStat == 2) GameInstance.Playerbody.Stats.corruptionSP += 1;
        if (_skillPointStat == 3) GameInstance.Playerbody.Stats.attackSP += 1;
        if (_skillPointStat == 4) GameInstance.Playerbody.Stats.defenseSP += 1;

        GameInstance.Playerbody.Stats.unassignedSkillPoints -= 1;
        
        GameInstance.Playerbody.Stats.UpdateSkillPointModifiers();
    }
    
    public void RemoveSkillPoint(int _skillPointStat)
    {
        if (_skillPointStat == 0 && GameInstance.Playerbody.Stats.healthSP > 0)
        {
            GameInstance.Playerbody.Stats.healthSP -= 1;
            GameInstance.Playerbody.Stats.unassignedSkillPoints += 1;
        }

        if (_skillPointStat == 1 && GameInstance.Playerbody.Stats.powerSP > 0)
        {
            GameInstance.Playerbody.Stats.powerSP -= 1;
            GameInstance.Playerbody.Stats.unassignedSkillPoints += 1;
        }

        if (_skillPointStat == 2 && GameInstance.Playerbody.Stats.corruptionSP > 0)
        {
            GameInstance.Playerbody.Stats.corruptionSP -= 1;
            GameInstance.Playerbody.Stats.unassignedSkillPoints += 1;
        }

        if (_skillPointStat == 3 && GameInstance.Playerbody.Stats.attackSP > 0)
        {
            GameInstance.Playerbody.Stats.attackSP -= 1;
            GameInstance.Playerbody.Stats.unassignedSkillPoints += 1;
        }

        if (_skillPointStat == 4 && GameInstance.Playerbody.Stats.defenseSP > 0)
        {
            GameInstance.Playerbody.Stats.defenseSP -= 1;
            GameInstance.Playerbody.Stats.unassignedSkillPoints += 1;
        }
        
        GameInstance.Playerbody.Stats.UpdateSkillPointModifiers();
    }


    #endregion
}
