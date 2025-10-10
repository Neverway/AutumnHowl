using System;

[Serializable]
public abstract class EffectActionTarget : IDescribable
{
    public string Description => Describe();
    protected abstract string Describe();

    public CharacterTargets GetTargetsFrom(Character user)
        => new CharacterTargets(this, user);
    public abstract bool IsTargeted(Character user, Character other);
    public override string ToString() => Describe();
}

public struct CharacterTargets
{
    private Func<Character, bool> filterFunc;
    public CharacterTargets(EffectActionTarget targetType, Character user)
    {
        filterFunc = (other) => targetType.IsTargeted(user, other);
    }
    public CharacterTargets(Func<Character, bool> filterFunc)
    {
        if (filterFunc == null)
            throw new NullReferenceException();

        this.filterFunc = filterFunc;
    }

    public bool IsTargeted(Character character) => filterFunc.Invoke(character);
}

// ----------------------------
// TARGET TYPES BELOW!!!!!
// ----------------------------

[Serializable]
public class TargetSelf : EffectActionTarget
{
    public override bool IsTargeted(Character user, Character other) => user == other;
    protected override string Describe() => "self";
}

[Serializable]
public class TargetNearestEnemy : EffectActionTarget
{
    public override bool IsTargeted(Character user, Character other)
    {
        throw new NotImplementedException("Need to define how to get " +
            "nearest target");
    }
    protected override string Describe() => "nearest enemy";
}

[Serializable]
public class TargetAllEnemies : EffectActionTarget
{

    public override bool IsTargeted(Character user, Character other)
    {
        throw new NotImplementedException("Need to define how to get " +
            "all enemies");
    }
    protected override string Describe() => "all enemies";
}


