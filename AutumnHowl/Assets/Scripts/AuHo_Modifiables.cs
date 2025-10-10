using System;
using static CharacterStats;

public class AuHo_Modifiables { }

public interface CharacterStat : INumberModifiable
{
    public Character linkedCharacter { get; set; }
    public bool IsStat(StatType stat);
}

[Serializable]
public class CharacterStatInt : ModifiableInt, CharacterStat
{
    public Character linkedCharacter { get; set; }

    private StatType statType;
    public CharacterStatInt(int startValue, StatType statType) : base(startValue)
    {
        this.statType = statType;
    }


    public bool IsStat(StatType stat) => statType == stat;
}
[Serializable]
public class CharacterStatFloat : ModifiableFloat, CharacterStat
{
    public Character linkedCharacter { get; set; }

    private StatType statType;
    public CharacterStatFloat(float startValue, StatType statType) : base(startValue)
    {
        this.statType = statType;
    }


    public bool IsStat(StatType stat) => statType == stat;
}

