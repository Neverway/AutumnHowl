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
using System.Linq;
using UnityEngine;
using static GameFeatureConstants.Battle;

public abstract class Char_Battle : Character
{
    #region========================================( Variables )======================================================//
    /*-----[ Inspector Variables ]------------------------------------------------------------------------------------*/
    public List<AttackSequence> AttackSequences;

    /*-----[ External Variables ]-------------------------------------------------------------------------------------*/
    public bool canMove;
    //If true, this character can be moved by "pushing" attacks
    public bool pushable;
    [Tooltip("Time in seconds it takes to hop from one grid position to another")]
    public float gridHopAnimation_Seconds = 0.15f;
    [Tooltip("Peak height to reach during hop animation")]
    public float gridHopAnimation_Height = 0.35f;
    [Tooltip("Transform to use for the hop animation")]
    public Transform gridHopAnimationContainer;
    /*-----[ Internal Variables ]-------------------------------------------------------------------------------------*/

    //unique identifier for a stat mod
    private const string Mod_ConditionalBlock = "ConditionalBlock";

    /*-----[ Reference Variables ]------------------------------------------------------------------------------------*/
    public GridPawn gridPawnController;
    public BattleGridPather gridPather;
    public BattleGrid battleGrid;
    public BattleStateController battleStateController;
    //If this is not null, this object gets spawned when the character dies.
    public GameObject spawnOnDeath;
    public bool useBlock = false;

    //This is so we can invert the direction the pawn faces when it attacks (currently only used for the Player pawn).
    [HideInInspector] public bool invertAttackFacingDirections = false;

    private Coroutine currentGridHop;
    #endregion


    #region=======================================( Functions )=======================================================//
    /*-----[ Mono Functions ]-----------------------------------------------------------------------------------------*/
    public override void Start()
    {
        base.Start();
        gridPather = FindObjectOfType<BattleGridPather>();
        battleGrid = FindObjectOfType<BattleGrid>();
        battleStateController = FindObjectOfType<BattleStateController>();
        OnDeath += Kill;
    }
    
    public void Update()
    {
        if (isDead) return;
        if (animator == null) return;

        animator.SetFloat("idleX", facingDirection.x);
        animator.SetFloat("idleY", facingDirection.y);
        if (!canMove) return;
    }


    /*-----[ Internal Functions ]-------------------------------------------------------------------------------------*/

    /// <summary>
    /// Tests if the character can move to a tile, and returns true if it was able to move.
    /// </summary>
    /// <param name="_direction">Tile to move to; relative to current position.</param>
    /// <param name="doNextTurn">Set to false if this object shouldn't trigger NextTurnStep, for example if it moves in realtime.</param>
    /// <returns></returns>
    protected virtual bool TryMoveInDirection (Vector2Int _direction, bool doNextTurn = true, GridPawn _pathTargetPawn = null, bool shouldProgressTurn = true, bool ignoreObsticals=false)
    {
        var testPos = gridPawnController.position + _direction;
        if (BattleGrid.Instance.ValidTile (testPos.x, testPos.y) && !BattleGrid.Instance.IsOccupied(testPos.x, testPos.y))
        {
            gridPawnController.MoveToTile (testPos.x, testPos.y, this);
            if (doNextTurn)
            {
                if (_pathTargetPawn != null)
                {
                    gridPather.GetPathToTarget (gridPawnController);
                }
                if (shouldProgressTurn) battleStateController.NextTurnStep();
            }
            return true;
        }

        return false;
    }
    
    protected virtual bool TryMoveTo(Vector2Int _direction)
    {
        
        print($"{gameObject.name} - try move called");
        if (BattleGrid.Instance.ValidTile(_direction.x, _direction.y) && !BattleGrid.Instance.IsOccupied(_direction.x, _direction.y))
        {
            print($"{gameObject.name} - try move success");
            gridPawnController.MoveToTile(_direction.x, _direction.y, this);
            battleStateController.NextTurnStep(caller:gameObject.name);
            return true;
        }
        print($"{gameObject.name} - try move failure");

        return false;
    }

