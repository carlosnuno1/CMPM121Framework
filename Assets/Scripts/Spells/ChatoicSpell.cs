using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ChaoticSpell : ModifierSpell
{
    private float damageMultiplier = 2.0f; // Large damage increase

    public ChaoticSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Chaotic";
        description = "Greatly increases damage but makes projectile follow an unpredictable path";
    }

    public ChaoticSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to ChaoticSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse damage multiplier if specified
        if (json["damage_multiplier"] != null)
        {
            float.TryParse(json["damage_multiplier"].ToString(), out damageMultiplier);
        }
        
        // Add the damage modifier
        modifiers.damageModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, damageMultiplier));
        
        // Override the trajectory to spiraling
        modifiers.trajectoryOverride = "spiraling";
    }
}