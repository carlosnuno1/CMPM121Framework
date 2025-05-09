using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellUIContainer : MonoBehaviour
{
    public PlayerController player;
    public SpellUI[] spellUIs;
    public SpellCaster spellCaster;
    
    void Start()
    {
        spellCaster = player.gameObject.GetComponent<SpellCaster>();
        
        // Initialize spell UIs
        spellUIs = gameObject.GetComponentsInChildren<SpellUI>(true); // these disappear at runtime? so im adding them back here
        UpdateSpellUI();
    }
    
    public void UpdateSpellUI()
    {
        // Get the list of spells from the SpellCaster
        List<Spell> spells = new List<Spell>();
        Debug.Log(spellCaster.GetSpellCount());
        for (int i = 0; i < spellCaster.GetSpellCount(); i++)
        {
            spells.Add(spellCaster.GetSpell(i));
        }
        
        // Update each spell UI based on the player's spells
        for (int i = 0; i < spellUIs.Length; i++)
        {
            if (i < spells.Count)
            {
                // Show and update this spell UI
                spellUIs[i].gameObject.SetActive(true);
                spellUIs[i].SetSpell(spells[i], i);
            }
            else
            {
                // Hide this spell UI
                spellUIs[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void DropSpell(int index)
    {
        // Check if the index is valid
        if (index >= 0 && index < spellCaster.GetSpellCount())
        {
            // Remove the spell
            spellCaster.RemoveSpell(index);
            
            // Update the UI
            UpdateSpellUI();
        }
    }
    public void SwitchHighlight(int index)
    {
        // Check if the index is valid
        if (index >= 0 && index < spellCaster.GetSpellCount())
        {
            // set everything else to unhighlighted
            for (int i = 0; i < spellCaster.GetSpellCount(); i++)
            {
                spellUIs[i].highlight.SetActive(false);
            }
            // highlight the spell
            spellUIs[index].highlight.SetActive(true);
        }
    }
}
