using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character meant to represent a projectile.
/// It periodically attempts to move, and if it's blocked, attempts it's attack and then dies.
/// </summary>
public class Char_ProjectileRealtime : Char_Battle
{

    #region Fields

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public bool ignoreObstacles;
    public bool destroyOnHit = true;
    public float movementDelay = 0.7f;
    public Vector2Int moveDirection = Vector2Int.zero;
    public List<Vector2Int> additionalCollisionPoints = new List<Vector2Int>();
    [SerializeField] private AttackSequence attackSequence;
    /*----------------------------------------------------------------------------------------------------------------*/

    #endregion

    #region Monobehaviour

    new void Start ()
    {
        base.Start ();
        OnHurt += ReverseProjectile;
        StartCoroutine(MoveOnTimer());
    }

    #endregion

    #region Char_ProjectileRealtime

    /// <summary>
    /// Recurring projectile movement routine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveOnTimer ()
    {
        yield return new WaitForSeconds(movementDelay);
        
        foreach (var collisionPoint in additionalCollisionPoints)
        {
            if (battleGrid.GetIsOccupied(gridPawnController.position + collisionPoint))
            {
                //If we failed to move, damage what's in front of us
                TryAttackSequence(attackSequence, shouldProgressTurn:false);
        
                //Kill the projectile
                if (destroyOnHit)
                {
                    Kill ();
                }
                yield break;
            }
        }
        
        //Try to move
        if (TryMoveInDirection (moveDirection, doNextTurn:false, ignoreObsticals:ignoreObstacles, shouldProgressTurn:false) == false)
        {
            print("RAT CORE, VALUE IS " + moveDirection);
            //If we failed to move, damage what's in front of us
            attackSequence.attacks[0].position = moveDirection; // 0, 1
            TryAttackSequence(attackSequence, shouldProgressTurn:false);
        
            //Kill the projectile
            if (destroyOnHit)
            {
                Kill ();
            }
            yield break;
        }
        // Test additional collisions
        else
        {
        }
        
        StartCoroutine(MoveOnTimer ());
    }

    private void ReverseProjectile ()
    {
        //Turn the projectile around when it's hit
        moveDirection = -moveDirection;
        foreach (var attack in attackSequence.attacks)
        {
            attack.position = attack.position * -1;
        }
        StopAllCoroutines ();
        StartCoroutine (MoveOnTimer ());
    }

    #endregion

}
