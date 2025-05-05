using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneSpraySpell : ConcreteSpell
{
    private int projectileCount = 5;
    private float spreadAngle = 30f;

    public ArcaneSpraySpell(SpellCaster owner) : base(owner)
    {
        // Initialize with default values
        // We'll need to see how ConcreteSpell handles these properties
    }

    public ArcaneSpraySpell(SpellCaster owner, JObject json) : base(owner)
    {
        // Set attributes from JSON
        SetAttributes(json);
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse projectile count if specified
        if (json["N"] != null)
        {
            string countExpression = json["N"].ToString();
            projectileCount = Mathf.RoundToInt(RPNEvaluator.EvaluateRPNFloat(
                countExpression, 0, owner.power, GameManager.Instance.wave));
        }
        
        // Parse spread angle if specified
        if (json["spread_angle"] != null)
        {
            float.TryParse(json["spread_angle"].ToString(), out spreadAngle);
        }
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
        
        // Calculate base direction
        Vector3 baseDirection = (target - where).normalized;
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
        
        // Get damage for the OnHit callback
        int damage = GetDamage(owner.power, GameManager.Instance.wave);
        
        // Create multiple projectiles in a spray pattern
        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate angle for this projectile
            float angleOffset = Random.Range(-spreadAngle/2, spreadAngle/2);
            float angle = baseAngle + angleOffset;
            
            // Calculate direction from angle
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            );
            
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
            
            // Small delay between projectiles for visual effect
            yield return new WaitForSeconds(0.05f);
        }
        
        yield break;
    }
}