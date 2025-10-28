using ErryLib.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "AuHo/Character Template", fileName = "char_template_")]
public class CharacterTemplate : ScriptableObject, UniquelyIdentifiable
{
    [field: SerializeField] public string UniqueID { get; private set; }
    [InvokeOnReflectionCacheLoadRuntime] public static void CacheIDs() => IDToObj<CharacterTemplate>.AddAllFromUnityResources();

    [Space]
    public CharacterTemplateToIdentifierStrategy characterReferenceType = CharacterTemplateToIdentifierStrategy.CloneableAndDisposable;
    public string characterName;
    public CharacterTags[] characterTags;

    [Space, Unbox] public CharacterStats baseStats = new CharacterStats();

#if UNITY_EDITOR
    public void OnValidate() => baseStats.RefreshStatIDs();
#endif

}

public enum CharacterTemplateToIdentifierStrategy
{
    UniqueAndPersistent,
    CloneableAndDisposable
}
public enum CharacterTags //Dont change the values if you can, it will change which values are assigned in inspectors
{
    Ally = 0,
    Enemy = 1,
    Boss = 2,
    Obstacle = 3,
    Attack = 4,
    NPC = 5,

    Beast = 100,
    Undead = 101,
    Pumpkin = 102
}