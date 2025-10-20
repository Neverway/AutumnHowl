using ErryLib.ModiferSystem.Instancers;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

///=============================================== Shortcuts to Modifiers ====================================================
/// For Polymorphic Serialization:
///    <see cref="IStatModifierCreator"> for any modifiers you want to serialize that target chracters
/// 
/// 
///===========================================================================================================================
public static class AuHo_Modifiers { }




//------------------------------------------------
//       MODIFIER BASE TYPES AND INTERFACES
//------------------------------------------------
public interface SerializedModifier : IDescribable
{
    [Reload] public static Dictionary<object, List<Modifier>> idToRegisteredModifiers;

    protected void RecordRegisteredModifier(object id, Modifier modifier)
    {
        //Created applied modifiers dictionary if not already existing
        if (idToRegisteredModifiers == null) idToRegisteredModifiers = new Dictionary<object, List<Modifier>>();
        //Create new list of modifiers for provided id 
        if (!idToRegisteredModifiers.ContainsKey(id)) idToRegisteredModifiers.Add(id, new List<Modifier>());

        //Finally, add the modifier to the list of activated modifiers for that ID
        idToRegisteredModifiers[id].Add(modifier);
    }


    public Modifier GetNew_Flexible(CharacterTargets targets)
    {
        //Register modifiers for any modifier creators that have no input data
        if (this is IModifierInstancer modifierCreator)
            return modifierCreator.GetNewRegisteredModifier();

        //Register modifiers for any modifier creators that have CharacterTargets as input data
        if (this is IModifierInstancer<CharacterTargets> statModifierCreator)
            return statModifierCreator.GetNewRegisteredModifier(targets);

        Debug.LogError($"{nameof(SerializedModifier)}: Unimplemented type of ModifierInstancer was assigned. " +
            $"This is because {GetType()} does not implement IModifierInstancer or an IModifierInstancer<TData> where " +
            $"TData is an accounted-for type in the method this error comes from");
        throw new NotImplementedException();
    }

    public void RegisterTo_Flexible(object id, CharacterTargets targets)
    {
        //Get a new modifier based on given paramters
        Modifier createdModifier = GetNew_Flexible(targets);
        //Apply the modifier
        createdModifier.RegisterModifier();
        //Record it to dictionary for removal by ID later
        RecordRegisteredModifier(id, createdModifier);
    }

    public void UnregisterFrom(object id) => UnregisterModifierFrom(id);
    public static void UnregisterModifierFrom(object id)
    {
        if (idToRegisteredModifiers == null) idToRegisteredModifiers = new Dictionary<object, List<Modifier>> ();
        if (idToRegisteredModifiers.TryGetValue(id, out var modifiers))
        {
            foreach (var modifier in modifiers)
                modifier.UnregisterModifier();

            idToRegisteredModifiers.Remove(id);
        }
    }
}
public interface SerializedModifier_NoInput : IModifierInstancer, SerializedModifier
{
    public Modifier GetNew() => GetNewModifier();
    public void RegisterTo(object id)
    {
        Modifier createdModifier = GetNew();
        createdModifier.RegisterModifier();
        RecordRegisteredModifier(id, createdModifier);
    }
}
public interface SerializedModifier_CharacterTargeting : IModifierInstancer<CharacterTargets>, SerializedModifier
{
    public Modifier GetNew(CharacterTargets targets) => GetNewModifier(targets);
    public void RegisterTo(object id, CharacterTargets targets)
    {
        Modifier createdModifier = GetNew(targets);
        createdModifier.RegisterModifier();
        RecordRegisteredModifier(id, createdModifier);
    }
}

[Serializable]
public abstract class CharacterStatModifierCreator : SerializedModifier_CharacterTargeting
{
    public abstract string Description { get; }
    /// <summary>Passes any stat that need to be modified by the modifier</summary>
    public abstract void ModifyStat(CharacterStat stat);

    /// <summary>Filters the modifiers for ones that are a CharacterStat and of a character that is targeted by the provided CharacterTargets</summary>
    void IModifierInstancer<CharacterTargets>.OnInstanceModifyValue(Modifiable modifiableValue, CharacterTargets targets)
    {
        if (modifiableValue is CharacterStat charStat)
            if (targets.IsTargeted(charStat.LinkedCharacter))
                ModifyStat(charStat);
    }
    public override string ToString() => Description;
}
/// <summary>Used by some classes to define a description for the object</summary>
public interface IDescribable { public string Description { get; } }








//------------------------------------------------
//            MODIFIERS READY TO USE
//------------------------------------------------

[Serializable]
public class MultipleCharacterStatModifiers : CharacterStatModifierCreator
{
    public bool hideDescription;
    [Box, Polymorphic, SerializeReference] public CharacterStatModifierCreator[] statModifiers;

    //CharacterStatModInstancer implementation ---------------------------------------------------
    public override void ModifyStat(CharacterStat stat)
    {
        foreach (var modifierInstancer in statModifiers.NotNull())
            modifierInstancer.ModifyStat(stat);
    }

    //IDescribable implementation --------------------------------------------------------------
    public override string Description =>
        string.Join(", ",                             // Join below array of strings together into one string seperated by "] [" 
            statModifiers.NotNull()                        // Skip all null modifiers
            .Select((m) => m.Description)              // Get array of all descriptions from modifiers
            .Where((m) => !string.IsNullOrEmpty(m)));  // Trim all empty or null strings from array
}


[Serializable]
public class CharacterStatModifiers : CharacterStatModifierCreator
{
    public bool hideDescription = false;
    public CharacterStatType statToModify;
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
                return $"{(value > 0 ? "+" : "")}{Mathf.RoundToInt(value)} {statToModify.GetStatName()}";
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
public partial class AuHo_ExtentionMethods
{
    public static void ModifyStatWith(this CharacterStat stat, object id, NumberModifierType modifierType, float value)
    {
        SerializedModifier_CharacterTargeting statModInstancer = new CharacterStatModifiers()
        {
            statToModify = stat.StatType,
            modifierType = modifierType,
            value = value
        };
        statModInstancer.RegisterTo(id, new TargetSelf().GetTargetsFrom(stat.LinkedCharacter));
    }
    public static void UnmodifyStatWith(this CharacterStat stat, object id) =>
        SerializedModifier.UnregisterModifierFrom(id);
}







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