using System;
using System.Collections;
using UnityEngine;
using static CharacterStats;

public class AuHo_Modifiers { }

public interface UserTargetedModifier : IDescribable
{
    public CharacterTargets targets { get; set; }
    public void RegisterModifier(CharacterTargets targets)
    {
        this.targets = targets;
        PassModifier().RegisterModifier(multiRegister: true);
    }
    public void UnRegisterModifier() => PassModifier().UnregisterModifier();
    protected Modifier PassModifier();
}

public interface IDescribable
{
    public string Description { get; }
}
[Serializable]
public class TimedModifier : Modifier, IDescribable
{
    public bool hideDescription;
    public float seconds;
    [Box, Polymorphic, SerializeReference] public Modifier modifier;
    public string Description
    {
        get
        {
            if (hideDescription || seconds <= 0) return "";

            if (modifier != null && modifier is IDescribable describable)
            {
                string description = describable.Description;
                if (string.IsNullOrEmpty(description))
                    return "";

                int inMinutes = Mathf.FloorToInt(seconds / 60);
                int inSeconds = Mathf.FloorToInt(seconds % 60);
                string XXm = inMinutes > 0 ? $"{inMinutes}m" : "";
                string XXs = inSeconds > 0 ? $"{inSeconds}s" : "";
                return $"{describable.Description} for {XXm}{XXs}";
            }
            return "";
        }
    }

    protected override void OnRegisterModifier()
    {
        modifier.RegisterModifier();
        GameInstance.SendCoroutine(RemoveModifierAfterTime());
    }
    public override void ModifyValue(Modifiable modifiableValue) { }

    public IEnumerator RemoveModifierAfterTime()
    {
        yield return new WaitForSeconds(seconds);
        modifier.UnregisterModifier();
        UnregisterModifier();
    }
}

[Serializable]
public class CharacterStatModifier : NumberModifier<CharacterStat>, UserTargetedModifier
{
    public bool hideDescription = false;
    public StatType statToModify;
    public string Description
    {
        get
        {
            if (hideDescription)
                return "";

            string stat = statToModify.GetStatName();
            if (stat == null)
                return "Changes stat?";

            if (modifierType == ModifierType.Add)
                return $"+{Mathf.RoundToInt(value)} {statToModify.GetStatName()}";
            else if (modifierType == ModifierType.Multiply)
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

    public CharacterTargets targets { get; set; }

    public override void ModifyValue(Modifiable modifiableValue)
    {
        //Only modify CharacterStats
        if (modifiableValue is not CharacterStat characterStat) return;
        //Only modify stats of specified type
        if (!characterStat.IsStat(statToModify)) return;
        //Only modify stats on targeted characters
        if (targets.IsTargeted(characterStat.linkedCharacter))

        base.ModifyValue(modifiableValue);
    }

    Modifier UserTargetedModifier.PassModifier() => this;
}