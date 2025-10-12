using System;

public interface CharacterStat : INumberModifiable
{
    public CharacterIdentifier linkedCharacter { get; set; }
    public bool IsStatType(CharacterStatType stat);
    public CharacterStat GetClonedStat();
}

[Serializable]
public class CharacterStatInt : ModifiableInt, CharacterStat
{
    private CharacterStatType statType;
    public CharacterStatInt(int startValue, CharacterStatType statType) : base(startValue)
        => this.statType = statType;

    // CharacterStat implementation -------------------------------------------------------------------
    public CharacterIdentifier linkedCharacter { get; set; }
    public bool IsStatType(CharacterStatType stat) => statType == stat;
    public CharacterStat GetClonedStat() => new CharacterStatInt(startValue, statType);
}

[Serializable]
public class CharacterStatFloat : ModifiableFloat, CharacterStat
{
    private CharacterStatType statType;
    public CharacterStatFloat(float startValue, CharacterStatType statType) : base(startValue)
        => this.statType = statType;

    // CharacterStat implementation -------------------------------------------------------------------
    public CharacterIdentifier linkedCharacter { get; set; }
    public bool IsStatType(CharacterStatType stat) => statType == stat;
    public CharacterStat GetClonedStat() => new CharacterStatFloat(startValue, statType);
}

