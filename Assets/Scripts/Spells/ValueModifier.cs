using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ValueModifier
{
    public enum ModType 
    { 
        Add,        // Adds a flat value
        Multiply    // Multiplies by a percentage (1.0 = 100%)
    }

    public ModType type;
    public float value;

    public ValueModifier(ModType type, float value)
    {
        this.type = type;
        this.value = value;
    }

    // applies list of modfiier to base value
    // additive modifiers are applied first, then multiplicative
    public static float ApplyModifiers(float baseValue, List<ValueModifier> modifiers)
    {
        float result = baseValue;
        
        // Apply additive modifiers first
        foreach (var mod in modifiers.Where(m => m.type == ModType.Add))
        {
            result += mod.value;
        }
        
        // Then apply multiplicative modifiers
        foreach (var mod in modifiers.Where(m => m.type == ModType.Multiply))
        {
            result *= mod.value;
        }
        
        return result;
    }
}
