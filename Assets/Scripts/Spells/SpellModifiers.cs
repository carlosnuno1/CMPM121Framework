using System.Collections.Generic;
using UnityEngine;

public class SpellModifiers
{
    public List<ValueModifier> damageModifiers = new List<ValueModifier>();
    public List<ValueModifier> manaModifiers = new List<ValueModifier>();
    public List<ValueModifier> speedModifiers = new List<ValueModifier>();
    public List<ValueModifier> cooldownModifiers = new List<ValueModifier>();
    
    // Behavior modifications
    public string trajectoryOverride;
    public float castDelay;
    public float splitAngle;

    public SpellModifiers Clone()
    {
        var clone = new SpellModifiers();
        clone.damageModifiers.AddRange(damageModifiers);
        clone.manaModifiers.AddRange(manaModifiers);
        clone.speedModifiers.AddRange(speedModifiers);
        clone.cooldownModifiers.AddRange(cooldownModifiers);
        clone.trajectoryOverride = trajectoryOverride;
        clone.castDelay = castDelay;
        clone.splitAngle = splitAngle;
        return clone;
    }
}
