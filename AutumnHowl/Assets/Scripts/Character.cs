//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    protected CharacterStats defaultStats = new CharacterStats();
    public CharacterStats currentStats = new CharacterStats();
    public bool isDead;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public event Action OnHurt;
    public event Action OnHeal;
    public event Action OnDeath;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    protected Vector2 movement;
    protected float currentMoveSpeed;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    protected Rigidbody2D _rigidbody;
    [SerializeField] protected Animator animator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual void ModifyHealth(float _amount)
    {
        if (_amount == 0) return;

        if (_amount > 0)
        {
            if (currentStats.health + _amount > currentStats.maxHealth) currentStats.health = currentStats.maxHealth;
            else currentStats.health += _amount;
            OnHeal?.Invoke();
        }
        else if (_amount < 0)
        {
            var totalAmount = _amount + currentStats.defense;
            if (currentStats.health + totalAmount < 0)
            {
                currentStats.health = 0;
                OnDeath?.Invoke();
            }
            else
            {
                currentStats.health += totalAmount;
                OnHurt?.Invoke();
            }
        }
    }


    #endregion
}

[Serializable]
public class CharacterStats
{
    [Header("Base Values")]
    public float health = 100;
    public float level = 0;
    public int power = 5;
    public int corruption = 0;
    [Tooltip("When damage is taken, this is how much damage is negated")]
    public int defense = 0;
    public int attack = 5;
    [Header("Max Values")]
    public float maxHealth = 100;
    public int maxPower = 100;
    public int maxCorruption = 100;
    
    public float walkSpeed;
    public float runSpeed;

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
