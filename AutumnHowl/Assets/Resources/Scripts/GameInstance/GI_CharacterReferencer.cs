using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GI_CharacterReferencer : MonoBehaviour
{
    public List<Character> activeCharacterComponents = new List<Character>();

    public void Register(Character character) => activeCharacterComponents.Add(character);
    public void UnRegister(Character character) => activeCharacterComponents.Remove(character);
}

