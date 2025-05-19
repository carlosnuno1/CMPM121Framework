using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    public Spell spell;
    public Hittable.Team team;
    
    public float mana;
    public float max_mana;
    public float mana_regen;
    public int power;
    
    // List to store the player's spells
    private List<Spell> spells = new List<Spell>();

    void Start()
    {
        mana = max_mana;
    }

    void Update()
    {
        if (mana < max_mana)
        {
            mana += mana_regen * Time.deltaTime;
            if (mana > max_mana)
            {
                mana = max_mana;
            }
        }
    }

    public bool CanCast()
    {
        if (spell == null) return false;
        // Now passing power and wave from GameManager
        return spell.IsReady() && mana >= spell.GetManaCost(power, GameManager.Instance.wave);
    }

    public bool Cast(Vector3 where, Vector3 target)
    {
        if (spell == null) return false;
        
        if (!spell.IsReady()) return false;
        
        int manaCost = spell.GetManaCost(power, GameManager.Instance.wave);
        if (mana < manaCost) return false;
        
        mana -= manaCost;
        
        StartCoroutine(spell.Cast(where, target, team));
        EventBus.Instance.DoCastSpell();
        
        return true;
    }

    // Get the number of spells the player has
    public int GetSpellCount()
    {
        return spells.Count;
    }

    // Get a spell at a specific index
    public Spell GetSpell(int index)
    {
        if (index >= 0 && index < spells.Count)
        {
            return spells[index];
        }
        return null;
    }

    // Add a new spell to the player's collection
    public void AddSpell(Spell spell)
    {
        spells.Add(spell);
    }

    // Remove a spell at a specific index
    public void RemoveSpell(int index)
    {
        if (index >= 0 && index < spells.Count)
        {
            spells.RemoveAt(index);
        }
    }

    // Replace a spell at a specific index with a new spell
    public void ReplaceSpell(int index, Spell newSpell)
    {
        if (index >= 0 && index < spells.Count)
        {
            spells[index] = newSpell;
        }
    }
}
