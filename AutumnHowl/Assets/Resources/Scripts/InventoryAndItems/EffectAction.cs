using ErryLib.ModiferSystem.Instancers;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>Base class for all EffectActions! 
/// (Polymorphic and serializeable actions that can be defined on objects) </summary>
[Serializable]
public abstract class EffectAction
{
    public bool hideDescription = false;

    /// <summary>Call this when you want to trigger this action</summary>
    public abstract void ApplyEffect(CharacterIdentifier user);
    /// <summary>Get the description of the action without any added formatting for displaying stat colors</summary>
    public abstract string DescribeNoFormat();
    /// <summary>Gets the description with colors and text speed added to it, or [ERROR] if an error occurred</summary>
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
    /// <summary>Automatically converts this class to a string without needing to cast it</summary>
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

[Serializable]
public abstract class TargetedEffectAction : EffectAction
{
    [Unbox, Polymorphic, SerializeReference] public EffectActionTarget target = new TargetSelf();
    public override void ApplyEffect(CharacterIdentifier user)
    {
        foreach (CharacterIdentifier target in target.GetTargetsFrom(user).AllTargets)
            ApplyEffectToTarget(target);
    }
    public abstract void ApplyEffectToTarget(CharacterIdentifier target);
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
        return string.Join(" ", effects.Select((e) => e.DescribeNoFormat()));
    }
}

[Serializable]
public class ModifyHealthAction : TargetedEffectAction
{
    public StatModType modifierType = StatModType.Flat;
    public int amount = 1;

    public override void ApplyEffectToTarget(CharacterIdentifier target)
    {
        CharacterStats stats = target.Stats;
        stats.ModifyHealth(modifierType.ApplyMod(amount, stats.health, stats.maxHealth));
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
            case StatModType.Flat: return $"[Heals {target} by {amount} HP]";
            case StatModType.PercentMissing: return $"[Heals {target} by {amount}% missing HP]";
            case StatModType.PercentCurrent: return $"[Heals {target} by {amount}% HP]";
            case StatModType.PercentMax: return $"[Heals {target} by {amount}% max HP]";
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
public class ModifyCorruptionAction : TargetedEffectAction
{
    public StatModType modifierType = StatModType.Flat;
    public int amount = 1;

    public override void ApplyEffectToTarget(CharacterIdentifier user) => user.Stats.ModifyCorruption(amount);
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
            case StatModType.Flat: return $"[{corrupts} {target} by {positiveAmount}]";
            case StatModType.PercentMissing: return $"[{corrupts} {target} by {positiveAmount}% of missing corruption]";
            case StatModType.PercentCurrent: return $"[{corrupts} {target} by {positiveAmount}% of corruption]";
            case StatModType.PercentMax: return $"[{corrupts} {target} by {positiveAmount}% max corruption]";
        }
        Debug.LogError($"{nameof(ModifyCorruptionAction)}: Does not have a description for ModifyCorruption {modifierType}: " +
            $"Falling back to back to ??? as description");
        return $"[???] ";
    }
}

[Serializable]
public class ModifyPowerAction : TargetedEffectAction
{
    public StatModType modifierType = StatModType.Flat;
    public int amount = 1;

    public override void ApplyEffectToTarget (CharacterIdentifier user) => user.Stats.ModifyPower (amount);
    public override string DescribeNoFormat ()
    {
        string power = "Restores Power of";
        string restores = "Restores ";
        int positiveAmount = amount;
        if (amount == 0) return "";
        if (amount < 0)
        {
            power = "Removes power from";
            restores = "Consumes ";
            positiveAmount *= -1;
        }
        switch (modifierType)
        {
            case StatModType.Flat: return $"[{restores} {positiveAmount} Power of {target}]";
            case StatModType.PercentMissing: return $"[{power} {target} by {positiveAmount}% of missing power]";
            case StatModType.PercentCurrent: return $"[{power} {target} by {positiveAmount}% of power]";
            case StatModType.PercentMax: return $"[{power} {target} by {positiveAmount}% max power]";
        }
        Debug.LogError ($"{nameof (ModifyCorruptionAction)}: Does not have a description for ModifyPower {modifierType}: " +
            $"Falling back to back to ??? as description");
        return $"[???] ";
    }
}

[Serializable]
public class GiveItemsEffect : EffectAction
{
    //[Polymorphic, SerializeReference] public EffectActionTarget target = new TargetSelf();
    public string EffectDescription;
    [Box, Polymorphic, SerializeReference] public ItemsReference itemToGive;
    public override void ApplyEffect(CharacterIdentifier user)
    {
        if (!user.IsPlayer()) return;

        Inventory inventoryToAddTo = GameInstance.Gamestate.inventory;
        Item[] items = itemToGive.GetItems(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        foreach (Item item in items)
            inventoryToAddTo.TryAddItem(item);
    }

    public override string DescribeNoFormat() => $"[{EffectDescription}]";
}



[Serializable]
public class ModifierForXTurns : TargetedEffectAction
{
    public SpriteRef[] modifierIcons;
    public int turns;
    [Box, Polymorphic, SerializeReference] public SerializedModifier modifier;

