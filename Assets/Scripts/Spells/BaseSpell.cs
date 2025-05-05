using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public abstract class BaseSpell : Spell
{
    // JSON-loaded expressions for scaling values
    protected string damageExpression;
    protected string manaCostExpression;
    protected float cooldown;
    
    // Projectile configuration
    protected string trajectory;
    protected string speedExpression;
    protected int projectileSprite;

    public BaseSpell(SpellCaster owner) : base(owner) { }

    public override string GetName() => name;
    public override string GetDescription() => description;
    public override int GetIcon() => icon;
    
    public override int GetManaCost(int power, int wave)
    {
        return (int)RPNEvaluator.EvaluateRPNFloat(manaCostExpression, 0, power, wave);
    }

    public override float GetCooldown() => cooldown;

    public override int GetDamage(int power, int wave)
    {
        return (int)RPNEvaluator.EvaluateRPNFloat(damageExpression, 0, power, wave);
    }

    public override void SetAttributes(JObject json)
    {
        // Basic attributes
        name = json["name"].ToString();
        description = json["description"].ToString();
        icon = json["icon"].Value<int>();

        // Expressions for scaling values
        var damage = json["damage"];
        damageExpression = damage["amount"].ToString();
        
        manaCostExpression = json["mana_cost"].ToString();
        cooldown = json["cooldown"].Value<float>();

        // Projectile configuration
        var proj = json["projectile"];
        trajectory = proj["trajectory"].ToString();
        speedExpression = proj["speed"].ToString();
        projectileSprite = proj["sprite"].Value<int>();
    }
}
