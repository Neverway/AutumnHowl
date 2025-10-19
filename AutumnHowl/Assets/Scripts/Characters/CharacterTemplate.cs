using ErryLib.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Character Template", fileName = "char_template_")]
public class CharacterTemplate : ScriptableObject
{
    public string uniqueTemplateID;
    [Space]
    public CharacterTemplateToIdentifierStrategy characterReferenceType;
    public string characterName;

    [Space, Unbox] public CharacterStats baseStats = new CharacterStats();


    public static Dictionary<string, CharacterTemplate> Instances;
    [InvokeOnReflectionCacheLoad]
    private void GetCharacterTemplateInstances()
    {
        Instances = new();
        foreach (CharacterTemplate template in Resources.LoadAll<CharacterTemplate>(""))
            Instances.Add(template.uniqueTemplateID, template);
    }
}

public enum CharacterTemplateToIdentifierStrategy
{
    UniqueAndPersistent,
    CloneableAndDisposable
}