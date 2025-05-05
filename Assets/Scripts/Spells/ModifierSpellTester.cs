using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;

public class ModifierSpellTester : MonoBehaviour
{
    // Make TestSpell available to ModifierSpellTester
    public class TestSpell : BaseSpell
    {
        public TestSpell(SpellCaster owner) : base(owner) { }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
        {
            Debug.Log($"[TestSpell] Cast at {where} targeting {target}");
            yield break;
        }
    }

    void Start()
    {
        // Create a test base spell
        var owner = gameObject.AddComponent<SpellCaster>();
        var baseSpell = new TestSpell(owner);
        
        // Create test JSON for base spell
        string baseJson = @"{
            'name': 'Test Bolt',
            'description': 'A test spell.',
            'icon': 0,
            'damage': {
                'amount': '25 power 5 / +',
                'type': 'arcane'
            },
            'mana_cost': '10 power 2 / +',
            'cooldown': '2.5',
            'projectile': {
                'trajectory': 'straight',
                'speed': '8 power 5 / +',
                'sprite': 0
            }
        }";
        baseSpell.SetAttributes(JObject.Parse(baseJson));

        // Test base spell values
        Debug.Log("[ModifierTest] Base Spell:");
        TestSpellValues(baseSpell, 10, 1);

        // Create a modifier spell
        var modSpell = new ModifierSpell(owner, baseSpell);
        
        // Create test JSON for modifier
        string modJson = @"{
            'damage_mod': 2.0,
            'mana_mod': 5.0,
            'speed_mod': 1.5,
            'cooldown_mod': 0.8
        }";
        modSpell.SetAttributes(JObject.Parse(modJson));

        // Test modified spell values
        Debug.Log("\n[ModifierTest] Modified Spell:");
        TestSpellValues(modSpell, 10, 1);

        // Test casting
        StartCoroutine(modSpell.Cast(Vector3.zero, Vector3.right, Hittable.Team.PLAYER));
    }

    void TestSpellValues(Spell spell, int power, int wave)
    {
        Debug.Log($"Name: {spell.GetName()}");
        Debug.Log($"Damage: {spell.GetDamage(power, wave)}");
        Debug.Log($"Mana Cost: {spell.GetManaCost(power, wave)}");
        Debug.Log($"Cooldown: {spell.GetCooldown()}");
    }
}
