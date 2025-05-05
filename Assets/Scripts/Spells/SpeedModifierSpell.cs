using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class SpeedModifierSpell : ModifierSpell
{
    private float speedMultiplier = 1.5f; // Default multiplier

    public SpeedModifierSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Swift";
        description = "Increases projectile speed";
    }

    public SpeedModifierSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to SpeedModifierSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse speed multiplier if specified
        if (json["speed_multiplier"] != null)
        {
            float.TryParse(json["speed_multiplier"].ToString(), out speedMultiplier);
        }
        
        // Add the modifier
        modifiers.speedModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, speedMultiplier));
    }
}