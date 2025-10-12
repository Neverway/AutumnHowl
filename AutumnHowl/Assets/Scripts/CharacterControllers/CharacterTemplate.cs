using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Character Template", fileName = "char_template_")]
public class CharacterTemplate : ScriptableObject
{
    public CharacterTemplateToIdentifierStrategy characterReferenceType;
    public string characterName;

    [Space, Unbox] public CharacterStats baseStats = new CharacterStats();
}

public enum CharacterTemplateToIdentifierStrategy
{
    UniqueAndPersistent,
    CloneableAndDisposable
}

/// <summary>
/// The constructed object of this class is supposed to serve as total-game-consistent way to identify characters from each other
/// <br/> - For UniqueAndPersistent characters: This identifier is stored and used consistently between new copies of that character, 
/// including switching between scenes (good for players, unique NPCs)
/// <br/> - For CloneableAndDisposable characters: This identifier is newly instantiated for each new instance of a character, 
/// which will not persist between scenes (good for spawnable enemies)
/// </summary>
public class CharacterIdentifier 
{
    public CharacterTemplate TemplateCreatedFrom { get; private set; }
    public CharacterStats Stats { get; private set; }

    private static Dictionary<CharacterTemplate, CharacterIdentifier> persistentCharacters = new();

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

    public static CharacterIdentifier GetDefaultCharacter() => GetFromCharacterTemplate(null);
    public static CharacterIdentifier GetFromCharacterTemplate(CharacterTemplate characterTemplate)
    {
        //Create empty identifier as default if no template is provided (used for GetDefaultCharacter()
        if (characterTemplate == null)
        {
            Debug.LogWarning("Creating a CharacterIdentifier without a template. Using a default character identifier in its place. " +
                "It will have default stats and not be persistent. May cause other errors");
            return new CharacterIdentifier(null);
        }

        CharacterIdentifier toReturn;
        switch (characterTemplate.characterReferenceType)
        {
            case CharacterTemplateToIdentifierStrategy.UniqueAndPersistent:
                if (!persistentCharacters.TryGetValue(characterTemplate, out toReturn))
                {
                    toReturn = new CharacterIdentifier(characterTemplate);
                    persistentCharacters.Add(characterTemplate, toReturn);
                }
                break;
            case CharacterTemplateToIdentifierStrategy.CloneableAndDisposable:
                toReturn = new CharacterIdentifier(characterTemplate);
                break;
            default:
                throw new System.NotImplementedException("Unimplemented CharacterIdentifier.ReferenceType in constructor");
        }
        return toReturn;
    }

    public override string ToString()
    {
        if (TemplateCreatedFrom == null)
            return "Default Character";
        if (string.IsNullOrWhiteSpace(TemplateCreatedFrom.name))
            return "Unnamed Character";

        return TemplateCreatedFrom.name;
    }
}