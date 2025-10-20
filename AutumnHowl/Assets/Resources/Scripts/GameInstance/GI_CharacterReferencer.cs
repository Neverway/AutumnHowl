using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GI_CharacterReferencer : MonoBehaviour
{
    private List<Character> activeCharacterComponents = new List<Character>();
    private List<CharacterIdentifier> activeCharacterIdentifiers = new List<CharacterIdentifier>();

    public void Register(Character character) => activeCharacterComponents.Add(character);
    public void UnRegister(Character character) => activeCharacterComponents.Remove(character);
    public void Register(CharacterIdentifier identifier) => activeCharacterIdentifiers.Add(identifier);
    public void UnRegister(CharacterIdentifier identifier) => activeCharacterIdentifiers.Remove(identifier);
}

