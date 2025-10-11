using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class EffectAction
{
    public bool hideDescription = false;
    public abstract void ApplyEffect(CharacterIdentifier user);
    public abstract string DescribeNoFormat();
    public string DescribeFormatted()
    {
        try
        {
            string description = DescribeNoFormat();
            return $"{"{spd=stat, col=stat}"}{description}{"{col=,spd=}"}";
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return "{col=err}[ERROR]{col=} ";
        }
    }
    public override string ToString() => DescribeFormatted();
}

public enum StatModType { Flat, PercentMissing, PercentCurrent, PercentMax }

public static class StatModTypeExtension
{
    public static float ApplyMod(this StatModType modType, float amount, float current, float max)
    {
        switch (modType)
        {
            case StatModType.Flat: return amount;
            case StatModType.PercentMissing: return (max - current) * (amount / 100);
            case StatModType.PercentCurrent: return current * (amount / 100);
            case StatModType.PercentMax: return max * (amount / 100);
        }
        throw new NotImplementedException($"{nameof(StatModType)}: Not implemented math for {modType}");
    }
    public static int ApplyModInt(this StatModType modType, float amount, float current, float max) =>
        Mathf.RoundToInt(ApplyMod(modType, amount, current, max));
}

// ----------------------------
// EFFECTS BELOW!!!!!!!
// ----------------------------

[Serializable]
public class MultipleEffectsAction : EffectAction
{
    [Box, Polymorphic, SerializeReference] public EffectAction[] effects = new EffectAction[1];
    public override void ApplyEffect(CharacterIdentifier user)
    {
        foreach(EffectAction effect in effects)
            effect.ApplyEffect(user);
    }

    public override string DescribeNoFormat()
    {
        return string.Join("", effects.Select((e) => e.DescribeNoFormat()));
    }
}

[Serializable]
public class ModifyHealthAction : EffectAction
{
    [Unbox, Polymorphic, SerializeReference] public EffectActionTarget target = new TargetSelf();
    public StatModType modifierType = StatModType.Flat;
    public int amount = 1;

    public override void ApplyEffect(CharacterIdentifier user)
    {
        /*
        var targetCharacter = target.GetTarget(user);
        var stats = targetCharacter.currentStats;
        targetCharacter.ModifyHealth(modifierType.ApplyMod(amount, stats.health, stats.maxHealth));
        // */
    }

    public override string DescribeNoFormat()
    {
        if (amount == 0) return "";

        if (amount > 0)
            return DescribeHealing();
        else
            return DescribeDealingDamage();
    }

    private string DescribeHealing()
    {
        switch (modifierType)
        {
            case StatModType.Flat: return $"[Heals {target} by {amount} HP] ";
            case StatModType.PercentMissing: return $"[Heals {target} by {amount}% missing HP] ";
            case StatModType.PercentCurrent: return $"[Heals {target} by {amount}% HP] ";
            case StatModType.PercentMax: return $"[Heals {target} by {amount}% max HP] ";
        }
        Debug.LogError($"{nameof(ModifyHealthAction)}: Does not have a description for healing {modifierType}: " +
            $"Falling back to back to ??? as description");
        return $"[???] ";
    }
    private string DescribeDealingDamage()
    {
        switch (modifierType)
        {
            case StatModType.Flat: return $"[Deals {amount} DMG to {target}] ";
            case StatModType.PercentMissing: return $"[Deals {amount}% missing HP DMG to {target}] ";
            case StatModType.PercentCurrent: return $"[Deals {amount}% HP DMG to {target}] ";
            case StatModType.PercentMax: return $"[Deals {amount}% max HP DMG to {target}] ";
        }
        Debug.LogError($"{nameof(ModifyHealthAction)}: Does not have a description for dealing damage {modifierType}: " +
            $"Falling back to back to ??? as description");
        return $"[???] ";
    }
}

[Serializable]
public class ModifyCorruptionAction : EffectAction
{
    [Unbox, Polymorphic, SerializeReference] public EffectActionTarget target;
    public StatModType modifierType = StatModType.Flat;
    public int amount = 1;

    public override void ApplyEffect(CharacterIdentifier user)
    {
        /*
        var targetCharacter = target.GetTarget(user);
        var stats = targetCharacter.currentStats;
        targetCharacter.currentStats.corruption += 
                modifierType.ApplyModInt(amount, stats.corruption, stats.maxCorruption);

        // */
    }

    public override string DescribeNoFormat()
    {
        string corrupts = "Corrupts";
        int positiveAmount = amount;
        if (amount == 0) return "";
        if (amount < 0)
        {
            corrupts = "Removes corruption on";
            positiveAmount *= -1;
        }
        switch (modifierType)
        {
            case StatModType.Flat: return $"[{corrupts} {target} by {positiveAmount}] ";
            case StatModType.PercentMissing: return $"[{corrupts} {target} by {positiveAmount}% of missing corruption] ";
            case StatModType.PercentCurrent: return $"[{corrupts} {target} by {positiveAmount}% of corruption] ";
            case StatModType.PercentMax: return $"[{corrupts} {target} by {positiveAmount}% max corruption] ";
        }
        Debug.LogError($"{nameof(ModifyCorruptionAction)}: Does not have a description for ModifyCorruption {modifierType}: " +
            $"Falling back to back to ??? as description");
        return $"[???] ";
    }
}

[Serializable]
public class GiveItemsEffect : EffectAction
{
    //[Polymorphic, SerializeReference] public EffectActionTarget target = new TargetSelf();
    public Item itemToGive;
    public int count;

    public override void ApplyEffect(CharacterIdentifier user)
    {
        Inventory inventoryToAddTo = GameInstance.Get<GI_AuHoGameState>().currentGameState.inventory;
        for (int i = 0;  i < count; i++)
            inventoryToAddTo.TryAddItem(itemToGive);
    }

    public override string DescribeNoFormat()
    {
        string target = "self"; //Placeholder until target is implemented for giving items
        if (count == 0) return "";
        if (count == 1) return $"[Give {target} {itemToGive.displayName}] ";
        return $"[Give {target} {count} {itemToGive.displayName}s] ";

    }
}

[Serializable]
public class ModifierForTimedDuration : EffectAction
{
    public float seconds;
    [Polymorphic, SerializeReference] public EffectActionTarget target = new TargetSelf();
    [Box, Polymorphic, SerializeReference] public ICharacterStatModInstancer modifier;

    public override void ApplyEffect(CharacterIdentifier user)
    {
        Modifier appliedModifier = modifier.GetNewRegisteredModifier(target.GetTargetsFrom(user));
        GameInstance.SendCoroutine(RemoveModifierAfterTime(appliedModifier));
    }
    public IEnumerator RemoveModifierAfterTime(Modifier toRemove)
    {
        yield return new WaitForSeconds(seconds);
        toRemove.UnregisterModifier();
    }

    public override string DescribeNoFormat()
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
            return $"[{describable.Description} for {XXm}{XXs}] ";
        }
        return "";
    }
}

/*
[Serializable]
public class ModifierForever : EffectAction
{
    //[Polymorphic, SerializeReference] public EffectActionTarget target = new TargetSelf();
    [Box, Polymorphic, SerializeReference] public Modifier modifier;

    public override void ApplyEffect(Character user)
    {
        modifier.RegisterModifier(multiRegister: true);
    }

    public override string DescribeNoFormat()
    {
        if (modifier is IDescribable describable)
        {
            string description = describable.Description;
            if (description != null)
                return $"[{description} forever] ";
        }
        return "";
    }
}
// */