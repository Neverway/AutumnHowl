using ErryLib.ModiferSystem.Instancers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuHo_DebugCharacterInfo : MonoBehaviour
{
    [DebugDisplayListAsStrings(nameof(persistentCharacters), true)]
    public string dummy;

    private string[] persistentCharacters;

    public void Update()
    {
        List<string> persistentCharactersStrings = new List<string>();
        foreach (CharacterIdentifier mod in CharacterIdentifier.persistentCharacters.Values)
        {
            persistentCharactersStrings.Add(
                $"TEMPLATE: {mod.TemplateCreatedFrom.characterName} (ID: {mod.TemplateCreatedFrom.UniqueID})\n" +
                $"STATS: " +
                $"  - {mod.Stats.health}/{mod.Stats.maxHealth}"
                );
        }
        persistentCharacters = persistentCharactersStrings.ToArray();
    }
}