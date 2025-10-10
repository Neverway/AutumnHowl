using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "AuHo/Character Template", fileName = "char_template_")]
public class CharacterTemplate : ScriptableObject
{
    public CharacterIdentifier.ReferenceType characterReferenceType;

    [Box] public CharacterStats baseStats = new CharacterStats();
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
    private static Dictionary<CharacterTemplate, CharacterIdentifier> persistentCharacters = new();

    public enum ReferenceType
    {
        UniqueAndPersistent,
        CloneableAndDisposable
    }

    //private 

    public static CharacterIdentifier Get(Character character)
    {
        if (character == null) throw new NullReferenceException("Why on gods green earth are you sending null " +
            "characters to get null identifiers? is it because your IQ is null?");

        var type = character.template.characterReferenceType;
        if (type == ReferenceType.UniqueAndPersistent)
        {
            if (persistentCharacters.TryGetValue(character.template, out CharacterIdentifier identifier))
                return identifier;
            persistentCharacters.Add(character.template, new CharacterIdentifier());
        }
        else if (type == ReferenceType.CloneableAndDisposable)
        {
            return new CharacterIdentifier();
        }
        throw new System.NotImplementedException("Unimplemented CharacterIdentifier.ReferenceType in constructor");
    }
}