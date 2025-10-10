using ErryLib.Reflection;
using System;
using System.Reflection;
using UnityEngine;
using static CharacterStats;
using static CharacterStats.StatType;

[Serializable]
public class CharacterStats
{
    public enum StatType
    {
        [StatName("DEF")] Defense,
        [StatName("ATK")] Attack,
        [StatName("MAX HP")] MaxHealth,
        [StatName("MAX PWR")] MaxPower,
        [StatName("MAX COR")] MaxCorruption,
        [StatName("SPD")] MoveSpeed
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


    public void LinkCharacterStatsToCharacter(Character character)
    {
        foreach (MemberInfo member in typeof(CharacterStats).GetCachedMemberInfos())
        {
            if (member is FieldInfo field)
            {
                object value = field.GetValue(this);
                if (value != null && value is CharacterStat stat)
                {
                    stat.linkedCharacter = character;
                }
            }
        }
    }

    #region HelperProperties
    public float PercentCurrentHealth => health / maxHealth;
    public float PercentMissingHealth => 1f - PercentCurrentHealth;
    public float MissingHealth => maxHealth - health;

    public float PercentCurrentCorruption => ((float)corruption) / maxCorruption;
    public float PercentMissingCorruption => 1f - PercentCurrentCorruption;
    public float MissingCorruption => maxCorruption - corruption;

    public float PercentCurrentPower => ((float)power) / maxPower;
    public float PercentMissingPower => 1f - PercentCurrentPower;
    public float MissingPower => maxPower - power;
    #endregion
}

public static partial class AuHo_ExtentionMethods
{
    public static string GetStatName(this StatType statType)
    {
        var attribute = statType.GetAttributeOfType<StatNameAttribute>();
        if (attribute != null)
            return attribute.statName;
        return null;
    }
}
public class StatNameAttribute : Attribute
{
    public string statName;
    public StatNameAttribute(string statName) 
    { 
        this.statName = statName; 
    } 
}