//==========================================( Neverway 2025 )=========================================================//
// Author
//  Errynei
//
// Contributors
//  Lizband
//
//====================================================================================================================//

using ErryLib.Reflection;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static CharacterStatType;

[Serializable]
public class CharacterStats
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public CharacterStats() { }
    public CharacterStats(CharacterIdentifier character) => SetupStatsLinkedToCharacter(character);

    [Box] public float health = 100;
    [Box] public float level = 0;
    [Box] public CharacterStatInt attack = new(10, Attack);
    [Box] public CharacterStatInt defense = new(10, Attack);
    [Box] public int power = 10;
    [Box] public int corruption = 0;

    [Header("Combat Stats")]
    [Box] public CharacterStatFloat maxHealth = new(100, Health);
    [Box] public CharacterStatFloat maxLevel = new(100, Level);
    [Box] public CharacterStatInt maxAttack = new(100, Attack);
    [Box] public CharacterStatInt maxDefense = new(100, Defense);
    [Box] public CharacterStatInt maxPower = new(100, Power);
    [Box] public CharacterStatInt maxCorruption = new(100, Corruption);

    [Header("Overworld Stats")]
    [Box] public CharacterStatFloat walkSpeed = new(2, MoveSpeed);
    [Box] public CharacterStatFloat runSpeed = new(3, MoveSpeed);

    public Character owner;

    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void SetupStatsLinkedToCharacter(CharacterIdentifier character)
    {
        int someInt = attack + defense;

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
                    stat.LinkedCharacter = character; //Safe to assign linkedCharacter since it wont override any CharacterTemplate.baseStats
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
                myStat.LinkedCharacter = character;
            }
        health = maxHealth;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Modify the current stats on a character
    /// </summary>
    /// <param name="_stat">Which of the character's stat is affected</param>
    /// <param name="_amount">How much to add to that stat</param>
    /// <param name="_direction">The direction in which this effect is coming from (used for detecting damage direction)</param>
    public void Modify(CharacterStatType _stat, float _amount, Vector2Int _direction = new Vector2Int())
    {
        switch (_stat)
        {
            case Health:
                // Character healed
                if (_amount > 0)
                {
                    if (health + _amount > maxHealth) health = maxHealth;
                    else health += _amount;
                    GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), owner.transform.position, 1);
                    // TODO - HOW teH HeCk do I call this now? ~Liz
                    //OnHeal?.Invoke();
                }
        
                if (_amount == 0) return;
        
                // Character damaged
                else if (_amount < 0)
                {
                    var totalAmount = _amount;
            
                    // Apply defense if active
                    if (owner.isDefenseActive)
                    {
                        totalAmount = _amount + defense;
                        GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), owner.transform.position, 0);
                        GameInstance.Get<GI_WidgetManager>().SpawnEffectText(defense.ToString(), owner.transform.position, 2, 0.5f);
                    }
            
                    // Damage killed
                    if (health + totalAmount <= 0)
                    {
                        health = 0;
                        //GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), transform, 0);
                        owner.isDead = true;
                        // TODO - HOW teH HeCk do I call this now? ~Liz
                        //OnDeath?.Invoke();
                    }
                    // Damage hurt
                    else
                    {
                        health += totalAmount;
                        GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), owner.transform.position, 0);
                        // TODO - HOW teH HeCk do I call this now? ~Liz
                        //OnHurt?.Invoke();
                    }
                }
                break;
            case Level:
                break;
            case Attack:
                break;
            case Defense:
                break;
            case Power:
                break;
            case Corruption:
                break;
            case MoveSpeed:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_stat), _stat, null);
        }
    }
    
    /// <summary>
    /// If there's enough Power, consume the given amount, otherwise return false.
    /// </summary>
    /// <param name="_amount">Amount of Power to consume</param>
    /// <returns></returns>
    public bool TryUsePower (int _amount)
    {
        if (power >= _amount)
        {
            power -= _amount;
            return true;
        }
        return false;
    }

    #endregion
    
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


public enum CharacterStatType
{
    [StatName("MAX HP")] Health,
    [StatName("MAX LVL")] Level,
    [StatName("MAX ATK")] Attack,
    [StatName("MAX DEF")] Defense,
    [StatName("MAX PWR")] Power,
    [StatName("MAX COR")] Corruption,
    [StatName("SPD")] MoveSpeed
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