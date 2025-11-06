using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_ShieldUser : Char_Battle_BasicAttacker
{
    //determines if the character plans to rotate their shield to block next turn
    private bool shieldNextTurn = false;
    private Vector2Int lastAttackDirection;
    public override void OnAttacked(AttackElement attack)
    {
        base.OnAttacked(attack);
        if (attack.direction != -facingDirection)
        {
            shieldNextTurn = true;
            lastAttackDirection = attack.direction;
        }
    }

    override public void TakeTurn()
    {
        //If the player got around the shield, rotate to try and block from that direction.
        if (shieldNextTurn)
        {
            facingDirection = -lastAttackDirection;
            battleStateController.NextTurnStep();
            shieldNextTurn = false;
            return;
        }
        base.TakeTurn();
    }
}
