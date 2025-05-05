using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SpellBuilderRunner : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Testing Spell System...");

        // Initialize SpellBuilder with JSON
        var jsonText = Resources.Load<TextAsset>("spells").text;
        SpellBuilder.Initialize(jsonText);

        // Create a test caster
        var caster = gameObject.AddComponent<SpellCaster>();

        // Test random spell generation
        Debug.Log("\nTesting Random Spell Generation:");
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"\nRandom Spell {i + 1}:");
            var randomSpell = SpellBuilder.BuildRandom(caster);
            PrintSpellInfo(randomSpell);
        }

        // Test specific combinations
        Debug.Log("\nTesting Specific Combinations:");
        
        // Test base spell with damage amplifier
        var baseSpell = SpellBuilder.Build("arcane_bolt", caster);
        var damageAmp = SpellBuilder.Build("damage_amp", caster) as ModifierSpell;
        if (damageAmp != null)
        {
            damageAmp.SetInnerSpell(baseSpell);
            Debug.Log("\nTesting Damage Amplified Arcane Bolt:");
            PrintSpellInfo(damageAmp);
        }
    }

    void PrintSpellInfo(Spell spell)
    {
        if (spell == null)
        {
            Debug.LogError("Null spell!");
            return;
        }

        Debug.Log($"Name: {spell.GetName()}");
        Debug.Log($"Description: {spell.GetDescription()}");
        Debug.Log($"Base Values (power=10, wave=1):");
        Debug.Log($"  Damage: {spell.GetDamage(10, 1)}");
        Debug.Log($"  Mana Cost: {spell.GetManaCost(10, 1)}");
        Debug.Log($"  Cooldown: {spell.GetCooldown()}");
    }

    IEnumerator TestCast(Spell spell)
    {
        Debug.Log($"\nTesting Cast:");
        yield return spell.Cast(
            Vector3.zero,           // from origin
            Vector3.right,          // to the right
            Hittable.Team.PLAYER
        );
    }
} 