using System;

[Serializable]
public abstract class EffectActionTarget : IDescribable
{
    public abstract string Description { get; }

    public CharacterTargets GetTargetsFrom(CharacterIdentifier user)
        => new CharacterTargets(this, user);
    public abstract bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other);
    public override string ToString() => Description;
}

public struct CharacterTargets
{
    private Func<CharacterIdentifier, bool> filterFunc;
    public CharacterTargets(EffectActionTarget targetType, CharacterIdentifier user)
    {
        filterFunc = (other) => targetType.IsTargeted(user, other);
    }
    public CharacterTargets(Func<CharacterIdentifier, bool> filterFunc)
    {
        if (filterFunc == null)
            throw new NullReferenceException();

        this.filterFunc = filterFunc;
    }
    /// <summary>Returns true if this CharacterTargets is targeting the given character</summary>
    public bool IsTargeted(CharacterIdentifier character) => filterFunc.Invoke(character);

    /// <summary>Pass in the type of target you want, and the user that is doing the targeting, and will return the associated CharacterTargets
    /// <br/> - Example: CharacterTargets.GetFrom<TargetNearestEnemy>(user)</summary>
    public static CharacterTargets GetFrom<TTarget>(CharacterIdentifier user)
        where TTarget : EffectActionTarget, new()
        => new TTarget().GetTargetsFrom(user);
}

// ----------------------------
// TARGET TYPES BELOW!!!!!
// ----------------------------

[Serializable]
public class TargetAll : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) => true;
    public override string Description => "all";
}

[Serializable]
public class TargetSelf : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other) => user == other;
    public override string Description => "self";
}

[Serializable]
public class TargetNearestEnemy : EffectActionTarget
{
    public override bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other)
    {
        throw new NotImplementedException("Need to define how to get " +
            "nearest target");
    }
    public override string Description => "nearest enemy";
}

