using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class SplitterSpell : ModifierSpell
{
    private float splitAngle = 15f; // Default split angle in degrees

    public SplitterSpell(SpellCaster owner) : base(owner)
    {
        spellName = "Splitting";
        description = "Casts the spell in two different directions";
    }

    public SplitterSpell(SpellCaster owner, JObject json) : base(owner, json)
    {
        // Additional setup specific to SplitterSpell
    }

    public override void SetAttributes(JObject json)
    {
        base.SetAttributes(json);
        
        // Parse split angle if specified
        if (json["split_angle"] != null)
        {
            float.TryParse(json["split_angle"].ToString(), out splitAngle);
        }
        
        // Add the split angle to modifiers
        modifiers.splitAngle = splitAngle;
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (innerSpell == null)
        {
            Debug.LogError("SplitterSpell has no inner spell!");
            yield break;
        }

        // Record cast time
        last_cast = Time.time;

        // Calculate directions for split
        Vector3 direction = (target - where).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // First cast - original direction
        yield return innerSpell.Cast(where, target, team);
        
        // Second cast - angled direction
        float newAngle = angle + splitAngle;
        Vector3 newDirection = new Vector3(
            Mathf.Cos(newAngle * Mathf.Deg2Rad),
            Mathf.Sin(newAngle * Mathf.Deg2Rad),
            0
        );
        Vector3 newTarget = where + newDirection * 10f; // Arbitrary distance
        yield return innerSpell.Cast(where, newTarget, team);
    }
}