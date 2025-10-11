using System;
using static CharacterStats;

public interface CharacterStat : INumberModifiable
{
    public CharacterIdentifier linkedCharacter { get; set; }
    public bool IsStatType(StatType stat);
    public CharacterStat GetClonedStat();
}

[Serializable]
public class CharacterStatInt : ModifiableInt, CharacterStat
{
    private StatType statType;
    public CharacterStatInt(int startValue, StatType statType) : base(startValue)
        => this.statType = statType;

    // CharacterStat implementation -------------------------------------------------------------------
    public CharacterIdentifier linkedCharacter { get; set; }
    public bool IsStatType(StatType stat) => statType == stat;
    public CharacterStat GetClonedStat() => new CharacterStatInt(startValue, statType);
}

[Serializable]
public class CharacterStatFloat : ModifiableFloat, CharacterStat
{
    private StatType statType;
    public CharacterStatFloat(float startValue, StatType statType) : base(startValue)
        => this.statType = statType;

    // CharacterStat implementation -------------------------------------------------------------------
    public CharacterIdentifier linkedCharacter { get; set; }
    public bool IsStatType(StatType stat) => statType == stat;
    public CharacterStat GetClonedStat() => new CharacterStatFloat(startValue, statType);
}

