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
    [SerializeField] protected CharacterTemplate template;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public CharacterStats Stats => Identifier.Stats;
    public CharacterIdentifier Identifier { get; private set; }

    [HideInInspector] public bool isDead;

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
    public void Awake()
    {
        //Grabs and assigns the CharacterIdentifier that gives this character its identity!!!
        Identifier = CharacterIdentifier.GetFromCharacterTemplate(template);
    }
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
            if (Stats.health + _amount > Stats.maxHealth) Stats.health = Stats.maxHealth;
            else Stats.health += _amount;
            OnHeal?.Invoke();
        }
        else if (_amount < 0)
        {
            var totalAmount = _amount + Stats.defense;
            if (Stats.health + totalAmount < 0)
            {
                Stats.health = 0;
                OnDeath?.Invoke();
            }
            else
            {
                Stats.health += totalAmount;
                OnHurt?.Invoke();
            }
        }
    }

    #endregion
}