using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    public Spell spell;
    public float mana;
    public float max_mana;
    public float mana_regen;
    public Hittable.Team team;
    
    void Start()
    {
        mana = max_mana;
    }

    void Update()
    {
        mana = Mathf.Min(mana + mana_regen * Time.deltaTime, max_mana);
    }

    public bool CanCast()
    {
        if (spell == null) return false;
        // Now passing power and wave from GameManager
        return spell.IsReady() && mana >= spell.GetManaCost(GameManager.Instance.power, GameManager.Instance.wave);
    }

    public void Cast(Vector3 where, Vector3 target)
    {
        if (!CanCast()) return;
        
        // Get mana cost with current power/wave
        int cost = spell.GetManaCost(GameManager.Instance.power, GameManager.Instance.wave);
        mana -= cost;
        
        StartCoroutine(spell.Cast(where, target, team));
    }
}
