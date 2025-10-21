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
    public Character owner { get; set; }

    /*-----[ Constructors ]-------------------------------------------------------------------------------------------*/
    public CharacterStats() 
    { 
    }
    public CharacterStats(CharacterIdentifier character) : base() => SetupStatsLinkedToCharacter(character);

    /*-----[ Save/Load SaveData ]-------------------------------------------------------------------------------------*/
    [Serializable]
    public struct SaveData
    {
        public float health;
        public float level;
        public int power;
        public int corruption;
    }
    public SaveData GetSaveData() => new SaveData()
    {
        health = this.health,
        level = this.level,
        power = this.power,
        corruption = this.corruption
    };
    public void LoadSaveData(SaveData saveData)
    {
        this.health = saveData.health;
        this.level = saveData.level;
        this.power = saveData.power;
        this.corruption = saveData.corruption;
    }


    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/

    [Header("Starting Values for Valued-Stats")]
    public float health = 100;
    public float level = 0;
    public int power = 10;
    public int corruption = 0;

    [Header("Combat Stats")]
    [Box] public CharacterStatInt attack = new(10, Attack);
    [Box] public CharacterStatInt defense = new(10, Defense);

    [Box] public CharacterStatFloat maxHealth = new(100, MaxHealth);
    [Box] public CharacterStatFloat maxLevel = new(100, MaxLevel);
    [Box] public CharacterStatInt maxAttack = new(100, MaxAttack);
    [Box] public CharacterStatInt maxDefense = new(100, MaxDefense);
    [Box] public CharacterStatInt maxPower = new(100, MaxPower);
    [Box] public CharacterStatInt maxCorruption = new(100, MaxCorruption);


    [Header("Overworld Stats")]
    [Box] public CharacterStatFloat walkSpeed = new(2, MoveSpeed);
    [Box] public CharacterStatFloat runSpeed = new(3, MoveSpeed);

    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    public void SetupStatsLinkedToCharacter(CharacterIdentifier character)
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
    }

    public void RefreshStatIDs()
    {
        CharacterStats defaults = new CharacterStats();

        //Loop through all fields in the CharacterStats class and process them if they are a CharacterStat field
        foreach (MemberInfo member in typeof(CharacterStats).GetCachedMemberInfos())
            if (member is FieldInfo field && typeof(CharacterStat).IsAssignableFrom(field.FieldType))
            {
                //Ignore static fields (Should really not ever happen anyways)
                if (member.IsStatic()) continue;

                //Get the default stat from a new instance of CharacterStats
                CharacterStat defaultStat = field.GetValue(defaults) as CharacterStat;
                if (defaultStat == null)
                {
                    Debug.LogError($"{nameof(CharacterStats)}: Attempting to get default ID on a " +
                                   $"CharacterStat {field.Name}, but it was null. Unable to get default ID from stat");
                    continue;
                }

                //Get the current stat
                CharacterStat currentStat = field.GetValue(this) as CharacterStat;
                if (defaultStat == null)
                {
                    Debug.LogError($"{nameof(CharacterStats)}: Attempting to get a reference to our own stat on a " +
                                   $"CharacterStat {field.Name}, but it was null. Unable to apply default ID to it");
                    continue;
                }

                //Clone the stat, and replace this stat with the clone, and link the given character ID to this stat
                currentStat.StatType = defaultStat.StatType;
            }
    }
    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    /// <summary>
    /// Modify the current stats on a character
    /// </summary>
    /// <param name="_stat">Which of the character's stat is affected</param>
    /// <param name="_amount">How much to add to that stat</param>
    /// <param name="_direction">The direction in which this effect is coming from (used for detecting damage direction)</param>
    public void ModifyHealth(float _amount, Vector2Int _direction = new Vector2Int())
    {
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
    //Make sure the numbers stay the same and unique or we will have to reserialize the CharacterTemplates
    [StatName("ATK")]     Attack,
    [StatName("DEF")]     Defense,

    [StatName("MAX HP")]  MaxHealth,
    [StatName("MAX LVL")] MaxLevel,
    [StatName("MAX ATK")] MaxAttack,
    [StatName("MAX DEF")] MaxDefense,
    [StatName("MAX PWR")] MaxPower,
    [StatName("MAX COR")] MaxCorruption,

    [StatName("SPD")]     MoveSpeed
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