using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neverway.StateMachine;
using System;
using System.Threading;

namespace Neverway.StateMachine
{
    using EW_State = StateBase<ControllerEnemyWander>;

    public class ControllerEnemyWander : StateMachine<ControllerEnemyWander>
    {
        public OverworldEnemy enemyController;
        [SerializeField] public float searchDistance = 6;
        [SerializeField] public float comfyDistance = 3f;
        [SerializeField] public float enterBattleDistance = 0.75f;
        public Vector3 homePosition {  get; private set; }
        [HideInInspector] public Controller_Overworld_Player player;
        // the attached Rigidbody2D component
        private Rigidbody2D rb;
        /// <summary>Normalized vector for movement direction.</summary>
        public Vector2 movement;
        public float currentMoveSpeed = 0;
        public float wanderSpeed = 1f;
        public float chaseSpeed = 2f;
        [Tooltip("Turn this off to make it wander but not attack. Useful for villagers and things.")]
        public bool chasesPlayer = true;
        private Animator animator;
        // Start is called before the first frame update
        void Start ()
        {
            player = FindObjectOfType<Controller_Overworld_Player> ();
            rb = GetComponent<Rigidbody2D> ();
            homePosition = transform.position;
            NewState (new EW_Idle(this));

            //Setup animator if animator is found
            animator = GetComponent<Animator>();
            if (animator == null) return;
            animator.SetFloat("idleX", 0);
            animator.SetFloat("idleY", -1);
        }

        // Update is called once per frame
        new void Update ()
        {
            base.Update ();
            rb.velocity = movement * currentMoveSpeed;

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

        internal void LookForPlayer ()
        {
            if (player == null)
            {
                Debug.LogError ("Enemy doesn't have player to search for??");
                return;
            }
            if (chasesPlayer == false)
            {
                return;
            }
            //If we're close to the player, return true.
            if ((player.transform.position - transform.position).magnitude < searchDistance)
            {
                NewState(new EW_Chase(this));
            }
        }

        internal void EnterBattle () => enemyController.EnterBattle();

        internal void PickRandomDirection ()
        {
            float randomAngle = UnityEngine.Random.Range (0f, 360f);
            movement = Quaternion.AngleAxis (randomAngle, Vector3.forward) * Vector3.up;
        }

        internal void GetDirectionToPlayer ()
        {
            movement = (player.transform.position - transform.position).normalized;
        }
    }

    class EW_Idle : EW_State
    {
        private float timer = 0f;
        private float maxTime = 2f;
        public EW_Idle (ControllerEnemyWander _controller) : base (_controller)
        {
        }

        public override void OnStateEnter (EW_State stateLeaving)
        {
            timer = 0f;
            controller.currentMoveSpeed = 0f;
        }

        public override void OnStateLeave (EW_State stateEntering)
        {
        }

        public override void OnStateUpdate ()
        {
            timer += Time.deltaTime;
            if (timer > maxTime)
            {
                controller.NewState (new EW_Wander (controller));
                return;
            }
            controller.LookForPlayer ();
        }
    }
    class EW_Wander : EW_State
    {
        private float timer = 0f;
        private float maxTime = 3f;
        public EW_Wander (ControllerEnemyWander _controller) : base (_controller)
        {
        }

        public override void OnStateEnter (EW_State stateLeaving)
        {
            timer = 0f;
            controller.currentMoveSpeed = controller.wanderSpeed;
            Vector3 homeDirection = controller.homePosition - controller.transform.position;
            if (homeDirection.magnitude > controller.comfyDistance)
            {
                controller.movement = homeDirection.normalized;
                return;
            }
            controller.PickRandomDirection ();
        }

        public override void OnStateLeave (EW_State stateEntering)
        {
        }

        public override void OnStateUpdate ()
        {
            timer += Time.deltaTime;
            if (timer > maxTime)
            {
                controller.NewState (new EW_Idle (controller));
                return;
            }
            controller.LookForPlayer ();
        }
    }

    class EW_Chase : EW_State
    {
        public EW_Chase (ControllerEnemyWander _controller) : base (_controller)
        {
        }

        public override void OnStateEnter (EW_State stateLeaving)
        {
            controller.currentMoveSpeed = controller.chaseSpeed;
        }

        public override void OnStateLeave (EW_State stateEntering)
        {
        }

        public override void OnStateUpdate ()
        {
            float playerDistance = (controller.transform.position - controller.player.transform.position).magnitude;
            if (playerDistance > controller.searchDistance)
            {
                controller.NewState (new EW_Idle (controller));
                return;
            }
            if (playerDistance < controller.enterBattleDistance)
            {
                controller.EnterBattle ();
            }

            controller.GetDirectionToPlayer ();
        }

    }
}