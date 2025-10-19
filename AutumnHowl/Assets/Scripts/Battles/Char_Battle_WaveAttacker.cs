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

public class Char_Battle_WaveAttacker : Char_Battle
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool waveActive;
    private Coroutine waveCoroutine;
    public GameObject projectile;


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    private void FixedUpdate()
    {
        // If the attack wave is still in progress, this will ensure that it is halted when the wave is over
        if (battleStateController.stepsRemaining <= 0 && waveActive)
        {
            StopCoroutine(waveCoroutine);
            waveActive = false;
        }
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    private void TakeTurn()
    {
        if (isDead) battleStateController.NextTurnStep();

        // Only start an attack wave on the first turn for this enemy
        if (!waveActive) { waveCoroutine = StartCoroutine(SpawnWave1()); }
        switch (Stats.health)
        {
            case >= 100:
                break;
            case >= 65:
                break;
            case >= 35:
                break;
        }
        
        // All following turns for this enemy are skipped since it's doing a wave attack
        battleStateController.NextTurnStep();
    }

    /// <summary>
    /// Spawns the specified projectile at the specified grid position and tells it to move in a specified direction
    /// </summary>
    /// <param name="_column">0 is left-most column</param>
    /// <param name="_row">0 is bottom-most row</param>
    /// <param name="direction">The direction to send the projectile</param>
    /// <param name="prefab">The prefab of the projectile to spawn</param>
    private void SpawnProjectile(int _column, int _row, Vector2Int direction, GameObject prefab, float moveDelay = -1f)
    {
        var newProjectile = battleGrid.InstantiatePawn(new Vector2Int(_column, _row), prefab).GetComponent<Char_ProjectileRealtime>();
        newProjectile.moveDirection = direction;
        if (moveDelay != -1) newProjectile.movementDelay = moveDelay;
    }

    private IEnumerator SpawnWave1()
    {
        waveActive = true;
        while (battleStateController.stepsRemaining > 0)
        {
            // Repeat 3 times
            for (int i = 0; i < 3; i++)
            {
                SpawnProjectile(0, 0, Vector2Int.right, projectile);
                yield return new WaitForSeconds(1);
                SpawnProjectile(0, 2, Vector2Int.right, projectile);
                yield return new WaitForSeconds(1);
                SpawnProjectile(0, 4, Vector2Int.right, projectile);
                yield return new WaitForSeconds(2);
            }
        
            yield return new WaitForSeconds(1);
        
            // Repeat 3 times
            for (int i = 0; i < 3; i++)
            {
                SpawnProjectile(0, 0, Vector2Int.up, projectile);
                yield return new WaitForSeconds(1);
                SpawnProjectile(2, 0, Vector2Int.up, projectile);
                yield return new WaitForSeconds(1);
                SpawnProjectile(4, 0, Vector2Int.up, projectile);
                yield return new WaitForSeconds(2);
            }
        }

        waveActive = false;
    }


    /*-----[ External Functions ]-------------------------------------------------------------------------------------*/
    public override void SetTurnActive(bool _isTurnActive)
    {
        canMove = _isTurnActive;
        if (_isTurnActive)
        {
            TakeTurn();
        }
    }


    #endregion
}