    /// <summary>
    /// Moves the pawn directly to a tile, without calling NextTurnStep.
    /// DOES NOT CHECK FOR OBSTACLES.
    /// </summary>
    /// <param name="_direction"></param>
    public void TeleportPawnTo(Vector2Int _position)
    {
        gridPawnController.MoveToTile (_position.x, _position.y, this);
    }
    public void AnimateGridHop(Vector3 hopFrom, Vector3 hopTo)
    {
        if (currentGridHop != null) StopCoroutine(currentGridHop);

        currentGridHop = StartCoroutine(CoAnimateGridHop(hopFrom, hopTo, gridHopAnimation_Seconds));
    }
    public IEnumerator CoAnimateGridHop(Vector3 hopFrom, Vector3 hopTo, float seconds)
    {
        if (gridHopAnimationContainer == null) yield break;
        if (seconds <= 0f) {
            Debug.LogWarning($"Hop animation set to {seconds} seconds, which make no gosh darn sense.. SO I AINT DOIN IT");
            yield break;
        }

        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            gridHopAnimationContainer.transform.position = Vector3.Lerp(hopFrom, hopTo, timer / seconds);
            float height = (-1 * Mathf.Pow(((2 * timer / seconds) - 1), 2) + 1) * gridHopAnimation_Height;
            gridHopAnimationContainer.transform.position += Vector3.up * height;

            //Stay until timer reaches end
            if (timer < seconds) yield return null;
            else break;
        }
        gridHopAnimationContainer.transform.position = hopTo;

