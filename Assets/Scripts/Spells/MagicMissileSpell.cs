using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class MagicMissileSpell : ConcreteSpell
{
    public MagicMissileSpell(SpellCaster owner) : base(owner)
    {
        // Initialize with default values
    }

    public MagicMissileSpell(SpellCaster owner, JObject json) : base(owner)
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
        
        // Magic missile always uses homing trajectory
        string trajectory = "homing";
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
        
        // Create the homing projectile
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