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

public abstract class Char_Battle : Character
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public List<AttackSequence> AttackSequences;


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public bool canMove;


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GridPawn gridPawnController;
    public BattleGridPather gridPather;
    public BattleGrid battleGrid;
    public BattleStateController battleStateController;
    //If this is not null, this object gets spawned when the character dies.
    public GameObject spawnOnDeath;


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public override void Start()
    {
        print ("START " + gameObject.name);
        base.Start();
        gridPather = FindObjectOfType<BattleGridPather>();
        battleGrid = FindObjectOfType<BattleGrid>();
        battleStateController = FindObjectOfType<BattleStateController>();
        OnDeath += Kill;
    }
    
    private void Update()
    {
        if (isDead) return;
        if (!canMove) return;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    /// <summary>
    /// Tests if the character can move to a tile, and returns true if it was able to move.
    /// </summary>
    /// <param name="_direction">Tile to move to; relative to current position.</param>
    /// <param name="doNextTurn">Set to false if this object shouldn't trigger NextTurnStep, for example if it moves in realtime.</param>
    /// <returns></returns>
    protected virtual bool TryMoveInDirection (Vector2Int _direction, bool doNextTurn = true)
    {
        var testPos = gridPawnController.position + _direction;
        if (BattleGrid.Instance.ValidTile (testPos.x, testPos.y) && !BattleGrid.Instance.IsOccupied(testPos.x, testPos.y))
        {
            gridPawnController.MoveToTile (testPos.x, testPos.y);
            if (doNextTurn)
            {
                battleStateController.NextTurnStep();
            }
            return true;
        }

        return false;
    }
    
    protected virtual bool TryMoveTo(Vector2Int _direction)
    {
        if (BattleGrid.Instance.ValidTile (_direction.x, _direction.y) && !BattleGrid.Instance.IsOccupied(_direction.x, _direction.y))
        {
            gridPawnController.MoveToTile (_direction.x, _direction.y);
            battleStateController.NextTurnStep();
            return true;
        }

        return false;
    }

    public IEnumerator CoTryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false)
    {
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

            movement = -attackSequence.attacks[i].position;
            
            Instantiate(attackSequence.attacks[i].visualEffect, battleGrid.transform.position+new Vector3(currentPosition.x, currentPosition.y, 0), new Quaternion(), null);
            
            var target = battleGrid.GetIsOccupied(currentPosition);
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
                    char_Battle.ModifyHealth(-attackSequence.attacks[i].damage);
                    if (char_Battle.GetHealth () <= 0)
                    {
                        GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.hitKill);
                    }
                    else
                    {
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
                movement = -attackSequence.attacks[dir].position;
                break;
            }
        }
        battleStateController.NextTurnStep(0.5f);
    }

    public virtual void TryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false)
    {
        StartCoroutine(CoTryAttackSequence(attackSequence, mirrorX, mirrorY));
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
    public Vector2Int position;
    public float damage;
    public GameObject visualEffect;
}       

[Serializable]  
public class AttackSequence
{
     public List<AttackElement> attacks;
}
