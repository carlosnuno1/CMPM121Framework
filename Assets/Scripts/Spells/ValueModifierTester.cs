using UnityEngine;
using System.Collections.Generic;

public class ValueModifierTester : MonoBehaviour
{
    void Start()
    {
        // Test case 1: No modifiers
        TestModifiers(100f, new List<ValueModifier>());

        // Test case 2: Single additive
        TestModifiers(100f, new List<ValueModifier> {
            new ValueModifier(ValueModifier.ModType.Add, 50f)
        });

        // Test case 3: Single multiplicative
        TestModifiers(100f, new List<ValueModifier> {
            new ValueModifier(ValueModifier.ModType.Multiply, 1.5f)
        });

        // Test case 4: Multiple additive
        TestModifiers(100f, new List<ValueModifier> {
            new ValueModifier(ValueModifier.ModType.Add, 50f),
            new ValueModifier(ValueModifier.ModType.Add, 25f)
        });

        // Test case 5: Multiple multiplicative
        TestModifiers(100f, new List<ValueModifier> {
            new ValueModifier(ValueModifier.ModType.Multiply, 1.5f),
            new ValueModifier(ValueModifier.ModType.Multiply, 2f)
        });

        // Test case 6: Mixed modifiers
        TestModifiers(100f, new List<ValueModifier> {
            new ValueModifier(ValueModifier.ModType.Add, 50f),
            new ValueModifier(ValueModifier.ModType.Multiply, 1.5f),
            new ValueModifier(ValueModifier.ModType.Add, 25f),
            new ValueModifier(ValueModifier.ModType.Multiply, 2f)
        });

        // Test case 7: Negative values
        TestModifiers(100f, new List<ValueModifier> {
            new ValueModifier(ValueModifier.ModType.Add, -20f),
            new ValueModifier(ValueModifier.ModType.Multiply, 0.5f)
        });
    }

    void TestModifiers(float baseValue, List<ValueModifier> mods)
    {
        float result = ValueModifier.ApplyModifiers(baseValue, mods);
        string modText = "";
        foreach (var mod in mods)
        {
            modText += $"\n  - {mod.type}: {mod.value}";
        }
        
        Debug.Log($"[ValueModifierTest] Base value: {baseValue}{modText}\nResult: {result}\n");
    }
}