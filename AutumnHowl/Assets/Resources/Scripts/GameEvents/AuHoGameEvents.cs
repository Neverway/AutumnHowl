using ErryLib.GameEvents;
using ErryLib.ModiferSystem.Instancers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AuHoGameEvent : BasicGameEvent
{
    protected override bool IsInterruptable => false;
    public abstract GameEventType EventType { get; }
    protected override void WhenInvoked() { }
    public bool IfInvokeSuccess()
    {
        Invoke();
        return !isEventInterrupted;
    }
    public bool IfInvokeInterrupted()
    {
        Invoke();
        return isEventInterrupted;
    }
}
public abstract class AuHoGameEvent_Interruptable : AuHoGameEvent 
{ 
    protected override bool IsInterruptable => true; 
}

public class EventCounter<T> : ListensToGameEvent<BasicGameEvent> where T : AuHoGameEvent
{
    public EventCounter() => GameEventSystem.Register(this);

    public int counter = 0;
    public bool ReactToEvent(BasicGameEvent gameEvent, InvokeTiming timing)
    {
        if (timing == InvokeTiming.After && gameEvent is T)
            counter++;
        return true;
    }
    public void Discard() => GameEventSystem.UnRegister(this);
}

public enum GameEventType
{
    TakeDamage = 1,
    Heal = 2,
    BattleTurnPassed = 3,
    BattleWon = 4,
    Died = 5,
    BattleWavePassed = 6,

    UseAnyItem = 0,
    UseConsumable = 7,
    UseSpell = 8,
}


//========================================= [ All Events Here ] ==================================================

public class Event_UseItem : AuHoGameEvent_Interruptable
{
    public Item itemBeingUsed;
    public CharacterIdentifier user;
    public Event_UseItem(Item itemBeingUsed, CharacterIdentifier user) 
    { 
        this.itemBeingUsed = itemBeingUsed;
        this.user = user;
    }
    protected override void WhenInvoked() 
    { 

    }

    public override GameEventType EventType => GameEventType.UseAnyItem;
}
public class Event_UseConsumable : AuHoGameEvent_Interruptable
{
    public Item itemBeingUsed;
    public CharacterIdentifier user;
    public Event_UseConsumable(Item itemBeingUsed, CharacterIdentifier user)
    {
        this.itemBeingUsed = itemBeingUsed;
        this.user = user;
    }
    protected override void WhenInvoked()
    {

    }

    public override GameEventType EventType => GameEventType.UseConsumable;
}
public class Event_UseSpell : AuHoGameEvent_Interruptable
{
    public Item itemBeingUsed;
    public CharacterIdentifier user;
    public Event_UseSpell(Item itemBeingUsed, CharacterIdentifier user)
    {
        this.itemBeingUsed = itemBeingUsed;
        this.user = user;
    }
    protected override void WhenInvoked()
    {

    }

    public override GameEventType EventType => GameEventType.UseAnyItem;
}


public class Event_TakeDamage : AuHoGameEvent_Interruptable
{
    public float damage;
    public Vector2Int direction;
    public CharacterIdentifier target;
    public Event_TakeDamage(CharacterIdentifier target, Vector2Int direction, float damage)
    {
        this.target = target;
        this.direction = direction;
        this.damage = damage;
    }
    public override GameEventType EventType => GameEventType.TakeDamage;
}
public class Event_Heal : AuHoGameEvent_Interruptable
{
    public float healAmount;
    public CharacterIdentifier target;
    public Event_Heal(CharacterIdentifier target, float healAmount)
    {
        this.target = target;
        this.healAmount = healAmount;
    }
    public override GameEventType EventType => GameEventType.Heal;
}
public class Event_BattleTurnPassed : AuHoGameEvent
{
    public override GameEventType EventType => GameEventType.BattleTurnPassed;
}
public class Event_BattleWon : AuHoGameEvent
{
    public override GameEventType EventType => GameEventType.BattleWon;
}
public class Event_BattleWavePassed : AuHoGameEvent
{
    public override GameEventType EventType => GameEventType.BattleWavePassed;
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



