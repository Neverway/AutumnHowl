using ErryLib.ModiferSystem.Instancers;
using System.Collections.Generic;
using UnityEngine;
public class AuHo_DebugActiveModifiers : MonoBehaviour
{
    [DebugDisplayListAsStrings(nameof(activeModifiers), true)]
    public string dummy;

    private string[] activeModifiers;

    public void Update()
    {
        List<string> getStrings = new List<string>();
        foreach (Modifier mod in Modifier.ActiveModifiers)
        {
            if (mod is IDescribable describable)
                getStrings.Add(describable.Description);
            else if (mod is InstancedModifier<CharacterTargets> modifier)
                getStrings.Add(modifier.GetDebugDescription());
            else
                getStrings.Add("??? : " + mod.ToString());
        }
        activeModifiers = getStrings.ToArray();
    }
}
public static partial class AuHo_ExtentionMethods
{
    public static string GetDebugDescription(this InstancedModifier<CharacterTargets> modifier)
    {
        string resultingString = "";
        if (modifier.ModifierData is CharacterTargetsFromUser userTargets)
        {
            resultingString += $"{userTargets.User} targeting {userTargets.TargetType}: ";
        }
        else if (modifier.ModifierData is CharacterTargetsFromFunc funcTargets)
        {
            resultingString += "Func targets : ";
        }
        else
            resultingString += "Unknown targets : ";

        if (modifier.Instancer is SerializedModifier serializedModInstancer)
            resultingString += $"[{serializedModInstancer.Description}]";
        else
            resultingString += "???";

        return resultingString;
    }
}