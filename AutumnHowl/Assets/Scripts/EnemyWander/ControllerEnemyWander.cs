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
        private Controller_Overworld_Player player;
        [SerializeField] private float searchDistance = 6;
        [SerializeField] private float walkForce = 100f;
        private Vector3 homePosition;
        // the attached Rigidbody2D component
        private Rigidbody2D rb;
        /// <summary>
        /// Normalized vector for movement direction.
        /// </summary>
        private Vector2 movement;
        public float currentMoveSpeed = 0;
        public float wanderSpeed = 1f;
        public float chaseSpeed = 2f;
        // Start is called before the first frame update
        void Start ()
        {
            player = FindObjectOfType<Controller_Overworld_Player> ();
            rb = GetComponent<Rigidbody2D> ();
            homePosition = transform.position;
            NewState (new EW_Idle(this));
        }

        // Update is called once per frame
        new void Update ()
        {
            base.Update ();
            rb.velocity = movement * currentMoveSpeed;
        }

        internal bool LookForPlayer ()
        {
            if (player == null)
            {
                Debug.LogError ("Enemy doesn't have player to search for??");
                return false;
            }
            //If we're close to the player, return true.
            if ((transform.position - player.transform.position).magnitude < searchDistance)
            {
                return true;
            }
            return false;
        }

        internal void PickRandomDirection ()
        {
            float randomAngle = UnityEngine.Random.Range (0f, 360f);
            movement = Quaternion.AngleAxis (randomAngle, Vector3.forward) * Vector3.up;
        }

        internal void GetDirectionToPlayer ()
        {
            movement = (transform.position - player.transform.position).normalized;
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
            controller.GetDirectionToPlayer ();
            controller.currentMoveSpeed = controller.chaseSpeed;
        }

        public override void OnStateLeave (EW_State stateEntering)
        {
        }

        public override void OnStateUpdate ()
        {
        }

    }
}