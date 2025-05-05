using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;

public class BaseSpellTester : MonoBehaviour
{
    // Test implementation of BaseSpell
    private class TestSpell : BaseSpell
    {
        public TestSpell(SpellCaster owner) : base(owner) { }

        public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
        {
            Debug.Log($"[TestSpell] Cast at {where} targeting {target}");
            yield break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Create a test JSON definition similar to arcane_bolt
        string testJson = @"{
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

        // Create a temporary spell instance
        var owner = gameObject.AddComponent<SpellCaster>();
        var spell = new TestSpell(owner);
        spell.SetAttributes(JObject.Parse(testJson));

        // Test basic attributes
        Debug.Log($"[BaseSpellTester] Name: {spell.GetName()}");
        Debug.Log($"[BaseSpellTester] Description: {spell.GetDescription()}");
        Debug.Log($"[BaseSpellTester] Icon: {spell.GetIcon()}");
        Debug.Log($"[BaseSpellTester] Cooldown: {spell.GetCooldown()}");

        // Test scaling with different power/wave values
        TestScaling(spell, 1, 1);  // Starting values
        TestScaling(spell, 10, 1); // Higher power
        TestScaling(spell, 20, 2); // Higher power and wave

        // Test casting
        StartCoroutine(spell.Cast(Vector3.zero, Vector3.right, Hittable.Team.PLAYER));
    }

    void TestScaling(BaseSpell spell, int power, int wave)
    {
        Debug.Log($"\n[BaseSpellTester] Testing with power={power}, wave={wave}:");
        Debug.Log($"  Damage: {spell.GetDamage(power, wave)}");
        Debug.Log($"  Mana Cost: {spell.GetManaCost(power, wave)}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
