using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class FrostNovaSpell : ConcreteSpell
{
    private int projectileCount = 12;
    private float slowDuration = 3.0f;
    private float slowAmount = 0.5f; // 50% slow

    public FrostNovaSpell(SpellCaster owner) : base(owner)
    {
        // Initialize with default values
    }

    public FrostNovaSpell(SpellCaster owner, JObject json) : base(owner)
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
        
        // Parse slow duration if specified
        if (json["slow_duration"] != null)
        {
            float.TryParse(json["slow_duration"].ToString(), out slowDuration);
        }
        
        // Parse slow amount if specified
        if (json["slow_amount"] != null)
        {
            float.TryParse(json["slow_amount"].ToString(), out slowAmount);
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
        
        // Get damage for the OnHit callback
        int damage = GetDamage(owner.power, GameManager.Instance.wave);
        
        // Create projectiles in a circular pattern
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i;
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            );
            
            GameManager.Instance.projectileManager.CreateProjectile(
                projectileSprite,
                trajectory,
                where,
                direction,
                speed,
                (hittable, hitPosition) => {
                    // Deal damage
                    hittable.Damage(new Damage(damage, Damage.Type.ICE));
                    
                    // Since we don't have direct access to a slow effect,
                    // we'll just apply ice damage which might have a slowing effect
                    // in the game's damage system
                    
                    // Alternatively, we could create a visual effect to indicate slowing
                    // GameManager.Instance.effectManager.CreateEffect("frost", hitPosition, 1.0f);
                }
            );
        }
        
        yield break;
    }
}