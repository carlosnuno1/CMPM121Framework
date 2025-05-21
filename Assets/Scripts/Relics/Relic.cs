using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Relic
{
    public string name;
    public int sprite;
    public Trigger trigger;
    public Effect effect;
    public bool active = false;

    public string GetLabel()
    {
        return "";
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
        return active;
    }

    public void StartListening() // have the relic listen for its trigger
    {
        switch (trigger.type)
        {
            case "take-damage":
                EventBus.Instance.OnTakeDamage += DoEffect;
                return;
            case "stand-still":
                EventBus.Instance.OnStandStill += DoEffect;
                return;
            case "on-kill":
                EventBus.Instance.OnKill += DoEffect;
                return;
            case "spell-switch":
                EventBus.Instance.OnSpellSwitch += DoEffect;
                return;
            default:
                // uh oh
                return;
        }
    }
    public void DoEffect()
    {
        EffectType();
    }

    public void DoEffect(int amount)
    {
        // check if it satisfies additional trigger conditions
        if (trigger.amount != null &&
            RPNEvaluator.EvaluateRPN(trigger.amount, 0, GameManager.Instance.wave) > amount)
        {
            return;
        }
        EffectType();
    }
    private void EffectType()
    {
        switch (effect.type)
        {
            case "gain-mana":
                GainMana();
                return;
            case "gain-spellpower":
                GainSpellpower();
                return;
            case "gain-speed":
                GainSpeed();
                return;
            default:
                // uh oh
                return;
        }
    }
    private void GainMana()
    {
        GameManager.Instance.player.GetComponent<SpellCaster>().mana += RPNEvaluator.EvaluateRPN(effect.amount, 0, GameManager.Instance.wave);
    }
    private void GainSpellpower()
    {
        if (!IsActive())
        {
            active = true;
            GameManager.Instance.player.GetComponent<SpellCaster>().power += RPNEvaluator.EvaluateRPN(effect.amount, 0, GameManager.Instance.wave);
            switch (effect.until)
            {
                case "move":
                    EventBus.Instance.OnMove += Deactivate;
                    break;
                case "cast-spell":
                    EventBus.Instance.OnCastSpell += Deactivate;
                    break;
                default:
                    break;
            }
        }
    }
    private void GainSpeed()
    {
        if (!IsActive())
        {
            active = true;
            GameManager.Instance.player.GetComponent<PlayerController>().speed += RPNEvaluator.EvaluateRPN(effect.amount, 0, GameManager.Instance.wave);
            //RPNEvaluator.EvaluateRPN(effect.duration, 0, GameManager.Instance.wave)
            switch (effect.until)
            {
                case "move":
                    EventBus.Instance.OnMove += Deactivate;
                    break;
                case "cast-spell":
                    EventBus.Instance.OnCastSpell += Deactivate;
                    break;
                case "spell-switch":
                    EventBus.Instance.OnSpellSwitch += Deactivate;
                    return;
                default:
                    break;
            }
        }
    }
    private void Deactivate()
    {
        if (IsActive())
        {
            active = false;
            switch (effect.until)
            {
                case "move":
                    EventBus.Instance.OnMove -= Deactivate;
                    break;
                case "cast-spell":
                    EventBus.Instance.OnCastSpell -= Deactivate;
                    break;
                case "spell-switch":
                    EventBus.Instance.OnSpellSwitch -= Deactivate;
                    return;
                default:
                    break;
            }
            switch (effect.type)
            {
                case "gain-mana":
                    return;
                case "gain-spellpower":
                    GameManager.Instance.player.GetComponent<SpellCaster>().power -= RPNEvaluator.EvaluateRPN(effect.amount, 0, GameManager.Instance.wave); // what if this happens between waves?
                    return;
                case "gain-speed":
                    GameManager.Instance.player.GetComponent<PlayerController>().speed -= RPNEvaluator.EvaluateRPN(effect.amount, 0, GameManager.Instance.wave);
                    return;
                default:
                    return;
            }
        }
    }
}