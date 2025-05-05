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
        
        return true;
    }
}
