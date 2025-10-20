using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neverway.StateMachine
{

    public class StateMachine<TSelf> : MonoBehaviour where TSelf : StateMachine<TSelf>
    {
        private StateBase<TSelf> currentState;

        // Update is called once per frame
        public virtual void Update ()
        {
            if (currentState == null)
            {
                return;
            }
            currentState.OnStateUpdate();
        }

        public void NewState(StateBase<TSelf> newState)
        {
            StateBase<TSelf> oldState = currentState;
            if (currentState != null)
            {
                oldState.OnStateLeave (newState);
            }
            currentState = newState;
            currentState.OnStateEnter(oldState);
        }
    }
}