//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Battle_PatchyBoss : Char_Battle_BasicAttacker
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/

    [SerializeField] private Char_Battle enemyToSpawn;

    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    private void TakeTurn ()
    {
        //spawn a boi on the first step of each wave
        if (battleStateController.waveStepCount == 0)
        {
            SpawnEnemy (new Vector2Int (0, 4));
        }

        if (isDead) battleStateController.NextTurnStep();
        
        SetAttackDamageToCurrentATK();
        var x = gridPawnController.position.x;
        var y = gridPawnController.position.y;
        // If target is in range, attack and end turn.
        switch (GetTarget())
        {
            case "north":
                    TryAttackSequence(AttackSequences[0]);
                    return;
            case "south":
                    TryAttackSequence(AttackSequences[1]);
                    return;
            case "east":
                    TryAttackSequence(AttackSequences[2]);
                    return;
            case "west":
                    TryAttackSequence(AttackSequences[3]);
                    return;
        }

        //Try to move side to side to follow player
        var autumn = FindObjectOfType<Char_Battle_Player> ();
        Vector2Int move = new Vector2Int (0, 0);
        if (autumn.gridPawnController.position.x > gridPawnController.position.x)
        {
            move = new Vector2Int (1, 0);
        }
        if (autumn.gridPawnController.position.x < gridPawnController.position.x)
        {
            move = new Vector2Int (-1, 0);
        }


        if ( move == Vector2Int.zero || !TryMoveTo(gridPawnController.position+move) )
        {
            battleStateController.NextTurnStep();
        }
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

    private void SpawnEnemy (Vector2Int _position)
    {
        battleStateController.AddCharacter (enemyToSpawn, _position);
    }


    #endregion
}
