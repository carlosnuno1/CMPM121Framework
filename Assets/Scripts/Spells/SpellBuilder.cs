using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

public class SpellBuilder
{
    private static JObject spellsJson;
    private static Dictionary<string, JObject> baseSpells = new Dictionary<string, JObject>();
    private static Dictionary<string, JObject> modifierSpells = new Dictionary<string, JObject>();
    
    // Probability weights for number of modifiers
    private static int[] modifierCountWeights = new int[] { 60, 30, 10 }; // 0, 1, 2 modifiers

    public static void Initialize(string jsonText)
    {
        spellsJson = JObject.Parse(jsonText);
        
        // Separate base spells and modifier spells
        foreach (var prop in spellsJson.Properties())
        {
            string spellKey = prop.Name;
            JObject spellData = (JObject)prop.Value;
            
            // Check if this is a modifier spell
            if (spellData["type"] != null && spellData["type"].ToString() == "modifier")
            {
                modifierSpells.Add(spellKey, spellData);
            }
            else
            {
                baseSpells.Add(spellKey, spellData);
            }
        }
        
        Debug.Log($"SpellBuilder initialized with {baseSpells.Count} base spells and {modifierSpells.Count} modifier spells");
    }
    
    // Create a spell from its key
    public static Spell CreateSpell(string spellKey, SpellCaster owner)
    {
        if (!spellsJson.ContainsKey(spellKey))
        {
            Debug.LogError($"Spell key '{spellKey}' not found in spells.json");
            return null;
        }
        
        JObject spellData = (JObject)spellsJson[spellKey];
        return CreateSpellFromJson(spellKey, spellData, owner);
    }
    
    // Create a spell from JSON data
    private static Spell CreateSpellFromJson(string spellKey, JObject spellData, SpellCaster owner)
    {
        // Check if this is a modifier spell
        bool isModifier = spellData["type"] != null && spellData["type"].ToString() == "modifier";
        
        if (isModifier)
        {
            // For modifier spells, we need to know what spell they're modifying
            if (spellData["inner_spell"] != null)
            {
                string innerSpellKey = spellData["inner_spell"].ToString();
                Spell innerSpell = CreateSpell(innerSpellKey, owner);
                
                // Create the appropriate modifier spell
                return CreateModifierSpell(spellKey, spellData, owner, innerSpell);
            }
            else
            {
                Debug.LogError($"Modifier spell '{spellKey}' has no inner_spell specified");
                return null;
            }
        }
        else
        {
            // Create the appropriate base spell
            return CreateBaseSpell(spellKey, spellData, owner);
        }
    }
    
    // Create a base spell based on its type
    private static Spell CreateBaseSpell(string spellKey, JObject spellData, SpellCaster owner)
    {
        string spellType = spellKey;
        if (spellData["spell_type"] != null)
        {
            spellType = spellData["spell_type"].ToString();
        }
        
        switch (spellType)
        {
            case "arcane_bolt":
                return new ArcaneBoltSpell(owner, spellData);
            case "arcane_spray":
                return new ArcaneSpraySpell(owner, spellData);
            case "magic_missile":
                return new MagicMissileSpell(owner, spellData);
            case "arcane_explosion":
                return new ArcaneExplosionSpell(owner, spellData);
            case "frost_nova":
                return new FrostNovaSpell(owner, spellData);
            default:
                Debug.LogWarning($"Unknown base spell type '{spellType}', defaulting to ArcaneBoltSpell");
                return new ArcaneBoltSpell(owner, spellData);
        }
    }
    
    // Create a modifier spell based on its type
    private static Spell CreateModifierSpell(string spellKey, JObject spellData, SpellCaster owner, Spell innerSpell)
    {
        string modifierType = spellKey;
        if (spellData["modifier_type"] != null)
        {
            modifierType = spellData["modifier_type"].ToString();
        }
        
        ModifierSpell modSpell = null;
        
        switch (modifierType)
        {
            case "splitter":
                modSpell = new SplitterSpell(owner, spellData);
                break;
            case "doubler":
                modSpell = new DoublerSpell(owner, spellData);
                break;
            case "damage_magnifier":
                modSpell = new DamageMagnifierSpell(owner, spellData);
                break;
            case "speed_modifier":
                modSpell = new SpeedModifierSpell(owner, spellData);
                break;
            case "chaotic":
                modSpell = new ChaoticSpell(owner, spellData);
                break;
            case "homing":
                modSpell = new HomingSpell(owner, spellData);
                break;
            case "piercing":
                modSpell = new PiercingSpell(owner, spellData);
                break;
            case "explosive":
                modSpell = new ExplosiveSpell(owner, spellData);
                break;
            default:
                Debug.LogWarning($"Unknown modifier type '{modifierType}', defaulting to generic ModifierSpell");
                modSpell = ModifierSpell.FromJson(owner, spellData);
                break;
        }
        
        if (modSpell != null)
        {
            modSpell.SetInnerSpell(innerSpell);
        }
        
        return modSpell;
    }
    
    // Generate a random spell (base spell with random modifiers)
    public static Spell GenerateRandomSpell(SpellCaster owner)
    {
        // Select a random base spell
        string baseSpellKey = GetRandomKey(baseSpells);
        JObject baseSpellData = baseSpells[baseSpellKey];
        
        // Create the base spell
        Spell spell = CreateBaseSpell(baseSpellKey, baseSpellData, owner);
        
        // Determine how many modifiers to add
        int modifierCount = GetWeightedRandomIndex(modifierCountWeights);
        
        // Add random modifiers
        for (int i = 0; i < modifierCount; i++)
        {
            string modifierKey = GetRandomKey(modifierSpells);
            JObject modifierData = modifierSpells[modifierKey];
            
            // Create a modifier spell that wraps our current spell
            ModifierSpell modSpell = null;
            
            switch (modifierKey)
            {
                case "splitter":
                    modSpell = new SplitterSpell(owner);
                    break;
                case "doubler":
                    modSpell = new DoublerSpell(owner);
                    break;
                case "damage_magnifier":
                    modSpell = new DamageMagnifierSpell(owner);
                    break;
                case "speed_modifier":
                    modSpell = new SpeedModifierSpell(owner);
                    break;
                case "chaotic":
                    modSpell = new ChaoticSpell(owner);
                    break;
                case "homing":
                    modSpell = new HomingSpell(owner);
                    break;
                case "piercing":
                    modSpell = new PiercingSpell(owner);
                    break;
                case "explosive":
                    modSpell = new ExplosiveSpell(owner);
                    break;
                default:
                    modSpell = new ModifierSpell(owner);
                    break;
            }
            
            modSpell.SetAttributes(modifierData);
            modSpell.SetInnerSpell(spell);
            spell = modSpell;
        }
        
        return spell;
    }
    
    // Helper method to get a random key from a dictionary
    private static string GetRandomKey<T>(Dictionary<string, T> dict)
    {
        int index = UnityEngine.Random.Range(0, dict.Count);
        return dict.Keys.ElementAt(index);
    }
    
    // Helper method to get a weighted random index
    private static int GetWeightedRandomIndex(int[] weights)
    {
        int totalWeight = weights.Sum();
        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        
        int cumulativeWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }
        
        return 0; // Fallback
    }

    // Alias for CreateSpell to maintain compatibility
    public static Spell Build(string spellKey, SpellCaster owner)
    {
        return CreateSpell(spellKey, owner);
    }

    // Alias for GenerateRandomSpell to maintain compatibility
    public static Spell BuildRandom(SpellCaster owner)
    {
        return GenerateRandomSpell(owner);
    }
}
