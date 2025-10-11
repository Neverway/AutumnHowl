using ErryLib.ModiferSystem.Instancers;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static CharacterStats;

//------------------------------------------------
//       MODIFIER BASE TYPES AND INTERFACES
//------------------------------------------------
public interface ICharacterStatModInstancer : IModifierInstancer<CharacterTargets>, IDescribable
{
    public void ModifyStat(CharacterStat stat);
}
public abstract class CharacterStatModInstancer : ICharacterStatModInstancer
{
    public abstract string Description { get; }
    public abstract void ModifyStat(CharacterStat stat);

    void IModifierInstancer<CharacterTargets>.OnInstanceModifyValue(Modifiable modifiableValue, CharacterTargets targets)
    {
        if (modifiableValue is CharacterStat charStat)
            if (targets.IsTargeted(charStat.linkedCharacter))
                ModifyStat(charStat);
    }
}

public interface IDescribable { public string Description { get; } }



//------------------------------------------------
//            MODIFIERS READY TO USE
//------------------------------------------------
//===========================================================================================================================
[Serializable]
public class GroupedModifiers : CharacterStatModInstancer, IDescribable
{
    public bool hideDescription;
    [Box, Polymorphic, SerializeReference] public ICharacterStatModInstancer[] modifiers;

    //CharacterStatModInstancer implementation ---------------------------------------------------
    public override void ModifyStat(CharacterStat modifiableValue)
    {
        foreach(var modifierInstancer in modifiers.NotNull())
            modifierInstancer.ModifyStat(modifiableValue);
    }

    //IDescribable implementation --------------------------------------------------------------
    public override string Description => 
        string.Join(" ",                             // Join below array of strings together into one string seperated by " " 
            modifiers.NotNull()                        // Skip all null modifiers
            .Select((m) => m.Description)              // Get array of all descriptions from modifiers
            .Where((m) => !string.IsNullOrEmpty(m)));  // Trim all empty or null strings from array
}

//===========================================================================================================================
[Serializable]
public class CharacterStatModifier : CharacterStatModInstancer
{
    public bool hideDescription = false;
    public StatType statToModify;
    public NumberModifierType modifierType;
    public float value;

    //CharacterStatModInstancer implementation ---------------------------------------------------
    public override void ModifyStat(CharacterStat stat)
    {
        if (!stat.IsStatType(statToModify))
            return;

        if (modifierType == NumberModifierType.Add)
            stat.OnModify_AddNumber(value);

        if (modifierType == NumberModifierType.Multiply)
            stat.OnModify_MultiplyNumber(value);
    }

    //IDescribable implementation --------------------------------------------------------------
    public override string Description
    {
        get
        {
            if (hideDescription)
                return "";

            string stat = statToModify.GetStatName();
            if (stat == null)
                return "Changes stat?";

            if (modifierType == NumberModifierType.Add)
                return $"+{Mathf.RoundToInt(value)} {statToModify.GetStatName()}";
            else if (modifierType == NumberModifierType.Multiply)
            {
                //return $"x{value.ToString("0.0")} {stat}";
                int percents = Mathf.RoundToInt((value - 1f) * 100f);
                if (percents == 0f)
                    return "";

                return $"{(percents > 0 ? "+" : "")}{percents}% {stat}";
            }
            else
                return $"Changes {stat}?";
        }
    }

}
//===========================================================================================================================



//------------------------------------------------
//            Deprecated Modifiers
//------------------------------------------------

/*
[Serializable]
public class TimedModifier : CharacterStatModInstancer, IDescribable
{
    public bool hideDescription;
    public float seconds;
    [Box, Polymorphic, SerializeReference] public ICharacterStatModInstancer modifier;

    private IEnumerator RemoveModifierAfterTime(Modifier modifier)
    {
        yield return new WaitForSeconds(seconds);
        modifier.UnregisterModifier();
    }

    //CharacterStatModInstancer implementation ---------------------------------------------------
    protected virtual void OnInstanceRegistered(InstancedModifier<CharacterTargets> instancedModifier) =>
        GameInstance.SendCoroutine(RemoveModifierAfterTime(instancedModifier));

    public override void ModifyStat(CharacterStat modifiableValue) => modifier.ModifyStat(modifiableValue);

    //IDescribable implementation --------------------------------------------------------------
    public override string Description
    {
        get
        {
            if (hideDescription || seconds <= 0) return "";

            if (modifier != null)
            {
                string description = modifier.Description;
                if (string.IsNullOrEmpty(description))
                    return "";

                int inMinutes = Mathf.FloorToInt(seconds / 60);
                int inSeconds = Mathf.FloorToInt(seconds % 60);
                string XXm = inMinutes > 0 ? $"{inMinutes}m" : "";
                string XXs = inSeconds > 0 ? $"{inSeconds}s" : "";
                return $"{modifier.Description} for {XXm}{XXs}";
            }
            return "";
        }
    }
}
// */