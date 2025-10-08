using System;

[Serializable]
public abstract class EffectActionTarget
{
    public abstract Character GetTarget(Character user);
    public abstract string Describe();
    public override string ToString() => Describe();
}
[Serializable]
public class TargetSelf : EffectActionTarget
{
    public override Character GetTarget(Character user) => user;
    public override string Describe() => "self";
}
[Serializable]
public class TargetNearestEnemy : EffectActionTarget
{
    public override Character GetTarget(Character user)
    {
        throw new NotImplementedException("Need to define how to get " +
            "nearest target");
    }
    public override string Describe() => "nearest enemy";
}
[Serializable]
public class TargetAllEnemies : EffectActionTarget
{
    public override Character GetTarget(Character user)
    {
        throw new NotImplementedException("Need to define how to get " +
            "all enemies");
    }
    public override string Describe() => "all enemies";
}