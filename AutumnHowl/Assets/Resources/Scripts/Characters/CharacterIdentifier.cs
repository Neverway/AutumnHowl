using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The constructed object of this class is supposed to serve as total-game-consistent way to identify characters from each other
/// <br/> - For UniqueAndPersistent characters: This identifier is stored and used consistently between new copies of that character, 
/// including switching between scenes (good for players, unique NPCs)
/// <br/> - For CloneableAndDisposable characters: This identifier is newly instantiated for each new instance of a character, 
/// which will not persist between scenes (good for spawnable enemies)
/// </summary>
public class CharacterIdentifier
{
    //Fields -----------------------------------------------------------------------------------------------------------------
    public CharacterTemplate TemplateCreatedFrom { get; private set; }
    public CharacterStats Stats { get; private set; }


    //Methods ----------------------------------------------------------------------------------------------------------------
    
    /// <summary>Called upon a creation of a NEW instance of a character</summary>
    public void OnNewCharacter()
    {
        Stats.OnNewCharacter();
    }

    //Character Identifier creation ------------------------------------------------------------------------------------------
    [Reload]
    public static SerializableDictionary<CharacterTemplate, CharacterIdentifier> persistentCharacters;

    public CharacterIdentifier(CharacterTemplate fromTemplate)
    {
        if (fromTemplate != null)
        {
            TemplateCreatedFrom = fromTemplate;
            Stats = new CharacterStats(this);
        }
        else
            Stats = new CharacterStats(this);
    }

    public static CharacterIdentifier NewDummyCharacter => GetFromCharacterTemplate(null);
    public static CharacterIdentifier GetFromCharacterTemplate(CharacterTemplate characterTemplate)
    {
        //Create empty identifier as default if no template is provided (used for GetDefaultCharacter()
        if (characterTemplate == null)
            return new CharacterIdentifier(null);

        if (persistentCharacters == null)
            persistentCharacters = new();

        CharacterIdentifier toReturn;
        switch (characterTemplate.characterReferenceType)
        {
            case CharacterTemplateToIdentifierStrategy.UniqueAndPersistent:
                if (!persistentCharacters.TryGetValue(characterTemplate, out toReturn))
                {
                    toReturn = new CharacterIdentifier(characterTemplate);
                    toReturn.OnNewCharacter();
                    persistentCharacters.Add(characterTemplate, toReturn);
                }
                break;
            case CharacterTemplateToIdentifierStrategy.CloneableAndDisposable:
                toReturn = new CharacterIdentifier(characterTemplate);
                toReturn.OnNewCharacter();
                break;
            default:
                {
                    Debug.LogError("Unimplemented CharacterTemplateToIdentifierStrategy in constructor");
                    throw new NotImplementedException("Unimplemented CharacterTemplateToIdentifierStrategy in constructor");
                }
        }
        return toReturn;


    }

    

    //Save and Load for Persistent Characters ----------------------------------------------------------------------------------------

    [InvokeBeforeSave]
    public static void OnSave()
    {
        Debug.Log("OnSave");
        List<SaveData> saveDatas = new();

        foreach (var character in persistentCharacters.Values)
            saveDatas.Add(character.GetSaveData());

        var toSave = new Wrapper<SaveData[]>(saveDatas.ToArray());
        GI_SaveSystem.SaveValue(toSave, "PersistentCharacterData");
        Debug.Log($"{toSave.value.Length}");
    }
    
    [InvokeAfterLoad]
    public static void OnLoad()
    {
        Debug.Log("OnLoad");
        var toLoad = GI_SaveSystem.LoadValue<Wrapper<SaveData[]>>(null, "PersistentCharacterData");
        if (toLoad == null)
        {
            Debug.Log($"toLoad was null!");
            return;
        }

        var oldCharacters = persistentCharacters;
        persistentCharacters = new();

        Debug.Log($"{toLoad.value.Length}");
        foreach (SaveData data in toLoad.value)
        {
            Debug.Log($"OnLoad - Character {data.templateID}");
            if (IDToObj<CharacterTemplate>.TryGet(data.templateID, out CharacterTemplate template))
            {
                CharacterIdentifier charToLoad = 
                    oldCharacters.ContainsKey(template) ? oldCharacters[template] : new(template);

                charToLoad.LoadSaveData(data);
                persistentCharacters.Add(template, charToLoad);
            }
            else
                Debug.LogError($"Invalid templateID save in persistent characters? ID: {data.templateID}");
        }

    }
    
    
    [Serializable]
    public struct SaveData
    {
        public string templateID;
        public CharacterStats.SaveData statsData;
    }
    public SaveData GetSaveData() => new SaveData()
    {
        templateID = TemplateCreatedFrom.UniqueID,
        statsData = Stats.GetSaveData()
    };
    public void LoadSaveData(SaveData saveData)
    {
        Stats.SetupStatsLinkedToCharacter(this);
        Stats.LoadSaveData(saveData.statsData);
    }


    //Basic C# class features ---------------------------------------------------------------------------------------------------------
    public override string ToString()
    {
        if (TemplateCreatedFrom == null)
            return "Default Character";
        if (string.IsNullOrWhiteSpace(TemplateCreatedFrom.name))
            return "Unnamed Character";

        return TemplateCreatedFrom.name;
    }
}