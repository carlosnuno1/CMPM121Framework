using UnityEngine;
using System.Collections.Generic;      // Keep this for Dictionary
using System.Linq;
using Newtonsoft.Json.Linq;

public class SpellBuilder 
{
    private static JObject spellData;
    private static System.Random random = new System.Random();

    private static readonly HashSet<string> KNOWN_BASE_SPELLS = new HashSet<string> 
    {
        "arcane_bolt",
        "magic_missile",
        "arcane_blast",
        "arcane_spray"
    };

    public static void Initialize(string jsonText)
    {
        spellData = JObject.Parse(jsonText);
    }

    public static Spell Build(string spellId, SpellCaster owner)
    {
        if (spellData == null)
        {
            Debug.LogError("SpellBuilder not initialized!");
            return null;
        }

        var spellJson = spellData[spellId];
        if (spellJson == null)
        {
            Debug.LogError($"No spell found with ID: {spellId}");
            return null;
        }

        return BuildFromJson(spellJson, spellId, owner);
    }

    public static Spell BuildRandom(SpellCaster owner)
    {
        Debug.Log("Building random spell...");
        
        // Get all spell IDs
        var spellIds = spellData.Properties().Select(p => p.Name).ToList();
        
        // First, get a list of base spells (non-modifiers)
        var baseSpells = spellIds.Where(id => KNOWN_BASE_SPELLS.Contains(id)).ToList();
        Debug.Log($"Found {baseSpells.Count} base spells: {string.Join(", ", baseSpells)}");
        
        if (baseSpells.Count == 0)
        {
            Debug.LogError("No base spells found in JSON!");
            return null;
        }
        
        // Start with a random base spell
        string baseSpellId = baseSpells[random.Next(baseSpells.Count)];
        Debug.Log($"Selected base spell: {baseSpellId}");
        var spell = Build(baseSpellId, owner);

        // Get list of modifier spells
        var modifierSpells = spellIds.Where(id => !KNOWN_BASE_SPELLS.Contains(id)).ToList();
        Debug.Log($"Found {modifierSpells.Count} modifier spells: {string.Join(", ", modifierSpells)}");
        
        // Randomly apply 0-2 modifiers
        int numModifiers = random.Next(3); // 0, 1, or 2
        Debug.Log($"Will apply {numModifiers} modifiers");
        
        var shuffledModifiers = modifierSpells.OrderBy(x => random.Next()).Take(numModifiers).ToList();
        foreach (var modId in shuffledModifiers)
        {
            Debug.Log($"Applying modifier: {modId}");
            var modifier = Build(modId, owner) as ModifierSpell;
            if (modifier != null)
            {
                modifier.SetInnerSpell(spell);
                spell = modifier;
            }
        }

        return spell;
    }

    private static Spell BuildFromJson(JToken spellJson, string spellId, SpellCaster owner)
    {
        if (spellJson == null)
        {
            Debug.LogError($"Spell {spellId} not found in JSON!");
            return null;
        }

        // Create the appropriate spell type
        Spell spell;
        bool isModifier = !KNOWN_BASE_SPELLS.Contains(spellId);
        Debug.Log($"Building spell {spellId} (isModifier: {isModifier})");

        if (isModifier)
        {
            spell = new ModifierSpell(owner, null);  // inner spell will be set later
        }
        else
        {
            spell = new ConcreteSpell(owner);
        }

        // Set the attributes
        spell.SetAttributes((JObject)spellJson);
        return spell;
    }

    public static JObject GetDefinition(string spellId)
    {
        if (!spellData.TryGetValue(spellId, out var def))
        {
            Debug.LogError($"SpellBuilder: no definition for '{spellId}'");
            return null;
        }
        return def as JObject;
    }

    public static IEnumerable<string> AllSpellIds()
    {
        return spellData.Properties().Select(p => p.Name);
    }

    public static void DebugPrintSpellData()
    {
        if (spellData == null)
        {
            Debug.Log("SpellBuilder not initialized!");
            return;
        }

        Debug.Log("All spells in JSON:");
        foreach (var prop in spellData.Properties())
        {
            bool isModifier = !KNOWN_BASE_SPELLS.Contains(prop.Name);
            Debug.Log($"- {prop.Name} (modifier: {isModifier})");
        }
    }
}
