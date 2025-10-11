//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public CharacterTemplate template;
    public CharacterStats currentStats = new CharacterStats();
    public bool isDead;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    [HideInInspector] public CharacterIdentifier identifier;
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
        currentStats.LinkCharacterStatsToCharacter(this);
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
            GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), gameObject.transform, 1);
            OnHeal?.Invoke();
        }
        else if (_amount < 0)
        {
            var totalAmount = _amount + currentStats.defense;
            if (currentStats.health + totalAmount < 0)
            {
                currentStats.health = 0;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), gameObject.transform, 0);
                isDead = true;
                OnDeath?.Invoke();
            }
            else
            {
                currentStats.health += totalAmount;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), gameObject.transform, 0);
                OnHurt?.Invoke();
            }
        }
    }

    #endregion
}