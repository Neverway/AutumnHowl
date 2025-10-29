using Neverway.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Phantom_Wander : MonoBehaviour
{
    public OverworldEnemy enemyController;
    [SerializeField] public float searchDistance = 6;
    [SerializeField] public float comfyDistance = 3f;
    [SerializeField] public float enterBattleDistance = 0.75f;
    public Vector3 homePosition { get; private set; }
    [HideInInspector] public Character player;
    // the attached Rigidbody2D component
    private Rigidbody2D rb;
    /// <summary>Normalized vector for movement direction.</summary>
    private Vector2 movement;
    private Vector2 lastTargetDirection;
    private Vector2 targetDirection;
    private float currentMoveSpeed = 0;
    public float wanderSpeed = 1f;
    public float chaseSpeed = 2f;
    public float timeToChooseNewDirection = 2f;
    public float followingPlayerTimeToChooseDirectionFactor = 0.75f;
    public float speedupSpeed = 10f;
    private Animator animator;
    private float newDirectionTimer = 0f;
    private bool chasingPlayer;
    // Start is called before the first frame update
    private void Start()
    {
        player = GameInstance.Playerbody;
        rb = GetComponent<Rigidbody2D>();
        homePosition = transform.position;
        PickRandomDirection();
        rb.velocity = targetDirection * wanderSpeed * 0.5f;

        //Setup animator if animator is found
        animator = GetComponent<Animator>();
        if (animator == null) return;
        animator.SetFloat("idleX", 0);
        animator.SetFloat("idleY", -1);
    }

    // Update is called once per frame
    private void Update()
    {
        if (newDirectionTimer > timeToChooseNewDirection)
        {
            newDirectionTimer = 0;
            PickRandomDirection();
            chasingPlayer = LookForPlayer();

            if (chasingPlayer) currentMoveSpeed = 
                    Mathf.Lerp(chaseSpeed, wanderSpeed, (transform.position - homePosition).magnitude / comfyDistance);
            else 
                currentMoveSpeed = wanderSpeed;
        }
        animator.speed = (chasingPlayer ? 2.5f : 1.5f);

        lastTargetDirection.Normalize();
        targetDirection.Normalize();
        //Enter battle if close enough
        Vector3 toPlayer = player.transform.position - transform.position;
        if (toPlayer.magnitude < enterBattleDistance) EnterBattle();

        newDirectionTimer += Time.deltaTime;
        movement = Vector2.Lerp(lastTargetDirection, targetDirection, 2f * (newDirectionTimer / timeToChooseNewDirection));
        movement.Normalize();
        rb.velocity = movement * LerpDeltaTime(rb.velocity.magnitude, currentMoveSpeed, speedupSpeed);

        //Update aniamtor if one exists
        if (animator == null) return;

        if (currentMoveSpeed != 0)
        {
            animator.SetBool("walking", true);
            animator.SetFloat("walkX", movement.x);
            animator.SetFloat("walkY", movement.y);
        }
        else
        {
            animator.SetBool("walking", false);
        }
        animator.SetFloat("idleX", movement.x);
        animator.SetFloat("idleY", movement.y);
    }

    private void OnDrawGizmos()
    {
        //Draw wander range
        if (Application.isPlaying)
            DrawGizmosCircle(Color.cyan, homePosition, comfyDistance);
        else
            DrawGizmosCircle(Color.cyan, transform.position, comfyDistance);

        //Draw aggro range and enter battle range
        DrawGizmosCircle(Color.yellow, transform.position, searchDistance);
        DrawGizmosCircle(Color.red, transform.position, enterBattleDistance);

        if (Application.isPlaying)
        {
            //Draw line between enemy and home position
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, homePosition);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + ((Vector3)movement));
        }
    }
    private void DrawGizmosCircle(Color color, Vector3 center, float radius)
    {
        Gizmos.color = color;
        int segments = 32;
        // Build rotation matrix to orient circle
        Quaternion rotation = Quaternion.LookRotation(Vector3.up);
        Vector3 prevPoint = center + rotation * (Vector3.right * radius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 nextPoint = center + rotation * (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    private bool LookForPlayer()
    {
        if (player == null) return false;

        Vector3 toPlayer = player.transform.position - transform.position;

        if (toPlayer.magnitude < searchDistance)
        {
            targetDirection = new Vector2(toPlayer.x, toPlayer.y);
            newDirectionTimer = timeToChooseNewDirection * followingPlayerTimeToChooseDirectionFactor;
            return true;
        }
        return false;
    }

    private void EnterBattle() => enemyController.EnterBattle();

    private void PickRandomDirection()
    {
        lastTargetDirection = targetDirection;

        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomComfyPosition = homePosition +
            Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector3.up * Random.Range(0f, comfyDistance);
        Vector3 randomDirectionPosition = transform.position +
            Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector3.up;

        float factor = (transform.position - homePosition).magnitude / comfyDistance;
        factor = Mathf.Clamp(factor, 0f, 1f);
        factor = Mathf.Pow(factor, factor + 0.4f);

        Vector3 positionToAimFor = Vector3.Lerp(randomDirectionPosition, randomComfyPosition, factor);

        targetDirection = randomComfyPosition - transform.position;
    }

    private void GetDirectionToPlayer()
    {
        movement = (player.transform.position - transform.position).normalized;
    }

    private Vector3 LerpDeltaTime(Vector3 from, Vector3 to, float speed) =>
        Vector3.Lerp(from, to, 1f - Mathf.Exp(-speed * Time.deltaTime));
    private float LerpDeltaTime(float from, float to, float speed) =>
        Mathf.Lerp(from, to, 1f - Mathf.Exp(-speed * Time.deltaTime));
}