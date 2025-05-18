using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Relic
{
    public string name;
    public int sprite;
    public Trigger trigger;
    public Effect effect;

    public string GetLabel()
    {
        return "100";
    }
    public string GetName()
    {
        return name;
    }
    public string GetDescription()
    {
        return trigger.description + " " + effect.description;
    }
    public bool IsActive()
    {
        return false;
    }

    public void DoEffect()
    {
        switch (effect.type)
        {
            case "gain-mana":
                GainMana(effect.amount);
                return;
            case "gain-spellpower":
                GainSpellpower(effect.amount);
                return;
            default:
                // uh oh
                return;
        }
    }
    private void GainMana(string amount)
    {
        //
    }
    private void GainSpellpower(string amount)
    {
        //
    }
}