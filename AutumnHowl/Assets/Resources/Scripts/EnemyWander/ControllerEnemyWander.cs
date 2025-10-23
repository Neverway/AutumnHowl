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
        public Controller_Overworld_Player player;
        public BattleData battleData;
        public string mapID;
        public GI_AuHoGameState gameState;
        [SerializeField] public float searchDistance = 6;
        [SerializeField] public float comfyDistance = 3f;
        [SerializeField] public float enterBattleDistance = 0.75f;
        public Vector3 homePosition {  get; private set; }
        // the attached Rigidbody2D component
        private Rigidbody2D rb;
        /// <summary>
        /// Normalized vector for movement direction.
        /// </summary>
        public Vector2 movement;
        public float currentMoveSpeed = 0;
        public float wanderSpeed = 1f;
        public float chaseSpeed = 2f;
        [Tooltip("Turn this off to make it wander but not attack. Useful for villagers and things.")]
        public bool chasesPlayer = true;
        // Start is called before the first frame update
        void Start ()
        {
            player = FindObjectOfType<Controller_Overworld_Player> ();
            rb = GetComponent<Rigidbody2D> ();
            homePosition = transform.position;
            NewState (new EW_Idle(this));
            gameState = FindObjectOfType<GI_AuHoGameState>();
        }

        // Update is called once per frame
        new void Update ()
        {
            base.Update ();
            rb.velocity = movement * currentMoveSpeed;
            Debug.DrawLine (transform.position, homePosition, Color.yellow);
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

        internal void EnterBattle ()
        {
            Debug.Log("Entering battle!");
            // I gotchu ~Liz
            gameState = FindObjectOfType<GI_AuHoGameState>();
            gameState.currentGameState.currentBattle = battleData;
            gameState.GetComponent<GI_WorldLoader>().Load(mapID);
        }

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