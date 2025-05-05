using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ModifierSpell : Spell
{
    protected Spell innerSpell;
    protected SpellModifiers modifiers = new SpellModifiers();
    protected string spellName = "";
    new protected string description = "";
    new protected int icon = 0;

    public ModifierSpell(SpellCaster owner) : base(owner)
    {
        // Initialize with empty modifiers
    }
    
    // Add this constructor to match what's expected by SpellBuilder
    public ModifierSpell(SpellCaster owner, JObject json) : base(owner)
    {
        SetAttributes(json);
    }
    
    // Add this constructor for when a spell is directly passed by ModifierSpellTester
    public ModifierSpell(SpellCaster owner, Spell innerSpell) : base(owner)
    {
        this.innerSpell = innerSpell;
    }

    // Static factory method to make it clear which constructor to use
    public static ModifierSpell FromJson(SpellCaster owner, JObject json)
    {
        return new ModifierSpell(owner, json);
    }

    // Static factory method to make it clear which constructor to use
    public static ModifierSpell WithInnerSpell(SpellCaster owner, Spell innerSpell)
    {
        return new ModifierSpell(owner, innerSpell);
    }

    public void SetInnerSpell(Spell spell)
    {
        innerSpell = spell;
    }

    public override void SetAttributes(JObject json)
    {
        // Parse name and description
        spellName = json["name"]?.ToString() ?? "Unnamed Modifier";
        description = json["description"]?.ToString() ?? "No description available.";
        
        // Parse icon
        if (json["icon"] != null)
        {
            int.TryParse(json["icon"].ToString(), out icon);
        }

        // Parse damage modifiers
        if (json["damage_multiplier"] != null)
        {
            float multiplier;
            if (float.TryParse(json["damage_multiplier"].ToString(), out multiplier))
            {
                modifiers.damageModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, multiplier));
            }
        }
        if (json["damage_bonus"] != null)
        {
            float bonus;
            if (float.TryParse(json["damage_bonus"].ToString(), out bonus))
            {
                modifiers.damageModifiers.Add(new ValueModifier(ValueModifier.ModType.Add, bonus));
            }
        }

        // Parse mana modifiers
        if (json["mana_multiplier"] != null)
        {
            float multiplier;
            if (float.TryParse(json["mana_multiplier"].ToString(), out multiplier))
            {
                modifiers.manaModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, multiplier));
            }
        }
        if (json["mana_bonus"] != null)
        {
            float bonus;
            if (float.TryParse(json["mana_bonus"].ToString(), out bonus))
            {
                modifiers.manaModifiers.Add(new ValueModifier(ValueModifier.ModType.Add, bonus));
            }
        }

        // Parse cooldown modifiers
        if (json["cooldown_multiplier"] != null)
        {
            float multiplier;
            if (float.TryParse(json["cooldown_multiplier"].ToString(), out multiplier))
            {
                modifiers.cooldownModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, multiplier));
            }
        }
        if (json["cooldown_bonus"] != null)
        {
            float bonus;
            if (float.TryParse(json["cooldown_bonus"].ToString(), out bonus))
            {
                modifiers.cooldownModifiers.Add(new ValueModifier(ValueModifier.ModType.Add, bonus));
            }
        }

        // Parse speed modifiers
        if (json["speed_multiplier"] != null)
        {
            float multiplier;
            if (float.TryParse(json["speed_multiplier"].ToString(), out multiplier))
            {
                modifiers.speedModifiers.Add(new ValueModifier(ValueModifier.ModType.Multiply, multiplier));
            }
        }

        // Parse trajectory override
        if (json["trajectory_override"] != null)
        {
            modifiers.trajectoryOverride = json["trajectory_override"].ToString();
        }

        // Parse cast delay
        if (json["cast_delay"] != null)
        {
            float.TryParse(json["cast_delay"].ToString(), out modifiers.castDelay);
        }

        // Parse split angle
        if (json["split_angle"] != null)
        {
            float.TryParse(json["split_angle"].ToString(), out modifiers.splitAngle);
        }
    }

    public override int GetDamage(int power, int wave)
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            return 0;
        }
        return Mathf.RoundToInt(ValueModifier.ApplyModifiers(innerSpell.GetDamage(power, wave), modifiers.damageModifiers));
    }

    public override int GetManaCost(int power, int wave)
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            return 0;
        }
        return Mathf.RoundToInt(ValueModifier.ApplyModifiers(innerSpell.GetManaCost(power, wave), modifiers.manaModifiers));
    }

    public override float GetCooldown()
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            return 0;
        }
        return ValueModifier.ApplyModifiers(innerSpell.GetCooldown(), modifiers.cooldownModifiers);
    }

    public override string GetName()
    {
        if (innerSpell == null) return spellName;
        return $"{spellName} {innerSpell.GetName()}";
    }

    public override string GetDescription()
    {
        if (innerSpell == null) return description;
        return $"{description}\n{innerSpell.GetDescription()}";
    }

    public override int GetIcon() => innerSpell?.GetIcon() ?? icon;

    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            yield break;
        }

        // Apply cast delay if specified
        if (modifiers.castDelay > 0)
        {
            yield return new WaitForSeconds(modifiers.castDelay);
        }

        // Apply split if angle is specified
        if (modifiers.splitAngle > 0)
        {
            // Calculate directions for split
            Vector3 direction = (target - where).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // First cast - original direction
            yield return innerSpell.Cast(where, target, team);
            
            // Second cast - angled direction
            float newAngle = angle + modifiers.splitAngle;
            Vector3 newDirection = new Vector3(
                Mathf.Cos(newAngle * Mathf.Deg2Rad),
                Mathf.Sin(newAngle * Mathf.Deg2Rad),
                0
            );
            Vector3 newTarget = where + newDirection * 10f; // Arbitrary distance
            yield return innerSpell.Cast(where, newTarget, team);
        }
        else
        {
            // Normal cast
            yield return innerSpell.Cast(where, target, team);
        }

        last_cast = Time.time;
    }
}
