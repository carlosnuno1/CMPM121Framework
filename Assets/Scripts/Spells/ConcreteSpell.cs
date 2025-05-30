using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ConcreteSpell : Spell
{
    public JObject spellJson { get; protected set; }
    protected SpellModifiers modifiers = new SpellModifiers();
    
    public ConcreteSpell(SpellCaster owner) : base(owner) { }

    public override void SetAttributes(JObject json)
    {
        spellJson = json;
    }

    public override string GetName() => spellJson["name"].ToString();
    public override string GetDescription() => spellJson["description"].ToString();
    public override int GetIcon() => spellJson["icon"].Value<int>();
    public override float GetCooldown() => 
        ValueModifier.ApplyModifiers(
            float.Parse(spellJson["cooldown"].ToString()),
            modifiers.cooldownModifiers
        );

    public override int GetManaCost(int power, int wave)
    {
        float baseCost = RPNEvaluator.EvaluateRPNFloat(
            spellJson["mana_cost"].ToString(),
            power, 0, wave
        );
        return (int)ValueModifier.ApplyModifiers(baseCost, modifiers.manaModifiers);
    }

    public override int GetDamage(int power, int wave)
    {
        var damage = spellJson["damage"];
        float baseDamage = RPNEvaluator.EvaluateRPNFloat(
            damage["amount"].ToString(),
            0,  // Don't use baseval for damage calculation
            power,
            wave
        );
        return (int)ValueModifier.ApplyModifiers(baseDamage, modifiers.damageModifiers);
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        Debug.Log($"Casting {GetName()} from {where} to {target}");
        
        // Record cast time
        last_cast = Time.time;
        
        // Get projectile configuration
        var proj = spellJson["projectile"];
        string trajectory = proj["trajectory"].ToString();
        if (!string.IsNullOrEmpty(modifiers.trajectoryOverride)) {
            trajectory = modifiers.trajectoryOverride;
        }
        
        float speed = RPNEvaluator.EvaluateRPNFloat(
            proj["speed"].ToString(),
            0, owner.power, GameManager.Instance.wave
        );
        speed = ValueModifier.ApplyModifiers(speed, modifiers.speedModifiers);
        
        int projectileSprite = proj["sprite"].Value<int>();
        
        // Calculate direction
        Vector3 direction = (target - where).normalized;
        
        // Get damage for the OnHit callback
        int damage = GetDamage(owner.power, GameManager.Instance.wave);
        
        // Create the projectile
        GameManager.Instance.projectileManager.CreateProjectile(
            projectileSprite,
            trajectory,
            where,
            direction,
            speed,
            (hittable, hitPosition) => {
                hittable.Damage(new Damage(damage, Damage.Type.ARCANE));
            }
        );
        
        // Apply any cast delay from modifiers
        if (modifiers.castDelay > 0) {
            yield return new WaitForSeconds(modifiers.castDelay);
        }
        
        yield break;
    }

    public void AddModifiers(SpellModifiers newModifiers)
    {
        modifiers.damageModifiers.AddRange(newModifiers.damageModifiers);
        modifiers.manaModifiers.AddRange(newModifiers.manaModifiers);
        modifiers.speedModifiers.AddRange(newModifiers.speedModifiers);
        modifiers.cooldownModifiers.AddRange(newModifiers.cooldownModifiers);
        
        if (!string.IsNullOrEmpty(newModifiers.trajectoryOverride))
        {
            modifiers.trajectoryOverride = newModifiers.trajectoryOverride;
        }
        
        modifiers.castDelay = newModifiers.castDelay;
        modifiers.splitAngle = newModifiers.splitAngle;
    }
}
