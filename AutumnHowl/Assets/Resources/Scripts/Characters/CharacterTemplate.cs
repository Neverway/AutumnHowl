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

    [Space, Unbox] public CharacterStats baseStats = new CharacterStats();

    public void OnValidate() => baseStats.RefreshStatIDs();

}

public enum CharacterTemplateToIdentifierStrategy
{
    UniqueAndPersistent,
    CloneableAndDisposable
}