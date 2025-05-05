using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI manaText;
    public Image cooldownImage;
    public Button dropButton;
    
    private Spell spell;
    private SpellCaster caster;
    private int spellIndex;
    
    void Start()
    {
        // Find the spell caster
        caster = GetComponentInParent<SpellCaster>();
        
        // Set up drop button
        if (dropButton != null)
        {
            dropButton.onClick.AddListener(DropSpell);
        }
    }
    
    public void SetSpell(Spell newSpell, int index)
    {
        spell = newSpell;
        spellIndex = index;
        
        // Update UI elements
        if (nameText != null)
            nameText.text = spell.GetName();
        
        // Get spell icon
        if (icon != null)
        {
            int iconIndex = spell.GetIcon();
            // Check how to get the icon from the icon index
            // icon.sprite = GameManager.Instance.spellIconManager.GetIcon(iconIndex);
            // For now, let's use a placeholder approach
            icon.sprite = Resources.Load<Sprite>($"SpellIcons/icon_{iconIndex}");
        }
        
        // Update stats
        UpdateStats();
    }
    
    // Overload for backward compatibility
    public void SetSpell(Spell newSpell)
    {
        SetSpell(newSpell, 0);
    }
    
    void Update()
    {
        if (spell != null)
        {
            // Update cooldown display
            if (cooldownImage != null)
            {
                float cooldownTime = spell.GetCooldown();
                // Calculate cooldown remaining based on last cast time and current time
                float timeSinceLastCast = Time.time - spell.GetLastCastTime();
                float cooldownRemaining = Mathf.Max(0, cooldownTime - timeSinceLastCast);
                float cooldownPercentage = Mathf.Clamp01(cooldownRemaining / cooldownTime);
                cooldownImage.fillAmount = cooldownPercentage;
            }
            
            // Update stats periodically (in case power changes)
            if (Time.frameCount % 30 == 0)
            {
                UpdateStats();
            }
        }
    }
    
    private void UpdateStats()
    {
        if (caster != null && spell != null)
        {
            int power = caster.power;
            int wave = GameManager.Instance.wave;
            
            if (damageText != null)
                damageText.text = $"DMG: {spell.GetDamage(power, wave)}";
            
            if (manaText != null)
                manaText.text = $"MP: {spell.GetManaCost(power, wave)}";
        }
    }
    
    private void DropSpell()
    {
        // Get the SpellUIContainer and drop this spell
        SpellUIContainer container = GetComponentInParent<SpellUIContainer>();
        if (container != null)
        {
            container.DropSpell(spellIndex);
        }
    }
}
