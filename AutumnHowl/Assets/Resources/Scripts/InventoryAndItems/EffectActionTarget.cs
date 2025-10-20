using System;

[Serializable]
public abstract class EffectActionTarget : IDescribable
{
    public abstract string Description { get; }

    public CharacterTargets GetTargetsFrom(CharacterIdentifier user)
        => new CharacterTargetsFromUser(this, user);
    public abstract bool IsTargeted(CharacterIdentifier user, CharacterIdentifier other);
    public override string ToString() => Description;
}

public abstract class CharacterTargets
{
    /// <summary>Returns true if this CharacterTargets is targeting the given character</summary>
    public abstract bool IsTargeted(CharacterIdentifier character);
}
public class CharacterTargetsFromUser : CharacterTargets
{
    public EffectActionTarget TargetType { get; private set; }
    public CharacterIdentifier User { get; private set; }
    public CharacterTargetsFromUser(EffectActionTarget targetType, CharacterIdentifier user)
    {
        TargetType = targetType;
        User = user;
    }
    public override bool IsTargeted(CharacterIdentifier character) => TargetType.IsTargeted(User, character);

    /// <summary>Pass in the type of target you want, and the user that is doing the targeting, and will return the associated CharacterTargets
    /// <br/> - Example: CharacterTargets.GetFrom<TargetNearestEnemy>(user)</summary>
    public static CharacterTargets GetFrom<TTarget>(CharacterIdentifier user)
        where TTarget : EffectActionTarget, new()
        => new TTarget().GetTargetsFrom(user);
}
public class CharacterTargetsFromFunc : CharacterTargets
{
    private Func<CharacterIdentifier, bool> filterFunc;
    public CharacterTargetsFromFunc(Func<CharacterIdentifier, bool> filterFunc)
    {
        this.filterFunc = filterFunc;
    }
    public override bool IsTargeted(CharacterIdentifier character) => filterFunc.Invoke(character);

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

