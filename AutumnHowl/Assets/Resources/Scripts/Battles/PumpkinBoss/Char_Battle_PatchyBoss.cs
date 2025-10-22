//==========================================( Neverway 2025 )=========================================================//
// Author
//  Liz M.
//
// Contributors
//
//
//====================================================================================================================//

using Neverway.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Char_Battle_PatchyBoss : Char_Battle_BasicAttacker
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/


    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/

    private enum PumpkinState
    {
        SpawnEnemy,
        VineAttack
    }
    private PumpkinState state = PumpkinState.VineAttack;

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/

    [SerializeField] private Char_Battle enemyToSpawn;
    [SerializeField] private GameObject vineEffectPrefab;
    [SerializeField] private GameObject vineWarningPrefab;

    private Coroutine vineRoutine;
    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/

    new private void Start()
    {
        base.Start();
        battleStateController.OnStartWave.AddListener(DoCurrentAttack);
    }

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    private void TakeTurn ()
    {
        /*if (battleStateController.waveStepCount == 0)
        {
            DoCurrentAttack();
        }*/

        if (isDead) battleStateController.NextTurnStep();
        
        SetAttackDamageToCurrentATK();
        var x = gridPawnController.position.x;
        var y = gridPawnController.position.y;
        // If target is in range, attack and end turn.
        switch (GetTarget())
        {
            case Direction.North:
                    TryAttackSequence(AttackSequences[0]);
                    return;
            case Direction.South:
                    TryAttackSequence(AttackSequences[1]);
                    return;
            case Direction.East:
                    TryAttackSequence(AttackSequences[2]);
                    return;
            case Direction.West:
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

    private void DoCurrentAttack()
    {
        switch (state)
        {
            case PumpkinState.SpawnEnemy:
                {
                    print("Patchystate SpawnEnemy");
                    if (vineRoutine != null)
                    {
                        StopCoroutine(vineRoutine);
                    }
                    //spawn 3 pumptims
                    for (int i = 0; i < 3; i++)
                    {
                        SpawnEnemyAtRandomLocation();
                    }
                    state = PumpkinState.VineAttack;
                    break;
                }
            case PumpkinState.VineAttack:
                {
                    print("Patchystate VineAttack");
                    if (vineRoutine != null)
                    {
                        StopCoroutine(vineRoutine);
                    }
                    vineRoutine = StartCoroutine(VineAttackRoutine());
                    state = PumpkinState.SpawnEnemy;
                    break;
                }
        }
    }

    /// <summary>
    /// Coroutine that repeatedly spawns vines on the player's location.
    /// </summary>
    /// <returns></returns>
    private IEnumerator VineAttackRoutine()
    {
        //store the location of Autumn
        var autumn = FindObjectOfType<Char_Battle_Player>();
        if (autumn == null)
        {
            yield break;
        }
        Vector2Int pos = autumn.gridPawnController.position;
        //Spawn hazard signs
        var warnings = SpawnVineWarningsOnPlayer(pos);
        yield return new WaitForSeconds(1.3f);
        //spawn attacks on that same location (but only if the wave is active)
        if (battleStateController.stepsRemaining > 0)
        {
            SpawnVineAttacks(pos);
        }
        //Remove the hazard signs
        foreach(GameObject g in warnings)
        {
            Destroy(g);
        }
        yield return new WaitForSeconds(0.5f);
        if (battleStateController.stepsRemaining > 0)
        {
            vineRoutine = StartCoroutine(VineAttackRoutine());
        }
    }
    /// <summary>
    /// Spawns hazard effects and returns a list of the spawned GameObjects.
    /// </summary>
    /// <returns></returns>
    private List<GameObject> SpawnVineWarningsOnPlayer(Vector2Int pos)
    {
        List<GameObject> vines = new List<GameObject>();
        for (int x = pos.x-1; x < pos.x + 2; x++)
        {
            for (int y = pos.y-1; y < pos.y+2; y++)
            {
                GameObject vineEffect = Instantiate(vineWarningPrefab, battleGrid.gameObject.transform);
                vineEffect.transform.localPosition = new Vector3(x, y, 0);
                vines.Add(vineEffect);
            }
        }

        GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.vineRumble);

        return vines;
    }

    /// <summary>
    /// Spawns vine attacks on given location.
    /// </summary>
    /// <param name="_pos"></param>
    private void SpawnVineAttacks(Vector2Int _pos)
    {
        for (int x = _pos.x - 1; x < _pos.x + 2; x++)
        {
            for (int y = _pos.y - 1; y < _pos.y + 2; y++)
            {
                AttackElement attack = new AttackElement();
                attack.damage = Stats.attack;
                attack.visualEffect = vineEffectPrefab;
                DoAttack(attack, new Vector2Int(x,y));
            }
        }
        GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.vineAttack);
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

    /// <summary>
    /// Spawns an enemy at _position.
    /// </summary>
    /// <param name="_position">Position on the Battle Grid</param>
    private void SpawnEnemy (Vector2Int _position)
    {
        battleStateController.AddCharacter (enemyToSpawn, _position);
    }

    /// <summary>
    /// Picks a random tile to spawn a pump kin at
    /// </summary>
    private void SpawnEnemyAtRandomLocation()
    {
        //Make a list of tiles to try
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        for (int x = 0; x < battleGrid.width; x++) {
            for (int y = 2; y < 5; y++)
            {
                possiblePositions.Add(new Vector2Int(x, y));
            }
        }
        //Pull tiles from the bag until we get one that's not occupied
        RandomBag<Vector2Int> bag = new RandomBag<Vector2Int> (possiblePositions);
        Vector2Int choice = bag.Grab();
        int n = possiblePositions.Count;
        while (battleGrid.IsOccupied(choice.x, choice.y))
        {
            choice = bag.Grab();
            n--;
            if (n == 0)
            {
                //If we somehow run out of tiles, break the loop...
                //(this should only happen if you somehow FILL the screen with punkins)
                break;
            }
        }
        //spawn a boi on the random tile
        SpawnEnemy(choice);
    }

    #endregion
}
