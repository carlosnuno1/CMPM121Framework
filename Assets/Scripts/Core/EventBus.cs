using UnityEngine;
using System;

public class EventBus 
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

    public event Action<int> OnTakeDamage;
    public void DoTakeDamage(int amount)
    {
        OnTakeDamage?.Invoke(amount);
    }

    public event Action OnKill;
    public void DoKill()
    {
        OnKill?.Invoke();
    }
    
    public event Action<int> OnStandStill;
    public void DoStandStill(int amount)
    {
        OnStandStill?.Invoke(amount);
    }

    public event Action OnSpellSwitch;
    public void DoSpellSwitch()
    {
        OnSpellSwitch?.Invoke();
    }
    
    public event Action OnMove;
    public void DoMove()
    {
        OnMove?.Invoke();
    }
    
    public event Action OnCastSpell;
    public void DoCastSpell()
    {
        OnCastSpell?.Invoke();
    }

    public event Action<Relic> OnRelicPickup;
    public void PickupRelic(Relic r)
    {
        OnRelicPickup?.Invoke(r);
    }

}
