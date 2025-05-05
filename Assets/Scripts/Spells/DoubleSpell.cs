using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class DoublerSpell : ModifierSpell
{
    private float castDelay = 0.5f; // Default delay in seconds

    public DoublerSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Doubling";
        description = "Casts the spell twice with a short delay";
    }

    public DoublerSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to DoublerSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse cast delay if specified
        if (json["cast_delay"] != null)
        {
            float.TryParse(json["cast_delay"].ToString(), out castDelay);
        }
        
        // Add the cast delay to modifiers
        modifiers.castDelay = castDelay;
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (innerSpell == null)
        {
            Debug.LogError("DoublerSpell has no inner spell!");
            yield break;
        }

        // Record cast time
        last_cast = Time.time;

        // First cast
        yield return innerSpell.Cast(where, target, team);
        
        // Wait for the delay
        yield return new WaitForSeconds(castDelay);
        
        // Second cast
        yield return innerSpell.Cast(where, target, team);
    }
}