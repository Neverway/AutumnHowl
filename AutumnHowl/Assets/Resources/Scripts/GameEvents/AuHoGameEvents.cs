using ErryLib.GameEvents;
using ErryLib.ModiferSystem.Instancers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AuHoGameEvent : BasicGameEvent
{
    protected override bool IsInterruptable => false;
    public abstract GameEventType EventType { get; }
    public virtual CharacterIdentifier EventOwner { get => null; }
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
        {
            counter++;
            return true;
        }
        return false;
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
        if (itemBeingUsed is Item_Consumable consumable) 
            if (new Event_UseConsumable(consumable, user).IfInvokeInterrupted())
            {
                InterruptEvent();
                return;
            }
        if (itemBeingUsed is Item_Magic spell)
            if (new Event_UseSpell(spell, user).IfInvokeInterrupted())
            {
                InterruptEvent();
                return;
            }
    }


    public override GameEventType EventType => GameEventType.UseAnyItem;
    public override CharacterIdentifier EventOwner { get => user; }
}
public class Event_UseConsumable : AuHoGameEvent_Interruptable
{
    public Item_Consumable itemBeingUsed;
    public CharacterIdentifier user;
    public Event_UseConsumable(Item_Consumable itemBeingUsed, CharacterIdentifier user)
    {
        this.itemBeingUsed = itemBeingUsed;
        this.user = user;
    }

    public override GameEventType EventType => GameEventType.UseConsumable;
    public override CharacterIdentifier EventOwner { get => user; }
}
public class Event_UseSpell : AuHoGameEvent_Interruptable
{
    public Item_Magic itemBeingUsed;
    public CharacterIdentifier user;
    public Event_UseSpell(Item_Magic itemBeingUsed, CharacterIdentifier user)
    {
        this.itemBeingUsed = itemBeingUsed;
        this.user = user;
    }

    public override GameEventType EventType => GameEventType.UseSpell;
    public override CharacterIdentifier EventOwner { get => user; }
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
    public override CharacterIdentifier EventOwner { get => target; }
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
    public override CharacterIdentifier EventOwner { get => target; }
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
