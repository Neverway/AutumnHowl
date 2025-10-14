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
    [Tooltip("If this is true and this character takes DMG, the DMG will be reduced by the characters current defense, down to the limit of zero")]
    public bool isDefenseActive;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public CharacterStats Stats => Identifier.Stats;
    public CharacterIdentifier Identifier { get; private set; }

    [HideInInspector] public bool isDead;

    public event Action OnHurt;
    public event Action OnHeal;
    public event Action OnDeath;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    public Vector2 movement;
    protected float currentMoveSpeed;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    protected Rigidbody2D _rigidbody;
    protected GI_AuHoGameState gameState;
    [SerializeField] protected Animator animator;


    #endregion


    #region=======================================( Functions )======================================================= //

    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public void Awake()
    {
        //Grabs and assigns the CharacterIdentifier that gives this character its identity!!!
        Identifier = CharacterIdentifier.GetFromCharacterTemplate(template);
        UpdateGameStateValues();
    }
    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void LateUpdate()
    {
        UpdateGameStateValues();
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateGameStateValues()
    {
        if (this is not IsPlayerCharacter) return;
        // Transfer player data to game state
        if (gameState != null)
        {
            gameState.currentGameState.playtime += Time.deltaTime;
            gameState.currentGameState.player = Identifier;
        }
        else gameState = GameInstance.Get<GI_AuHoGameState>();
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual void ModifyHealth(float _amount)
    {
        if (_amount == 0) return;

        // Character healed
        if (_amount > 0)
        {
            if (Stats.health + _amount > Stats.maxHealth) Stats.health = Stats.maxHealth;
            else Stats.health += _amount;
            GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), transform, 1);
            OnHeal?.Invoke();
        }
        
        // Character damaged
        else if (_amount < 0)
        {
            var totalAmount = _amount;
            
            // Apply defense if active
            if (isDefenseActive)
            {
                totalAmount = _amount + Stats.defense;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(Stats.defense.ToString(), transform, 2);
            }
            GameInstance.Get<GI_WidgetManager>().SpawnEffectText(totalAmount.ToString(), transform, 0);
            
            // Damage killed
            if (Stats.health + totalAmount < 0)
            {
                Stats.health = 0;
                OnDeath?.Invoke();
            }
            // Damage hurt
            else
            {
                Stats.health += totalAmount;
                OnHurt?.Invoke();
            }
        }
    }

    #endregion
}