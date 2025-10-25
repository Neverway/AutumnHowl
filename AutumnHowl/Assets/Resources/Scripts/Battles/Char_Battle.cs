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
using static GameFeatureConstants.Battle;

public abstract class Char_Battle : Character
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public List<AttackSequence> AttackSequences;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public bool canMove;
    //If true, this character can be moved by "pushing" attacks
    public bool pushable;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/

    //unique identifier for a stat mod
    private const string Mod_ConditionalBlock = "ConditionalBlock";

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GridPawn gridPawnController;
    public BattleGridPather gridPather;
    public BattleGrid battleGrid;
    public BattleStateController battleStateController;
    //If this is not null, this object gets spawned when the character dies.
    public GameObject spawnOnDeath;
    public bool useBlock = false;
    //This is so we can invert the direction the pawn faces when it attacks (currently only used for the Player pawn).
    [HideInInspector] public bool invertAttackFacingDirections = false;

    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public override void Start()
    {
        base.Start();
        gridPather = FindObjectOfType<BattleGridPather>();
        battleGrid = FindObjectOfType<BattleGrid>();
        battleStateController = FindObjectOfType<BattleStateController>();
        OnDeath += Kill;
    }
    
    public void Update()
    {
        if (isDead) return;
        if (animator == null) return;

        animator.SetFloat("idleX", facingDirection.x);
        animator.SetFloat("idleY", facingDirection.y);
        if (!canMove) return;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    /// <summary>
    /// Tests if the character can move to a tile, and returns true if it was able to move.
    /// </summary>
    /// <param name="_direction">Tile to move to; relative to current position.</param>
    /// <param name="doNextTurn">Set to false if this object shouldn't trigger NextTurnStep, for example if it moves in realtime.</param>
    /// <returns></returns>
    protected virtual bool TryMoveInDirection (Vector2Int _direction, bool doNextTurn = true, GridPawn _pathTargetPawn = null, bool shouldProgressTurn = true, bool ignoreObsticals=false)
    {
        var testPos = gridPawnController.position + _direction;
        if (BattleGrid.Instance.ValidTile (testPos.x, testPos.y) && !BattleGrid.Instance.IsOccupied(testPos.x, testPos.y))
        {
            gridPawnController.MoveToTile (testPos.x, testPos.y);
            if (doNextTurn)
            {
                if (_pathTargetPawn != null)
                {
                    gridPather.GetPathToTarget (gridPawnController);
                }
                if (shouldProgressTurn) battleStateController.NextTurnStep();
            }
            return true;
        }

        return false;
    }
    
    protected virtual bool TryMoveTo(Vector2Int _direction)
    {
        
        print($"{gameObject.name} - try move called");
        if (BattleGrid.Instance.ValidTile (_direction.x, _direction.y) && !BattleGrid.Instance.IsOccupied(_direction.x, _direction.y))
        {
            print($"{gameObject.name} - try move success");
            gridPawnController.MoveToTile (_direction.x, _direction.y);
            battleStateController.NextTurnStep(caller:gameObject.name);
            return true;
        }
        print($"{gameObject.name} - try move failure");

        return false;
    }

    public IEnumerator CoTryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false, bool shouldProgressTurn = true)
    {
        //when hasStopped is true, it stops the rest of the sequence from firing.
        var hasStopped = false;
        
        for (int i = 0; i < attackSequence.attacks.Count; i++)
        {                
            // Applied position is the position offset after mirroring has been applied
            var appliedPosition = attackSequence.attacks[i].position;
            if (mirrorX)
            {
                appliedPosition.x = -attackSequence.attacks[i].position.x;
            }
            if (mirrorY) appliedPosition.y = -attackSequence.attacks[i].position.y;
            
            var currentPosition = gridPawnController.position + appliedPosition;

            if (invertAttackFacingDirections)
            {
                facingDirection = -attackSequence.attacks[i].position;
            }
            else
            {
                facingDirection = attackSequence.attacks[i].position;
            }

                DoAttack(attackSequence.attacks[i], currentPosition);
            GridPawn target = battleGrid.GetIsOccupied(currentPosition);
            if (target)
            {
                if (target.type == GridPawn.GridPawnType.obstacle)
                {
                    print($"Found obstcl at {appliedPosition}");
                    hasStopped = true;
                    GI_AudioManager.Instance.PlayClip (GI_AudioManager.Instance.hitBounce);
                }
                else if (target.type == GridPawn.GridPawnType.character)
                {
                    print($"Found char {target.gameObject.name} at {appliedPosition}");
                    var char_Battle = target.GetComponent<Char_Battle> ();
                    if (char_Battle.GetHealth () <= 0)
                    {
                        GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.hitKill);
                    }
                    else
                    {
                        //Stop the attack because we hit something and didn't kill it.
                        hasStopped = true;
                        GI_AudioManager.Instance.PlayClip (GI_AudioManager.Instance.hitDamage);
                    }
                }
                else if (target.type == GridPawn.GridPawnType.attack)
                {
                    print($"Found attack at {appliedPosition}");
                    hasStopped = true;
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            //If we bonked something, go back to the last cardinal direction
            if (hasStopped)
            {
                int dir = i;
                if (dir > 0)
                {
                    dir--;
                }
                while (dir > 0 && dir % 2 != 0) //assuming cardinal direction = even numbers
                {
                    dir--;
                }
                if (invertAttackFacingDirections)
                {
                    facingDirection = -attackSequence.attacks[dir].position;
                }
                else
                {
                    facingDirection = attackSequence.attacks[i].position;
                }
                    break;
            }
        }
        if (shouldProgressTurn) battleStateController.NextTurnStep(0.5f);
    }

    /// <summary>
    /// executes the attack on the specified grid tile
    /// </summary>
    /// <param name="attack"></param>
    public void DoAttack(AttackElement attack, Vector2Int _position)
    {
        Instantiate(attack.visualEffect, battleGrid.transform.position + new Vector3(_position.x, _position.y, 0), new Quaternion(), null);
        GridPawn target = battleGrid.GetIsOccupied(_position);
        if (target == null)
        {
            return;
        }
        Char_Battle char_Battle = target.GetComponent<Char_Battle>();
        //deal damage
        char_Battle.ApplyConditionalBlock(attack.direction);
        char_Battle.ModifyHealth(-attack.damage);
        char_Battle.RemoveConditionalBlock();
        //check if we should push the target
        if (char_Battle.pushable && attack.pushing)
        {
            char_Battle.TryMoveInDirection(attack.direction, false);
        }
    }
    
    /// <summary>
    /// Applies a defense modifier, but only if blockDirection blocks the attack.
    /// </summary>
    /// <param name="direction"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ApplyConditionalBlock (Vector2Int direction)
    {
        if (useBlock == false)
        {
            return;
        }
        //Autumn blocks in reverse, shield guy doesn't XD
        if (invertAttackFacingDirections)
        {
            if (facingDirection != direction)
            {
                return;
            }
        }
        else
        {
            if (facingDirection != -direction)
            {
                return;
            }
        }
        print("ConditionalBlock activated");
            Stats.defense.ModifyStatWith(Mod_ConditionalBlock, BLOCKDEFENSETYPE, BLOCKDEFENSEMOD);
    }
    
    /// <summary>
    /// Removes the defense modifier applied by ApplyConditionalBlock.
    /// </summary>
    private void RemoveConditionalBlock ()
    {
        Stats.defense.UnmodifyStatWith (Mod_ConditionalBlock);
    }


    public virtual void TryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false, bool shouldProgressTurn = true)
    {
        StartCoroutine(CoTryAttackSequence(attackSequence, mirrorX, mirrorY, shouldProgressTurn));
    }

    /// <summary>
    /// Triggered by OnDeath; Spawns spawnOnDeath if it exists.
    /// </summary>
    public virtual void Kill ()
    {
        if (spawnOnDeath != null)
        {
            GameObject g = Instantiate (spawnOnDeath);
            g.transform.position = transform.position;
        }
        if (this is IsPlayerCharacter) return;
        FindObjectOfType<BattleStateController> ().RemoveCharacter (this);
        Destroy (gameObject);
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public virtual void SetTurnActive(bool _isTurnActive)
    {
        canMove = _isTurnActive;
    }

    public void SetAttackDamageToCurrentATK()
    {
        for (int i = 0; i < AttackSequences.Count; i++)
        {
            for (int j = 0; j < AttackSequences[i].attacks.Count; j++)
            {
                AttackSequences[i].attacks[j].damage = Stats.attack;
            }
        }
    }


    #endregion
}

[Serializable]
public class AttackElement
{
    //The position to attack on the BattleGrid
    public Vector2Int position;
    //amount of damage dealt to enemy
    public float damage;
    //object that spawns on the attack's tile
    public GameObject visualEffect;
    //direction the attack is moving.
    public Vector2Int direction = new Vector2Int(0, 0);
    //if true, this attack can push the target.
    public bool pushing = false;
}       

[Serializable]  
public class AttackSequence
{
     public List<AttackElement> attacks;
}
