using ErryLib.Reflection;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static CharacterStatType;

public enum CharacterStatType
{
    [StatName("DEF")] Defense,
    [StatName("ATK")] Attack,
    [StatName("MAX HP")] MaxHealth,
    [StatName("MAX PWR")] MaxPower,
    [StatName("MAX COR")] MaxCorruption,
    [StatName("SPD")] MoveSpeed
}

[Serializable]
public class CharacterStats
{
    public CharacterStats() { }
    public CharacterStats(CharacterIdentifier character) => SetupStatsLinkedToCharacter(character);

    [HideInInspector] public float health = 100;
    [HideInInspector] public float level = 0;
    [HideInInspector] public int power = 5;
    [HideInInspector] public int corruption = 0;

    [Tooltip("When damage is taken, this is how much damage is negated")]
    [Header("Combat Stats")]
    [Box] public CharacterStatInt defense = new(0, Defense);
    [Box] public CharacterStatInt attack = new(5, Attack);
    [Box] public CharacterStatFloat maxHealth = new(100, MaxHealth);
    [Box] public CharacterStatInt maxPower = new(5, MaxPower);
    [Box] public CharacterStatInt maxCorruption = new(5, MaxCorruption);

    [Header("Overworld Stats")]
    [Box] public CharacterStatFloat walkSpeed = new(2, MoveSpeed);
    [Box] public CharacterStatFloat runSpeed = new(3, MoveSpeed);

    public void SetupStatsLinkedToCharacter(CharacterIdentifier character)
    {
        bool hasNoTemplate = character.TemplateCreatedFrom == null;

        //Loop through all fields in the CharacterStats class and process them if they are a CharacterStat field
        foreach (MemberInfo member in typeof(CharacterStats).GetCachedMemberInfos())
            if (member is FieldInfo field && typeof(CharacterStat).IsAssignableFrom(field.FieldType))
            {
                //Ignore static fields (Should really not ever happen anyways)
                if (member.IsStatic()) continue;
                //If there is no template, it is likely created with no template.
                if (hasNoTemplate)
                {
                    CharacterStat stat = field.GetValue(this) as CharacterStat;
                    stat.linkedCharacter = character; //Safe to assign linkedCharacter since it wont override any CharacterTemplate.baseStats
                    continue;
                }
                //Get the base stat from the character template
                CharacterStat statToClone = field.GetValue(character.TemplateCreatedFrom.baseStats) as CharacterStat;
                if (statToClone == null)
                {
                    Debug.LogError($"{nameof(CharacterStats)}: Attempting to clone a " +
                        $"CharacterStat {field.Name}, but it was null. Unable to link character to stat");
                    continue;
                }
                //Clone the stat, and replace this stat with the clone, and link the given character ID to this stat
                CharacterStat myStat = statToClone.GetClonedStat();
                field.SetValue(this, myStat);
                myStat.linkedCharacter = character;
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
    public static string GetStatName(this CharacterStatType statType)
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