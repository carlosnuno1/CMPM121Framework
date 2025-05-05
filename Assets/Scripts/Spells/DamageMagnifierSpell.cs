using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class DamageMagnifierSpell : ModifierSpell
{
    private float damageMultiplier = 1.5f; // Default multiplier
    private float manaMultiplier = 1.3f;   // Default mana cost increase

    public DamageMagnifierSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Empowered";
        description = "Increases damage but costs more mana";
    }

    public DamageMagnifierSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to DamageMagnifierSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse damage multiplier if specified
        if (json["damage_multiplier"] != null)
        {
            float.TryParse(json["damage_multiplier"].ToString(), out damageMultiplier);
        }
        
        // Parse mana multiplier if specified
        if (json["mana_multiplier"] != null)
        {
            float.TryParse(json["mana_multiplier"].ToString(), out manaMultiplier);
        }
        
        // Add the modifiers
        modifiers.damageModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, damageMultiplier));
        modifiers.manaModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, manaMultiplier));
    }
}