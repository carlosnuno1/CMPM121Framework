using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ArcaneExplosionSpell : ConcreteSpell
{
    private int secondaryProjectileCount = 8;
    private int secondaryDamage;

    public ArcaneExplosionSpell(SpellCaster owner) : base(owner)
    {
        // Initialize with default values
    }

    public ArcaneExplosionSpell(SpellCaster owner, JObject json) : base(owner)
    {
        // Set attributes from JSON
        SetAttributes(json);
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse secondary projectile count if specified
        if (json["N"] != null)
        {
            string countExpression = json["N"].ToString();
            secondaryProjectileCount = Mathf.RoundToInt(RPNEvaluator.EvaluateRPNFloat(
                countExpression, 0, owner.power, GameManager.Instance.wave));
        }
        
        // Parse secondary damage if specified
        if (json["secondary_damage"] != null)
        {
            string damageExpression = json["secondary_damage"].ToString();
            secondaryDamage = Mathf.RoundToInt(RPNEvaluator.EvaluateRPNFloat(
                damageExpression, 0, owner.power, GameManager.Instance.wave));
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        Debug.Log($"Casting {GetName()} from {where} to {target}");
        
        // Record cast time
        last_cast = Time.time;
        
        // Get primary projectile configuration
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
        
        // Get secondary projectile configuration
        var secondaryProj = spellJson["secondary_projectile"];
        string secondaryTrajectory = secondaryProj["trajectory"].ToString();
        
        float secondarySpeed = RPNEvaluator.EvaluateRPNFloat(
            secondaryProj["speed"].ToString(),
            0, owner.power, GameManager.Instance.wave
        );
        
        float secondaryLifetime = 0.3f;
        if (secondaryProj["lifetime"] != null)
        {
            float.TryParse(secondaryProj["lifetime"].ToString(), out secondaryLifetime);
        }
        
        int secondarySprite = secondaryProj["sprite"].Value<int>();
        
        // Calculate direction
        Vector3 direction = (target - where).normalized;
        
        // Get damage for the OnHit callback
        int damage = GetDamage(owner.power, GameManager.Instance.wave);
        
        // Create the primary projectile with a special OnHit callback
        GameManager.Instance.projectileManager.CreateProjectile(
            projectileSprite,
            trajectory,
            where,
            direction,
            speed,
            (hittable, hitPosition) => {
                // Deal damage to the hit target
                hittable.Damage(new Damage(damage, Damage.Type.ARCANE));
                
                // Spawn secondary projectiles in a circular pattern
                for (int i = 0; i < secondaryProjectileCount; i++)
                {
                    float angle = (360f / secondaryProjectileCount) * i;
                    Vector3 secondaryDirection = new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad),
                        0
                    );
                    
                    GameManager.Instance.projectileManager.CreateProjectile(
                        secondarySprite,
                        secondaryTrajectory,
                        hitPosition,
                        secondaryDirection,
                        secondarySpeed,
                        (secondaryHittable, secondaryHitPosition) => {
                            secondaryHittable.Damage(new Damage(secondaryDamage, Damage.Type.ARCANE));
                        },
                        secondaryLifetime
                    );
                }
            }
        );
        
        yield break;
    }
}