    public override void ApplyEffectToTarget(CharacterIdentifier target)
    {
        Modifier appliedModifier = modifier.GetNew_Flexible(new TargetSelf().GetTargetsFrom(target));
        appliedModifier.RegisterModifier();
        GameInstance.SendCoroutine(RemoveModifierAfterTurns(appliedModifier, target.IsPlayer()));
    }
    public IEnumerator RemoveModifierAfterTurns(Modifier toRemove, bool doIcons)
    {
        if (doIcons) WB_ModifierIcons.AddIcon(toRemove, modifierIcons.NotNull().Select(sr => sr.sprite).ToArray());
        EventCounter<Event_BattleTurnPassed> turnCounter = new EventCounter<Event_BattleTurnPassed>();

        while (turnCounter.counter < turns)
            yield return null;

        toRemove.UnregisterModifier();
        turnCounter.Discard();
        if (doIcons) WB_ModifierIcons.RemoveIcon(toRemove);
    }

    public override string DescribeNoFormat()
    {
        if (hideDescription || turns <= 0) return "";

        if (modifier != null && modifier is IDescribable describable)
        {
            string description = describable.Description;
            if (string.IsNullOrEmpty(description))
                return "";

            return $"[{describable.Description} to {target} for {turns} step{(turns == 1 ? "" : "s" )}]";
        }
        return "";
    }
}

[Serializable]
public class ModifierForXWaves : TargetedEffectAction
{
    public SpriteRef[] modifierIcons;
    public int waves;
    [Box, Polymorphic, SerializeReference] public SerializedModifier modifier;

    public override void ApplyEffectToTarget(CharacterIdentifier target)
    {
        Modifier appliedModifier = modifier.GetNew_Flexible(new TargetSelf().GetTargetsFrom(target));
        appliedModifier.RegisterModifier();
        GameInstance.SendCoroutine(RemoveModifierAfterWaves(appliedModifier, target.IsPlayer()));
    }
    public IEnumerator RemoveModifierAfterWaves(Modifier toRemove, bool doIcons)
    {
        if (doIcons) WB_ModifierIcons.AddIcon(toRemove, modifierIcons.NotNull().Select(sr => sr.sprite).ToArray());
        EventCounter<Event_BattleWavePassed> turnCounter = new EventCounter<Event_BattleWavePassed>();

        while (turnCounter.counter < waves) yield return null;

        toRemove.UnregisterModifier();
        turnCounter.Discard();
        if (doIcons) WB_ModifierIcons.RemoveIcon(toRemove);
    }

    public override string DescribeNoFormat()
    {
        if (hideDescription || waves <= 0) return "";

        if (modifier != null && modifier is IDescribable describable)
        {
            string description = describable.Description;
            if (string.IsNullOrEmpty(description))
                return "";

            return $"[{describable.Description} to {target} for {waves} wave{(waves == 1 ? "" : "s")}]";
        }
        return "";
    }
}

[Serializable]
public class ModifierUntilEndOfBattle : TargetedEffectAction
{
    public int battlesCount = 1;
    public SpriteRef[] modifierIcons;
    [Box, Polymorphic, SerializeReference] public SerializedModifier modifier;

    public override void ApplyEffectToTarget(CharacterIdentifier target)
    {
        Modifier appliedModifier = modifier.GetNew_Flexible(new TargetSelf().GetTargetsFrom(target));
        appliedModifier.RegisterModifier();
        GameInstance.SendCoroutine(RemoveModifierAfterEndOfBattle(appliedModifier, target.IsPlayer()));
    }
    public IEnumerator RemoveModifierAfterEndOfBattle(Modifier toRemove, bool doIcons)
    {
        if (doIcons) WB_ModifierIcons.AddIcon(toRemove, modifierIcons.NotNull().Select(sr => sr.sprite).ToArray());
        EventCounter<Event_BattleWon> turnCounter = new EventCounter<Event_BattleWon>();

        while (turnCounter.counter < battlesCount)
            yield return null;

        toRemove.UnregisterModifier();
        turnCounter.Discard();
        if (doIcons) WB_ModifierIcons.RemoveIcon(toRemove);
    }

    public override string DescribeNoFormat()
    {
        if (hideDescription || battlesCount <= 0) return "";

        if (modifier != null && modifier is IDescribable describable)
        {
            string description = describable.Description;
            if (string.IsNullOrEmpty(description))
                return "";

            if (battlesCount == 1)
                return $"[{describable.Description} to {target} for 1 battle]";
            else
                return $"[{describable.Description} to {target} for {battlesCount} battles]";
        }
        return "";
    }
}

[Serializable]
public class ModifierForTimedDuration : TargetedEffectAction
{
    public SpriteRef[] modifierIcons;
    public float seconds;
    [Box, Polymorphic, SerializeReference] public SerializedModifier modifier;

    public override void ApplyEffectToTarget(CharacterIdentifier target)
    {
        Modifier appliedModifier = modifier.GetNew_Flexible(new TargetSelf().GetTargetsFrom(target));
        appliedModifier.RegisterModifier();
        GameInstance.SendCoroutine(RemoveModifierAfterTime(appliedModifier, target.IsPlayer()));
    }
    public IEnumerator RemoveModifierAfterTime(Modifier toRemove, bool doIcons)
    {
        if (doIcons) WB_ModifierIcons.AddIcon(toRemove, modifierIcons.NotNull().Select(sr => sr.sprite).ToArray());
        yield return new WaitForSeconds(seconds);
        toRemove.UnregisterModifier();
        if (doIcons) WB_ModifierIcons.RemoveIcon(toRemove);
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
            return $"[{describable.Description} to {target} for {XXm}{XXs}]";
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