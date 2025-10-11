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


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public override void Start()
    {
        base.Start();
        gridPather = FindObjectOfType<BattleGridPather>();
        battleGrid = FindObjectOfType<BattleGrid>();
        battleStateController = FindObjectOfType<BattleStateController>();
    }
    
    private void Update()
    {
        if (isDead) return;
        if (!canMove) return;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    protected virtual bool TryMoveInDirection(Vector2Int _direction)
    {
        var testPos = gridPawnController.position + _direction;
        if (BattleGrid.Instance.ValidTile (testPos.x, testPos.y) && !BattleGrid.Instance.IsOccupied(testPos.x, testPos.y))
        {
            gridPawnController.MoveToTile (testPos.x, testPos.y);
            battleStateController.NextTurnStep();
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
            if (hasStopped) continue;
                
            // Applied position is the position offset after mirroring has been applied
            var appliedPosition = attackSequence.attacks[i].position;
            if (mirrorX)
            {
                appliedPosition.x = -attackSequence.attacks[i].position.x;
            }
            if (mirrorY) appliedPosition.y = -attackSequence.attacks[i].position.y;
            
            var currentPosition = gridPawnController.position + appliedPosition;
            
            Instantiate(attackSequence.attacks[i].visualEffect, battleGrid.transform.position+new Vector3(currentPosition.x, currentPosition.y, 0), new Quaternion(), null);
            
            var target = battleGrid.GetIsOccupied(currentPosition);
            if (target)
            {
                if (target.type == GridPawn.GridPawnType.obstacle)
                {
                    print($"Found obstcl at {appliedPosition}");
                    hasStopped = true;
                }
                else if (target.type == GridPawn.GridPawnType.character)
                {
                    print($"Found char {target.gameObject.name} at {appliedPosition}");
                    target.GetComponent<Char_Battle>().ModifyHealth(-attackSequence.attacks[i].damage);
                }
                else if (target.type == GridPawn.GridPawnType.attack)
                {
                    print($"Found attack at {appliedPosition}");
                    hasStopped = true;
                }
            }

            
            yield return new WaitForSeconds(0.1f);
        }
        battleStateController.NextTurnStep(0.5f);
    }

    public virtual void TryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false)
    {
        StartCoroutine(CoTryAttackSequence(attackSequence, mirrorX, mirrorY));
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
