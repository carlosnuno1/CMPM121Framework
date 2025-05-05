using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ExplosiveSpell : ModifierSpell
{
    private float explosionRadius = 2.0f;
    private float explosionDamageMultiplier = 0.5f; // Explosion deals 50% of the original damage

    public ExplosiveSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Explosive";
        description = "Adds area damage when projectiles hit";
    }

    public ExplosiveSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to ExplosiveSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse explosion radius if specified
        if (json["explosion_radius"] != null)
        {
            float.TryParse(json["explosion_radius"].ToString(), out explosionRadius);
        }
        
        // Parse explosion damage multiplier if specified
        if (json["explosion_damage_multiplier"] != null)
        {
            float.TryParse(json["explosion_damage_multiplier"].ToString(), out explosionDamageMultiplier);
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (innerSpell == null)
        {
            Debug.LogError("ExplosiveSpell has no inner spell!");
            yield break;
        }

        // Record cast time
        last_cast = Time.time;

        // Create a custom OnHit callback that adds explosion effect
        System.Action<Hittable, Vector3> explosiveOnHit = (hittable, hitPosition) => {
            // Deal damage to the hit target
            int damage = GetDamage(owner.power, GameManager.Instance.wave);
            hittable.Damage(new Damage(damage, Damage.Type.ARCANE));
            
            // Create explosion effect
            // Find all enemies in explosion radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(hitPosition, explosionRadius);
            foreach (Collider2D collider in colliders)
            {
                Hittable target = collider.GetComponent<Hittable>();
                if (target != null && target != hittable && target.team != team)
                {
                    // Calculate explosion damage
                    int explosionDamage = Mathf.RoundToInt(damage * explosionDamageMultiplier);
                    target.Damage(new Damage(explosionDamage, Damage.Type.ARCANE));
                }
            }
            
            // Visual effect for explosion (could be added here)
            // GameManager.Instance.effectManager.CreateExplosionEffect(hitPosition, explosionRadius);
        };
        
        // Cast the inner spell with our explosive behavior
        // In a real implementation, you'd need to modify how projectiles are created
        // to support the custom OnHit callback
        yield return innerSpell.Cast(where, target, team);
    }
}