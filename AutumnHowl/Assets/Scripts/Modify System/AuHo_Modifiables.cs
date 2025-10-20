using System;
using Unity.VisualScripting;

public interface CharacterStat : INumberModifiable
{
    public CharacterIdentifier LinkedCharacter { get; set; }
    public CharacterStatType StatType { get; set; }
    public bool IsStatType(CharacterStatType stat);
    public CharacterStat GetClonedStat();
}

[Serializable]
public class CharacterStatInt : ModifiableInt, CharacterStat
{
    public CharacterStatInt(int startValue, CharacterStatType statType) : base(startValue)
        => this.StatType = statType;

    // CharacterStat implementation -------------------------------------------------------------------
    public CharacterIdentifier LinkedCharacter { get; set; }
    public CharacterStatType StatType { get; set; }
    public bool IsStatType(CharacterStatType stat) => StatType == stat;
    public CharacterStat GetClonedStat() => new CharacterStatInt(startValue, StatType);
}

[Serializable]
public class CharacterStatFloat : ModifiableFloat, CharacterStat
{
    public CharacterStatFloat(float startValue, CharacterStatType statType) : base(startValue)
        => this.StatType = statType;

    // CharacterStat implementation -------------------------------------------------------------------
    public CharacterIdentifier LinkedCharacter { get; set; }
    public CharacterStatType StatType { get; set; }
    public bool IsStatType(CharacterStatType stat) => StatType == stat;
    public CharacterStat GetClonedStat() => new CharacterStatFloat(startValue, StatType);
}

