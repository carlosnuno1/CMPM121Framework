using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneBoltSpell : ConcreteSpell
{
    public ArcaneBoltSpell(SpellCaster owner) : base(owner)
    {
        // Initialize with default values
    }

    public ArcaneBoltSpell(SpellCaster owner, JObject json) : base(owner)
    {
        // Set attributes from JSON
        SetAttributes(json);
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
        
        yield break;
    }
}