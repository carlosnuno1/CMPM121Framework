using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class HomingSpell : ModifierSpell
{
    private float damageMultiplier = 0.7f; // Damage reduction
    private float manaBonus = 5f;         // Additional mana cost

    public HomingSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Homing";
        description = "Makes projectiles seek targets but reduces damage and increases mana cost";
    }

    public HomingSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to HomingSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse damage multiplier if specified
        if (json["damage_multiplier"] != null)
        {
            float.TryParse(json["damage_multiplier"].ToString(), out damageMultiplier);
        }
        
        // Parse mana bonus if specified
        if (json["mana_bonus"] != null)
        {
            float.TryParse(json["mana_bonus"].ToString(), out manaBonus);
        }
        
        // Add the modifiers
        modifiers.damageModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, damageMultiplier));
        modifiers.manaModifiers.Add(new ValueModifier(ValueModifier.ModType.Add, manaBonus));
        
        // Override the trajectory to homing
        modifiers.trajectoryOverride = "homing";
    }
}