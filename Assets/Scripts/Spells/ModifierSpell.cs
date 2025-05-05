using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ModifierSpell : Spell
{
    private Spell innerSpell;
    private float damageMultiplier = 1.0f;
    private float manaMultiplier = 1.0f;
    private float cooldownMultiplier = 1.0f;
    private int damageBonus = 0;
    private int manaBonus = 0;
    private float cooldownBonus = 0f;
    private string spellName = "";
    private string description = "";
    private int icon = 0;

    public ModifierSpell(SpellCaster owner, Spell innerSpell) : base(owner)
    {
        this.innerSpell = innerSpell;
    }

    public void SetInnerSpell(Spell spell)
    {
        innerSpell = spell;
    }

    public override void SetAttributes(JObject json)
    {
        // Parse name and description
        spellName = json["name"]?.ToString() ?? "Unnamed Spell";
        description = json["description"]?.ToString() ?? "No description available.";
        
        // Parse icon
        if (json["icon"] != null)
        {
            int.TryParse(json["icon"].ToString(), out icon);
        }

        // Parse multipliers (default to 1.0 if not specified)
        if (json["damage_multiplier"] != null)
        {
            float.TryParse(json["damage_multiplier"].ToString(), out damageMultiplier);
        }
        if (json["mana_multiplier"] != null)
        {
            float.TryParse(json["mana_multiplier"].ToString(), out manaMultiplier);
        }
        if (json["cooldown_multiplier"] != null)
        {
            float.TryParse(json["cooldown_multiplier"].ToString(), out cooldownMultiplier);
        }

        // Parse flat bonuses (default to 0 if not specified)
        if (json["damage_bonus"] != null)
        {
            int.TryParse(json["damage_bonus"].ToString(), out damageBonus);
        }
        if (json["mana_bonus"] != null)
        {
            int.TryParse(json["mana_bonus"].ToString(), out manaBonus);
        }
        if (json["cooldown_bonus"] != null)
        {
            float.TryParse(json["cooldown_bonus"].ToString(), out cooldownBonus);
        }
    }

    public override int GetDamage(int power, int wave)
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            return 0;
        }
        return Mathf.RoundToInt(innerSpell.GetDamage(power, wave) * damageMultiplier) + damageBonus;
    }

    public override int GetManaCost(int power, int wave)
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            return 0;
        }
        return Mathf.RoundToInt(innerSpell.GetManaCost(power, wave) * manaMultiplier) + manaBonus;
    }

    public override float GetCooldown()
    {
        if (innerSpell == null)
        {
            Debug.LogError("ModifierSpell has no inner spell!");
            return 0;
        }
        return innerSpell.GetCooldown() * cooldownMultiplier + cooldownBonus;
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

        yield return innerSpell.Cast(where, target, team);

        last_cast = Time.time;
    }
}
