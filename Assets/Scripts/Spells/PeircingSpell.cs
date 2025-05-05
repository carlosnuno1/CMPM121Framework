using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class PiercingSpell : ModifierSpell
{
    private int pierceCount = 2; // Number of enemies the projectile can pierce

    public PiercingSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Piercing";
        description = "Allows projectiles to pass through enemies";
    }

    public PiercingSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to PiercingSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse pierce count if specified
        if (json["pierce_count"] != null)
        {
            int.TryParse(json["pierce_count"].ToString(), out pierceCount);
        }
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (innerSpell == null)
        {
            Debug.LogError("PiercingSpell has no inner spell!");
            yield break;
        }

        // Record cast time
        last_cast = Time.time;

        // Get projectile configuration from inner spell
        // This is a simplified approach - in a real implementation, you'd need to 
        // extract this information from the inner spell or pass it through
        
        // Create a special OnHit callback that allows piercing
        System.Action<Hittable, Vector3> piercingOnHit = (hittable, hitPosition) => {
            // Deal damage to the hit target
            int damage = GetDamage(owner.power, GameManager.Instance.wave);
            hittable.Damage(new Damage(damage, Damage.Type.ARCANE));
            
            // The projectile continues after hitting (handled by ProjectileManager)
            // This would require modifying ProjectileManager to support piercing
        };
        
        // Cast the inner spell with our piercing behavior
        // In a real implementation, you'd need to modify how projectiles are created
        // to support piercing through multiple enemies
        yield return innerSpell.Cast(where, target, team);
    }
}