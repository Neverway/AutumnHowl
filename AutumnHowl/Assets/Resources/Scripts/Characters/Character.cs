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
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] protected CharacterTemplate template;
    [Tooltip("If this is true and this character takes DMG, the DMG will be reduced by the characters current defense, down to the limit of zero")]
    public bool isDefenseActive;

    public Vector2Int startFaceDirection = Vector2Int.down;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public CharacterStats Stats => Identifier.Stats;
    public CharacterIdentifier Identifier { get; private set; }

    [HideInInspector] public bool isDead;

    public event Action OnHurt;
    public event Action OnHeal;
    public event Action OnDeath;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    public Vector2 movement;
    public Vector2 facingDirection;
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
        if (this is IsPlayerCharacter)
        {
            GameInstance.Playerbody = this;
            GameInstance.Gamestate.player = Identifier;
        }
        GameInstance.Get<GI_CharacterReferencer>().Register(this);
        UpdateGameStateValues();
        Stats.owner = this;
    }
    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (animator == null )
        {
            return;
        }
        animator.SetFloat("idleX", startFaceDirection.x);
        animator.SetFloat("idleY", startFaceDirection.y);
    }
    public void OnDestroy()
    {
        try { GameInstance.Get<GI_CharacterReferencer>().UnRegister(this); } catch { }
    }
    public void LateUpdate() => UpdateGameStateValues();


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    private void UpdateGameStateValues()
    {
        if (this is not IsPlayerCharacter) return;
        // Transfer player data to game state
        if (gameState != null)
        {
            gameState.currentGameState.playtime += Time.deltaTime;
        }
        else gameState = GameInstance.Get<GI_AuHoGameState>();
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual void ModifyHealth(float _amount, Vector2Int direction = new Vector2Int())
    {

        // Character healed
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
    }

    public float GetHealth ()
    {
        return Stats.health;
    }

    public bool HasTag (CharacterTags _tag)
    {
        if (template.characterTags.Contains(_tag)) return true;
        return false;
    }

    public virtual void ModifyPower(int _amount)
    {
        if (_amount == 0) return;

        // Power Increase
        if (_amount > 0)
        {
            if (Stats.power + _amount > Stats.maxPower) Stats.power = Stats.maxPower;
            else Stats.power += _amount;
            GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), transform.position, 3);
        }
        
        // Power Decrease
        else if (_amount < 0)
        {
            // Clamp to minimum value of 0
            if (Stats.power + _amount < 0)
            {
                Stats.power = 0;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), transform.position, 3);
            }
            // Subtract amount
            else
            {
                Stats.power += _amount;
                GameInstance.Get<GI_WidgetManager>().SpawnEffectText(_amount.ToString(), transform.position, 3);
            }
        }
    }

    public void DoAttackAnimation()
    {
        if (animator == null) return;
        StartCoroutine (AttackAnimationRoutine ());
    }

    public IEnumerator AttackAnimationRoutine ()
    {
        print ("Set Animator Attacking");
        animator.SetBool ("attacking", true);
        yield return new WaitForSeconds (0.2f);
        animator.SetBool ("attacking", false);
    }

    #endregion
}