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

public class Char_Battle_Beast : Char_Battle_BasicAttacker
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public int realTimeAttacksTilBurnout = 5;
    public int currentRealTimeAttacksTilBurnout = 5;
    public GameObject spriteObject; //The sprite of the boss graphics.
    public Vector3 spriteDefaultPosition; //Position that the sprite is sent to, separate from the actual boss position.

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/


    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/

    
    private enum BossState
    {
        EchoAttack, // Bark then send fast echo projectiles down columns (Reflectable)
        EchoAttackTurbo,
        SpawnEnemy, // Spawn a bunch of specters
        BurntOut,
        ClawSwipeAttack, // Swipe paws down rows (Damageable)
        LaneRushAttack, // Bark then charge down column (Damageable)
        CorruptionPillarAttack, // Spawn a bunch of pillars that do corruption damage randomly
        SuperAttack, // Do a fast combo of all moves in one turn
    }
    private BossState state = BossState.EchoAttack;

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    [SerializeField] private Char_Battle enemyToSpawn;
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private GameObject echoEffectPrefab;
    [SerializeField] private GameObject echoProjectile;

    private Coroutine echoCoroutine;
    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    new private void Start()
    {
        base.Start();
        battleStateController.OnStartWave.AddListener(DoCurrentAttack);
    }

    new private void Update ()
    {
        spriteObject.transform.position = spriteDefaultPosition;
        base.Update ();
    }

    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/
    new private void TakeTurn ()
    {
        if (isDead) battleStateController.NextTurnStep();

        //I've disabled the normal attacks, they were accessing unassigned Attack Sequences. -Connor

        /*
        SetAttackDamageToCurrentATK();
        var x = gridPawnController.position.x;
        var y = gridPawnController.position.y;
        
        // If target is in range, attack.
        DoAttackAnimation ();
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
        }*/

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


        if (move == Vector2Int.zero || !TryMoveTo (gridPawnController.position + move))
        {
            battleStateController.NextTurnStep ();
        }
    }

    private void DoCurrentAttack()
    {
        if (echoCoroutine != null)
        {
            StopCoroutine(echoCoroutine);
        }
        switch (state)
        {
            case BossState.EchoAttack:
            {
                animator.Play("Beast_Idle");
                currentRealTimeAttacksTilBurnout = realTimeAttacksTilBurnout;
                echoCoroutine = StartCoroutine(EchoAttackCoroutine());
                state = BossState.SpawnEnemy;
                break;
            }
            case BossState.SpawnEnemy:
            {
                //spawn X enemies
                for (int i = 0; i < 5; i++)
                {
                    SpawnEnemyAtRandomLocation();
                }
                state = BossState.EchoAttackTurbo;
                break;
            }
            case BossState.EchoAttackTurbo:
            {
                currentRealTimeAttacksTilBurnout = realTimeAttacksTilBurnout*2;
                echoCoroutine = StartCoroutine(EchoAttackCoroutine(0.0f, 0.0f));
                state = BossState.BurntOut;
                break;
            }
            case BossState.BurntOut:
            {
                animator.Play("Beast_BurntOut");
                state = BossState.EchoAttack;
                break;
            }
        }
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

    /// <summary>
    /// Coroutine that repeatedly spawns vines on the player's location.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EchoAttackCoroutine(float delayBeforeAttack = 1.3f, float delayBetweenAttacks = 0.5f)
    {
        // Store the location of Autumn
        var autumn = FindObjectOfType<Char_Battle_Player>();
        if (autumn == null)
        {
            yield break;
        }
        Vector2Int pos = autumn.gridPawnController.position;
        
        // Spawn hazard signs
        var warnings = SpawnEchoWarningsOnPlayer(pos);
        yield return new WaitForSeconds(delayBeforeAttack);
        
        //spawn attacks on that same location (but only if the wave is active)
        if (battleStateController.stepsRemaining > 0)
        {
            animator.Play("Beast_Bark");
            SpawnEchoAttacks(pos);
        }
        
        // Remove the hazard signs
        foreach(GameObject g in warnings)
        {
            Destroy(g);
        }
        yield return new WaitForSeconds(delayBetweenAttacks);
        if (battleStateController.stepsRemaining > 0 && currentRealTimeAttacksTilBurnout > 0)
        {
            currentRealTimeAttacksTilBurnout--;
            echoCoroutine = StartCoroutine(EchoAttackCoroutine());
        }
    }
    
    /// <summary>
    /// Spawns hazard effects and returns a list of the spawned GameObjects.
    /// </summary>
    /// <returns></returns>
    private List<GameObject> SpawnEchoWarningsOnPlayer(Vector2Int pos)
    {
        List<GameObject> warnings = new List<GameObject>();
        for (int y = 0; y < battleGrid.height; y++)
        {
            for (int x = pos.x-2; x < pos.x+3; x+=2)
            {
                if (x > -1 && x < battleGrid.width)
                {
                    GameObject echoEffect = Instantiate (warningPrefab, battleGrid.gameObject.transform);
                    echoEffect.transform.localPosition = new Vector3 (x, y, 0);
                    warnings.Add (echoEffect);
                }
            }
        }

        GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.vineRumble);

        return warnings;
    }

    /// <summary>
    /// Spawns vine attacks on given location.
    /// </summary>
    /// <param name="_pos"></param>
    private void SpawnEchoAttacks(Vector2Int _pos)
    {
        for (int x = _pos.x - 2; x < _pos.x + 3; x += 2)
        {
            if (x > -1 && x < battleGrid.width)
            {
                SpawnProjectile (x, 8, Vector2Int.down, echoProjectile, 0.1f);
                GI_AudioManager.Instance.PlayClip (GI_AudioManager.Instance.vineAttack);
            }
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