        yield break;
    }

    public virtual void OnAttacked(AttackElement attack)
    {

    }

    public IEnumerator CoTryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false, bool shouldProgressTurn = true)
    {
        //when hasStopped is true, it stops the rest of the sequence from firing.
        var hasStopped = false;
        //Start with last cardinal direction being the same as the characters CURRENT cardinal direction
        Vector2Int lastCardinalDirection = new Vector2Int(Mathf.RoundToInt(facingDirection.x), Mathf.RoundToInt(facingDirection.y));
        for (int i = 0; i < attackSequence.attacks.Count; i++)
        {
            // Applied position is the position offset after mirroring has been applied
            var appliedPosition = attackSequence.attacks[i].position;
            if (mirrorX) appliedPosition.x = -attackSequence.attacks[i].position.x;
            if (mirrorY) appliedPosition.y = -attackSequence.attacks[i].position.y;

            var currentPosition = gridPawnController.position + appliedPosition;

            if (this is Char_Battle_Player)
            {
                battleGrid.AddPlayerAttackPosition(currentPosition);
            }

            //Set facing direction to attack position (mirror the position if invertAttackFacingDirections is true)
            facingDirection = attackSequence.attacks[i].position * (invertAttackFacingDirections ? -1 : 1);

            DoAttack(attackSequence.attacks[i], currentPosition);
            GridPawn target = battleGrid.GetIsOccupied(currentPosition);
            if (target != null)
            {
                if (target.type == GridPawn.GridPawnType.obstacle)
                {
                    //print($"Found obstcl at {appliedPosition}");
                    hasStopped = true;
                    GI_AudioManager.Instance.PlayClip (GI_AudioManager.Instance.hitBounce);
                }
                else if (target.type == GridPawn.GridPawnType.character)
                {
                    //print($"Found char {target.gameObject.name} at {appliedPosition}");
                    var char_Battle = target.GetComponent<Char_Battle> ();
                    if (char_Battle.Stats.health <= 0)
                    {
                        GI_AudioManager.Instance.PlayClip(GI_AudioManager.Instance.hitKill);
                    }
                    else
                    {
                        //Stop the attack because we hit something and didn't kill it.
                        hasStopped = true;
                        GI_AudioManager.Instance.PlayClip (GI_AudioManager.Instance.hitDamage);
                    }
                }
                else if (target.type == GridPawn.GridPawnType.attack)
                {
                    //print($"Found attack at {appliedPosition}");
                    hasStopped = true;
                }
            }

            if (hasStopped && this is IsPlayerCharacter player)
            {
                //DONT stop the attack if this was the first attack in the sequence and its from the player 
                if (i == 0 && hasStopped) hasStopped = false;
                else //But if we truly are stopping, register the recoil with the animation
                {
                    FindObjectOfType<BattleCameraManager>().GoBackHome();
                    if (DirectionUtility.TryConvertToDirection(lastCardinalDirection, out var convertedDirection2))
                        player.SwingAnimator.RegisterRecoil(convertedDirection2.Value.Info().turned180);
                }
            }

            yield return new WaitForSeconds(0.075f);

            //If we bonked something, go back to the last cardinal direction
            if (hasStopped)
            {
                //Set facing direction to last cardinal direciton (mirror the position if invertAttackFacingDirections is true)
                facingDirection = lastCardinalDirection * (invertAttackFacingDirections ? -1 : 1);
                Debug.Log($"Erry: facingDirection: {facingDirection}");
                break;
            }

            //Update last cardinal direciton if this attack IS in fact a cardinal direction
            if (DirectionUtility.TryConvertToDirection(appliedPosition, out var convertedDirection))
                lastCardinalDirection = convertedDirection.Value.Info().direction;
        }
        if (shouldProgressTurn) battleStateController.NextTurnStep(0.5f);
    }

    /// <summary>executes the attack on the specified grid tile</summary>
    /// <param name="attack"></param>
    public void DoAttack(AttackElement attack, Vector2Int _position)
    {
        Vector3 hitPosition = battleGrid.transform.position + new Vector3(_position.x, _position.y, 0);
        Instantiate(attack.visualEffect, hitPosition, new Quaternion(), null);
        GridPawn target = battleGrid.GetIsOccupied(_position);
        if (target == null)
        {
            DebugDrawAttack(hitPosition, 0.3f);
            return;
        }
        Char_Battle char_Battle = target.GetComponent<Char_Battle>();
        //deal damage
        char_Battle.ApplyConditionalBlock(attack.direction);
        char_Battle.Stats.ModifyHealth(-attack.damage);
        char_Battle.RemoveConditionalBlock();
        //check if we should push the target
        if (char_Battle.pushable && attack.pushing)
        {
            char_Battle.TryMoveInDirection(attack.direction, false);
        }
        char_Battle.OnAttacked(attack);

        //Register hit with swing animator for hitstuns
        if (this is IsPlayerCharacter player)
            player.SwingAnimator.RegisterHit(attack, hitPosition);

        DebugDrawAttack(hitPosition, 1f);
    }
    private void DebugDrawAttack(Vector3 pos, float size)
    {
        size *= 0.5f;
        Color debugAttackColor = (this is IsPlayerCharacter) ? Color.cyan : Color.red;
        Debug.DrawLine(pos + new Vector3(size, size), pos + new Vector3(-size, -size), debugAttackColor, 0.5f);
        Debug.DrawLine(pos + new Vector3(-size, size), pos + new Vector3(size, -size), debugAttackColor, 0.5f);
    }
    
    /// <summary>Applies a defense modifier, but only if blockDirection blocks the attack.</summary>
    /// <param name="direction"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ApplyConditionalBlock (Vector2Int direction)
    {
        if (useBlock == false)
        {
            return;
        }
        //Autumn blocks in reverse, shield guy doesn't XD
        if (invertAttackFacingDirections)
        {
            if (facingDirection != direction)
            {
                return;
            }
        }
        else
        {
            if (facingDirection != -direction)
            {
                return;
            }
        }
        print("ConditionalBlock activated");
        Stats.defense.ModifyStatWith(Mod_ConditionalBlock, BLOCKDEFENSETYPE, Stats.shieldPower);
    }
    
    /// <summary>Removes the defense modifier applied by ApplyConditionalBlock.</summary>
    private void RemoveConditionalBlock ()
    {
        Stats.defense.UnmodifyStatWith (Mod_ConditionalBlock);
    }


    public virtual void TryAttackSequence(AttackSequence attackSequence, bool mirrorX = false, bool mirrorY = false, bool shouldProgressTurn = true)
    {
        StartCoroutine(CoTryAttackSequence(attackSequence, mirrorX, mirrorY, shouldProgressTurn));
    }

    /// <summary>Triggered by OnDeath; Spawns spawnOnDeath if it exists.</summary>
    public virtual void Kill ()
    {
        //Obstacle and enemy destroyed events
        if (Identifier.TemplateCreatedFrom.characterTags.Contains(CharacterTags.Obstacle))
            new Event_ObstacleDestroyed(Identifier).Invoke();
        if (Identifier.TemplateCreatedFrom.characterTags.Contains(CharacterTags.Enemy))
            new Event_EnemyDefeated(Identifier).Invoke();

        if (spawnOnDeath != null)
        {
            GameObject g = Instantiate (spawnOnDeath);
            g.transform.position = transform.position;
        }
        if (this is IsPlayerCharacter) return;
        FindObjectOfType<BattleStateController> ().RemoveCharacter (this);
        Destroy (gameObject);
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

    /// <summary>
    /// Returns the tile with the lowest path number that is closest to the player.
    /// If there are no valid tiles, returns -1,-1 which tells the character to not move.
    /// </summary>
    /// <returns></returns>
    protected Vector2Int GetLowestTileToTarget ()
    {
        var playerPos = battleStateController.battlePlayer.gridPawnController.position;
        var lowestTileNumber = 9999;
        var lowestTile = new Vector2Int (-1, -1);

        List<Vector2Int> possibleTiles = new List<Vector2Int> ();

        // Check surrounding tiles
        // If multiple are found, they should be added to the possibleTiles list
        DirectionUtility.ForEachDirection ((direction) =>
        {
            Vector2Int checkPos = gridPawnController.position + direction.Info ().direction;
            if (battleGrid.IsMoveable (checkPos.x,checkPos.y)==false) return;
            int n = gridPather.grid[checkPos.x, checkPos.y];
            if (n == BattleGridPather.UnassignedTileNumber)
            {
                return;
            }

            if (n < lowestTileNumber)
            {
                lowestTileNumber = gridPather.grid[checkPos.x, checkPos.y];
                possibleTiles.Clear ();
                possibleTiles.Add (checkPos);
            }
            else if (n == lowestTileNumber)
            {
                possibleTiles.Add(checkPos);
            }
        });
        if (possibleTiles.Count == 0)
        {
            //if there's no possible tiles, return lowestTile which is still -1,-1 at this point.
            return lowestTile;
        }

        //Compare the distance of the tiles before picking one.
        //If there's equadistant options, pick at random.
        float farthest = 0;
        //Note: I went with the farthest tiles as-the-crow-flies because
        //      while all these tiles are the same number of steps from
        //      the player, this behavior makes enemies prefer approach from cardinal directions.
        //      In theory we could make it a setting: have them prefer cardinal, diagonal (nearest), or random!
        List<Vector2Int> sameScoreTiles = new List<Vector2Int> ();
        foreach (var tile in possibleTiles)
        {
            float distance = (tile - playerPos).magnitude;
            if (distance > farthest)
            {
                sameScoreTiles.Clear ();
                sameScoreTiles.Add (tile);
                farthest = distance;
                lowestTile = tile;
            }
            else if (distance == farthest)
            {
                sameScoreTiles.Add(tile);
            }
        }
        if (sameScoreTiles.Count == 1)
        {
            return sameScoreTiles[0];
        }

        int n = UnityEngine.Random.Range(0,sameScoreTiles.Count);
        return sameScoreTiles[n];
    }


    #endregion
}

[Serializable]
public class AttackElement
{
    //The position to attack on the BattleGrid
    public Vector2Int position;
    //amount of damage dealt to enemy
    public float damage;
    //object that spawns on the attack's tile
    public GameObject visualEffect;
    //direction the attack is moving.
    public Vector2Int direction = new Vector2Int(0, 0);
    //if true, this attack can push the target.
    public bool pushing = false;
}       

[Serializable]  
public class AttackSequence
{
     public List<AttackElement> attacks;
}
