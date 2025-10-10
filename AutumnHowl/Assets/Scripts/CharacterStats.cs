using System;
using UnityEngine;
using StatType = CharacterStats.StatType;
using static CharacterStats.StatType;

[Serializable]
public class CharacterStats
{
    [Polymorphic, SerializeReference] public Modifier modifier;

    public enum StatType
    {
        Defense, Attack, MaxHealth, MaxPower, MaxCorruption, MoveSpeed
    }

    [Header("Base Values")]
    public float health = 100;
    public float level = 0;
    public int power = 5;
    public int corruption = 0;
    [Tooltip("When damage is taken, this is how much damage is negated")]
    [Unbox] public CharacterStatInt defense = new(0, Defense);
    [Unbox] public CharacterStatInt attack = new(5, Attack);
    [Header("Max Values")]
    [Unbox] public CharacterStatFloat maxHealth = new(100, MaxHealth);
    [Unbox] public CharacterStatInt maxPower = new(5, MaxPower);
    [Unbox] public CharacterStatInt maxCorruption = new(5, MaxCorruption);

    [Unbox] public CharacterStatFloat walkSpeed = new(2, MoveSpeed);
    [Unbox] public CharacterStatFloat runSpeed = new(3, MoveSpeed);

    public float PercentCurrentHealth => health / maxHealth;
    public float PercentMissingHealth => 1f - PercentCurrentHealth;
    public float MissingHealth => maxHealth - health;

    public float PercentCurrentCorruption => ((float)corruption) / maxCorruption;
    public float PercentMissingCorruption => 1f - PercentCurrentCorruption;
    public float MissingCorruption => maxCorruption - corruption;

    public float PercentCurrentPower => ((float)power) / maxPower;
    public float PercentMissingPower => 1f - PercentCurrentPower;
    public float MissingPower => maxPower - power;
}

public interface CharacterStat : INumberModifiable
{
    public bool IsStat(StatType stat);
}
[Serializable]
public class CharacterStatInt : ModifiableInt, CharacterStat
{
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
    private StatType statType;
    public CharacterStatFloat(float startValue, StatType statType) : base(startValue)
    {
        this.statType = statType;
    }
    public bool IsStat(StatType stat) => statType == stat;
}

[Serializable]
public class CharacterStatModifier : NumberModifier<CharacterStat>
{
    public StatType statToModify;
    public override void ModifyValue(Modifiable modifiableValue)
    {
        //Only modify values that are CharacterStats of the specific StatType
        if (modifiableValue is CharacterStat characterStat)
            if (characterStat.IsStat(statToModify))
                base.ModifyValue(modifiableValue);
    }
}