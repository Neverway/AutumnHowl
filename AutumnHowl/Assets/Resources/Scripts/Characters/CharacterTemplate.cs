using ErryLib.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Character Template", fileName = "char_template_")]
public class CharacterTemplate : ScriptableObject, UniquelyIdentifiable
{
    [field: SerializeField] public string UniqueID { get; private set; }
    [InvokeOnReflectionCacheLoadRuntime] public static void CacheIDs() => IDToObj<CharacterTemplate>.AddAllFromUnityResources();

    [Space]
    public CharacterTemplateToIdentifierStrategy characterReferenceType = CharacterTemplateToIdentifierStrategy.CloneableAndDisposable;
    public string characterName;

    [Space, Unbox] public CharacterStats baseStats = new CharacterStats();

}

public enum CharacterTemplateToIdentifierStrategy
{
    UniqueAndPersistent,
    CloneableAndDisposable
}