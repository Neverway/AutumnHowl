using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neverway.StateMachine
{
    /// <summary>
    /// A structure for generic states the state machine can have.
    /// </summary>
    public abstract class StateBase<TStateMachine> where TStateMachine : StateMachine<TStateMachine>
    {
        [Tooltip("A reference to the controller so the state can access it.")]
        internal TStateMachine controller;

        public StateBase(TStateMachine _controller)
        {
            this.controller = _controller;
        }

        public abstract void OnStateEnter (StateBase<TStateMachine> stateLeaving);
        public abstract void OnStateUpdate ();
        public abstract void OnStateLeave (StateBase<TStateMachine> stateEntering);

    }
}
