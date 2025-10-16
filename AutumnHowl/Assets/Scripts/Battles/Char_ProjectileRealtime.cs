using System.Collections;
using UnityEngine;

/// <summary>
/// Character meant to represent a projectile.
/// It periodically attempts to move, and if it's blocked, attempts it's attack and then dies.
/// </summary>
public class Char_ProjectileRealtime : Char_Battle
{

    #region Fields

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private float movementDelay = 0.7f;
    [SerializeField] private Vector2Int moveDirection = Vector2Int.down;
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
        //Try to move
        if (TryMoveInDirection (moveDirection, false) == false)
        {
            //If we failed to move, damage what's in front of us
            TryAttackSequence (attackSequence);
            //Kill the projectile
            Kill ();
            yield break;
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
