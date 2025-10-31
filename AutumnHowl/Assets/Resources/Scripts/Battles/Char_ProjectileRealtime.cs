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
    private float timeOfLastReflect;//For checking how long it's been since projectile deflected.
    private BattleGrid battleGrid;
    /*----------------------------------------------------------------------------------------------------------------*/

    #endregion

    #region Monobehaviour

    new void Start ()
    {
        base.Start ();
        battleGrid = FindObjectOfType<BattleGrid>();
        //OnHurt += ReverseProjectile;
        StartCoroutine(MoveOnTimer());
    }

    new private void Update ()
    {
        base.Update ();
        if (BattleGrid.playerAttackPositions.Contains (gridPawnController.position))
        {
            ReverseProjectile ();
        }
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
            if (BattleGrid.Instance.IsOccupied(gridPawnController.position + collisionPoint))
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

    //Reverses projectile
    private void ReverseProjectile ()
    {
        //Don't reverse if we've been reversed within the last .3 seconds
        if (Time.timeSinceLevelLoad - timeOfLastReflect > 0.5f)
        {
            timeOfLastReflect = Time.timeSinceLevelLoad;
        }
        else
        {
            return;
        }
        //Turn the projectile around when it's hit
        print ("ReverseProjectile");
        moveDirection = -moveDirection;
    }

    #endregion

}
