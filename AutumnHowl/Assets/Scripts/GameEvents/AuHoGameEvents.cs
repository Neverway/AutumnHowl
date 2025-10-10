using ErryLib.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AuHoGameEvents 
{
    public static bool InvokeAndGetIfSuccess(this GameEvent gameEvent)
    {
        gameEvent.Invoke();
        return !gameEvent.isEventInterrupted;
    }
}

public class Event_UseItem : GameEvent<Event_UseItem>
{
    public Item itemBeingUsed;
    public Character user;
    public Event_UseItem(Item itemBeingUsed, Character user) 
    { 
        this.itemBeingUsed = itemBeingUsed;
        this.user = user;
    }

    protected override bool IsInterruptable => true;

    protected override void WhenInvoked() { }
}
public class Event_GetItem : GameEvent<Event_UseItem>
{
    protected override bool IsInterruptable => true;

    protected override void WhenInvoked()
    {
        throw new System.NotImplementedException();
    }
}















/*


public class BattleStateController : MonoBehaviour
{
    public GameObject player;

    BattleState currentState;

    public BattleState PlayerTurnState { get; private set; }
    public bool BattleActive { get; private set; }
    public void StartBattle()
    {
        BattleActive = true;
        currentState = new BattleStart(this);
        PlayerTurnState = new BattlePlayerTurn(this);
        StartCoroutine(UpdateBattleStates());
    }
    public void EndBattle() => BattleActive = false;

    public IEnumerator UpdateBattleStates()
    {
        while (BattleActive)
        {
            yield return currentState.OnStateUpdate();
        }
    }

    public void NewState(BattleState newState)
    {
        BattleState oldState = currentState;
        oldState.OnStateLeave(newState);

        currentState = newState;
        currentState.OnStateEnter(oldState);
    }
}

// */






namespace ErryLib.EnumerableStateMachine
{
    public abstract class EnumerableStateMachine<TState> where TState : IEnumerable<TState>
    {
        public IEnumerator Start()
        {
            while (true)
            {

            }
        }
        public abstract TState StartingState { get; }
    }

    public interface IEnumerable<TStateMachine, TState> where TState : IEnumerable<TState>
    {
        public IEnumerator OnStateEnter(EnumerableStateMachine<TState> machine, IEnumerable stateLeaving);
        public IEnumerator OnStateExit(EnumerableStateMachine<TState> machine, IEnumerable stateEntering);
        public IEnumerator OnStateUpdate(EnumerableStateMachine<TState> machine);
    }
}



