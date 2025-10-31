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
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static CharacterStatType;

[Serializable]
public class CharacterStats
{
    public CharacterIdentifier Identifier;
    public Character owner { get; set; }

    /*-----[ Constructors ]-------------------------------------------------------------------------------------------*/
    public CharacterStats() { }
    public CharacterStats(CharacterIdentifier character)
    {
        Identifier = character;
        SetupStatsLinkedToCharacter(character);
    }
    #region========================================( Variables )======================================================//
    /*-----[ Settable stats (Not modifiable) ]------------------------------------------------------------------------*/

    [Header("Starting Values for Valued-Stats")]
    [HideInInspector] public float health;
    public float level = 0;
    public int power = 10;
    public float corruption = 0;

    /*-----[ Modifiable stats (Not settable) ]------------------------------------------------------------------------*/
    [Header("Combat Stats")]
    [Box] public CharacterStatInt attack = new(10, Attack);
    [Box] public CharacterStatInt defense = new(10, Defense);

    [Box] public CharacterStatFloat maxHealth = new(100, MaxHealth);
    [Box] public CharacterStatFloat maxLevel = new(100, MaxLevel);
    [Box] public CharacterStatInt maxAttack = new(100, MaxAttack);
    [Box] public CharacterStatInt maxDefense = new(100, MaxDefense);
    [Box] public CharacterStatInt maxPower = new(100, MaxPower);
    [Box] public CharacterStatInt maxCorruption = new(100, MaxCorruption);

    [Box] public CharacterStatInt shieldPower = new(10, ShieldPower);

    [Header("Overworld Stats")]
    [Box] public CharacterStatFloat walkSpeed = new(2, MoveSpeed);
    [Box] public CharacterStatFloat runSpeed = new(3, MoveSpeed);

    #endregion

    #region=======================================( Functions )=======================================================//
    /*-----[ Modify Functions ]-------------------------------------------------------------------------------------*/
    
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
            //Create a heal event, stop if it gets interrupted by a modifier, and get modified value
            Event_Heal healEvent = new Event_Heal(Identifier, _amount);
            if (healEvent.IfInvokeInterrupted() || healEvent.healAmount <= 0) return;
            _amount = healEvent.healAmount;

            if (_amount <= 0) return;

            owner.InvokeOnHeal();

            if (health + _amount > maxHealth)
            {
                health = maxHealth;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText("MAX", owner.transform.position, 1);
            }
            else
            {
                health += _amount;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), owner.transform.position, 1);
            }

            return;
        }
        
        if (_amount == 0) return;
        
        // Character damaged
        if (_amount < 0)
        {
            //Create a damage event, stop if it gets interrupted by a modifier, and get modified value
            Event_TakeDamage damageEvent = new Event_TakeDamage(Identifier, _direction, -_amount);
            if (damageEvent.IfInvokeInterrupted() || damageEvent.damage <= 0) return;
            _amount = -damageEvent.damage;

            var totalAmount = _amount;

            // Apply defense if active
            if (owner.isDefenseActive)
            {
                totalAmount = _amount + defense;
                if (totalAmount > 0) { totalAmount = 0; }
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), owner.transform.position, 0);
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(defense.ToString(), owner.transform.position, 2, 0.5f);
            }

            // Damage killed
            if (health + totalAmount <= 0)
            {
                health = 0;
                if (owner.uniqueDeathBehavior == false)
                {
                    GameInstance.Get<GI_WidgetManager> ().SpawnEffectText ("DOWN", owner.transform.position, 0);
                    owner.isDead = true;
                    owner.InvokeOnDeath ();
                }
            }
            // Damage hurt
            else
            {
                health += totalAmount;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), owner.transform.position, 0);
                if (totalAmount > 0)
                    owner.InvokeOnHurt();
            }
        }

        /*
         *         // Character healed
       if (_amount > 0)
       {
           if (Stats.health + _amount > Stats.maxHealth) Stats.health = Stats.maxHealth;
           else Stats.health += _amount;
           GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), transform.position, 1);
           OnHeal?.Invoke();
       }

       if (_amount == 0) return;

       // Character damaged
       else if (_amount < 0)
       {
           var totalAmount = _amount;

           // Apply defense if active
           if (isDefenseActive)
           {
               totalAmount = _amount + Stats.defense;
               //Clamp to 0 so that it can't heal the character.
               if (totalAmount > 0) { totalAmount = 0; }
               GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), transform.position, 0);
               GameInstance.Get<GI_WidgetManager>().SpawnEffectText(Stats.defense.ToString(), transform.position, 2, 0.5f);
           }

           // Damage killed
           if (Stats.health + totalAmount <= 0)
           {
               Stats.health = 0;
               //GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), transform, 0);
               isDead = true;
               OnDeath?.Invoke();
           }
           // Damage hurt
           else
           {
               print($"{gameObject.name} took {totalAmount} DMG, HP {Stats.health}");
               Stats.health += totalAmount;
               GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), transform.position, 0);
               OnHurt?.Invoke();
           }
       }
       // */
    }

    /// <summary>
    /// Modify the current stats on a character
    /// </summary>
    /// <param name="_stat">Which of the character's stat is affected</param>
    /// <param name="_amount">How much to add to that stat</param>
    /// <param name="_direction">The direction in which this effect is coming from (used for detecting damage direction)</param>
    public void ModifyCorruption(float _amount)
    {
        // Corruption Increase
        if (_amount > 0)
        {
            if (corruption + _amount > maxCorruption)
            { 
                corruption = maxCorruption;
                owner.isDead = true;
            }
            else corruption += _amount;
            GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), owner.transform.position, 4);
        }
        
        // Corruption Decrease
        else if (_amount < 0)
        {
            corruption += _amount;
        }
        corruption = Mathf.Clamp(corruption, 0, maxCorruption);
    }
   
    /// <summary>
    /// Modify the power of a character
    /// </summary>
    /// <param name="_amount">Amount added to total power</param>
    public void ModifyPower (int _amount)
    {
        power += _amount;
        GameInstance.Get<GI_WidgetManager> ().SpawnEffectText (_amount.ToString (), owner.transform.position, 5);
        power = Mathf.Clamp (power, 0, maxPower);
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

    #region====================================( Helper Properties )====================================================//
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

    #region==================================( System Integration )======================================================//
    /*-----[ Stat setup functions ]-----------------------------------------------------------------------------------*/
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

    /// <summary>Called upon a creation of a NEW set of stats for a NEW character</summary>
    public void OnNewCharacter()
    {
        health = maxHealth;
        power = 20;
    }

    /*-----[ Save/Load SaveData ]-------------------------------------------------------------------------------------*/
    [Serializable]
    public struct SaveData
    {
        public float health;
        public float level;
        public int power;
        public float corruption;
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

    [StatName("SPD")]     MoveSpeed,

    [StatName("SLD")] ShieldPower
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