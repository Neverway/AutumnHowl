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

public class Char_Battle_Tutorial : Char_Battle
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/
    private bool waveActive;
    private Coroutine waveCoroutine;
    public int currentWave;
    public GameObject dummyPrefab, boulderPrefab;
    public Char_Battle currentDummy, currentBoulder;

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/


    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public override void Start()
    {
        base.Start();
        battleStateController.SetAvailablePlayerActions(
            canUseAttack: true,
            canUseItem: false,
            canUseSpell: false, 
            canUseDefend: false
        );
        
        battleStateController.OnStartWave.AddListener(() => OnStartWave());
        
        GameInstance.Gamestate.currentBattleWave = 0;

    }

    public new void Update()
    {
        if (waveActive)
        {
            // Check for wave goal completion
            // Wave 0 - Learn to attack - Dummy damaged
            if (currentWave == 0)
            {
                if (currentDummy == null || currentDummy.Stats.health < currentDummy.Stats.maxHealth)
                {
                    Debug.Log("WAVE 0 GOAL");
                    waveActive = false;
                    GameInstance.Gamestate.currentBattleWave = 1;
                    currentWave = 1;
                    // Set next turn actions to defend only
                    battleStateController.SetAvailablePlayerActions(
                        canUseAttack: false,
                        canUseItem: false,
                        canUseSpell: false,
                        canUseDefend: true
                    );
                    battleStateController.ChangeState(new BS_EarlyExitFromBattleGrid(battleStateController));
                }
            }
            // Wave 1 - Learn to defend and restore strength - Just wait for the turn steps to complete
            else if (currentWave == 1)
            {
                if (battleStateController.stepsRemaining == 0)
                {
                    Debug.Log("WAVE 1 GOAL");
                    waveActive = false;
                    GameInstance.Gamestate.currentBattleWave = 2;
                    currentWave = 2;
                    // Set next turn actions to attack/defend
                    battleStateController.SetAvailablePlayerActions(
                        canUseAttack: true,
                        canUseItem: false,
                        canUseSpell: false,
                        canUseDefend: true
                    );
                    //battleStateController.ChangeState(new BS_EarlyExitFromBattleGrid(battleStateController));
                }
            }
            // Wave 2 - Learn to move - Dummy damaged
            else if (currentWave == 2)
            {
                Debug.Log("CHECKING WAVE 2 STATE" + (currentDummy == null) + " | " + currentDummy.Stats.health);
                if (currentDummy == null || currentDummy.Stats.health < currentDummy.Stats.maxHealth)
                {
                    Debug.Log("WAVE 2 GOAL");
                    waveActive = false;
                    GameInstance.Gamestate.currentBattleWave = 3;
                    currentWave = 3;
                    // Set next turn actions to attack/defend
                    battleStateController.SetAvailablePlayerActions(
                        canUseAttack: true,
                        canUseItem: false,
                        canUseSpell: false,
                        canUseDefend: true
                    );
                    battleStateController.ChangeState(new BS_EarlyExitFromBattleGrid(battleStateController));
                }
            }
            // Wave 3 - Learn to obstacle - Dummy damaged
            else if (currentWave == 3)
            {
                if (currentDummy == null || currentDummy.Stats.health < currentDummy.Stats.maxHealth)
                {
                    Debug.Log("WAVE 3 GOAL");
                    waveActive = false;
                    GameInstance.Gamestate.currentBattleWave = 4;
                    currentWave = 4;
                    // Set next turn actions to all enabled
                    battleStateController.SetAvailablePlayerActions(
                        canUseAttack: true,
                        canUseItem: true,
                        canUseSpell: true,
                        canUseDefend: true
                    );
                    battleStateController.ChangeState(new BS_EarlyExitFromBattleGrid(battleStateController));
                }
            }
        }
    }
    

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    private void OnStartWave()
    {
        var autumn = GameInstance.Playerbody as Char_Battle_Player;
        // ========== [Wave 0] - Learn to attack ==========
        if (currentWave == 0)
        {
            // Reset player location
            autumn.gridPawnController.MoveToTile(2, 1);
            // Spawn a dummy
            //if (currentDummy) Destroy(currentDummy);
            if (!currentDummy) currentDummy = SpawnBattlePawn(dummyPrefab, 2, 2);
            // Await dummy hurt
        }
        // ========== [Wave 1] - Learn to defend ==========
        else if (currentWave == 1)
        {
            // Reset player location
            autumn.gridPawnController.MoveToTile(2, 1);
            currentDummy.spawnOnDeath = null;
            currentDummy.Kill();
        }
        // ========== [Wave 2] - Learn to move ==========
        else if (currentWave == 2)
        {
            // Reset player location
            autumn.gridPawnController.MoveToTile(2, 1);
            // Spawn a dummy far away
            if (!currentDummy) currentDummy = SpawnBattlePawn(dummyPrefab, 2, 3);
            // Await dummy hurt
        }
        // ========== [Wave 3] - Learn to obstacle ==========
        else if (currentWave == 3)
        {
            // Reset player location
            autumn.gridPawnController.MoveToTile(2, 1);
            // Spawn a dummy
            currentDummy.spawnOnDeath = null;
            currentDummy.Kill();
            if (!currentDummy) currentDummy = SpawnBattlePawn(dummyPrefab, 2, 2);
            // Spawn a boulder
            if (currentBoulder) Destroy(currentBoulder);
            if (!currentBoulder) currentBoulder = SpawnBattlePawn(boulderPrefab, 1, 2);
            // Await dummy hurt
        }
        waveActive = true;
    }

    private void TakeTurn()
    {
        if (isDead) battleStateController.NextTurnStep();
        

        // All following turns for this enemy are skipped since it's doing a wave attack
        battleStateController.NextTurnStep();
    }

    private Char_Battle SpawnBattlePawn(GameObject _prefab, int _column, int _row)
    {
        var battlePawn = battleGrid.InstantiatePawn(new Vector2Int(_column, _row), _prefab).GetComponent<Char_Battle>();

        return battlePawn;
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
