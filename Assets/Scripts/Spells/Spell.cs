using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public abstract class Spell 
{
    // Core properties every spell needs
    protected SpellCaster owner;
    protected float last_cast;
    
    // Basic attributes from JSON
    protected string name;
    protected string description;
    protected int icon;

    protected Spell(SpellCaster owner)
    {
        this.owner = owner;
        this.last_cast = -999f; // Ensure first cast is available
    }

    // Core methods for all spells
    public abstract string GetName();
    public abstract string GetDescription();
    public abstract int GetIcon();
    
    // Scalable attributes
    public abstract int GetManaCost(int power, int wave);
    public abstract float GetCooldown();
    public abstract int GetDamage(int power, int wave);

    // JSON initialization
    public abstract void SetAttributes(JObject json);

    // Casting 
    public abstract IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team);

    // Add this accessor for UI
    public float GetLastCastTime() => last_cast;

    // Update IsReady to use GetLastCastTime
    public bool IsReady()
    {
        return (GetLastCastTime() + GetCooldown() < Time.time);
    }

    // Factory method to create the right type of spell from JSON
    public static Spell FromJson(JObject json, SpellCaster owner)
    {
        // We'll implement this later when we have concrete classes
        throw new System.NotImplementedException();
    }
}
