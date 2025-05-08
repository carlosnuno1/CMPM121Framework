using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SpellRewardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rewardPanel;
    public TextMeshProUGUI spellNameText;
    public TextMeshProUGUI spellDescriptionText;
    public SpellUI demoSpell;
    public Image spellIconImage;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI cooldownText;
    
    [Header("Buttons")]
    public Button acceptButton;
    public Button rejectButton;
    public GameObject MainMenuButton;
    
    [Header("Spell Selection")]
    public GameObject spellSelectionPanel;
    public Button[] spellSelectionButtons;
    public SpellUI[] spellSelectionUIs;
    
    private Spell currentRewardSpell;
    private SpellCaster playerCaster;

    private int spellcheck; // to make sure you only generate 1 spell per wave break
    
    void Start()
    {
        // Hide the panels initially
        rewardPanel.SetActive(false);
        if (spellSelectionPanel != null)
            spellSelectionPanel.SetActive(false);
        
        // Get reference to player's spell caster
        playerCaster = GameManager.Instance.player.GetComponent<SpellCaster>();
        
        // Set up button listeners
        acceptButton.onClick.AddListener(AcceptSpell);
        rejectButton.onClick.AddListener(RejectSpell);
        
        // Set up spell selection buttons
        if (spellSelectionButtons != null)
        {
            for (int i = 0; i < spellSelectionButtons.Length; i++)
            {
                int index = i; // Capture the index for the lambda
                spellSelectionButtons[i].onClick.AddListener(() => ReplaceSpell(index));
            }
        }
        spellcheck = 0;
    }
    // Update is called once per frame
    void Update()
    {

        if (rewardPanel == null)
        {
            return;
        }
        if (GameManager.Instance.state == GameManager.GameState.GAMEWIN)
        {
            rewardPanel.SetActive(true);
            MainMenuButton.SetActive(true);
        }
        else if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            rewardPanel.SetActive(true);
            MainMenuButton.SetActive(false);
            if (spellcheck == 0) {
                spellcheck = 1;
                ShowSpellReward();
            }
        }
        else if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            rewardPanel.SetActive(true);
            MainMenuButton.SetActive(true);
        }
        else
        {
            rewardPanel.SetActive(false);
            spellcheck = 0;
        }
    }
    
    // This method can be called from GameManager when a wave is completed
    public void ShowSpellReward()
    {
        Debug.Log("Generating Spell Reward");
        // Generate a random spell
        currentRewardSpell = SpellBuilder.BuildRandom(playerCaster);
        
        // Update UI with spell details
        spellNameText.text = currentRewardSpell.GetName();
        spellDescriptionText.text = currentRewardSpell.GetDescription();
        
        /*
        // Get spell icon
        int iconIndex = currentRewardSpell.GetIcon();
        // Use a placeholder approach since SpellIconManager doesn't have GetIcon method
        spellIconImage.sprite = GameManager.Instance.spellIconManager.Get(iconIndex);
        
        // Update stats
        int power = playerCaster.power;
        int wave = GameManager.Instance.wave;
        
        damageText.text = $"{currentRewardSpell.GetDamage(power, wave)}";
        manaText.text = $"{currentRewardSpell.GetManaCost(power, wave)}";
        */
        cooldownText.text = $"Cooldown: {currentRewardSpell.GetCooldown():F1}s";
        demoSpell.SetSpell(currentRewardSpell, 0);
        
        // Show the panel
        rewardPanel.SetActive(true);
        
        // Pause the game while showing reward
        Time.timeScale = 0;
    }
    
    private void AcceptSpell()
    {
        // Check if player has room for another spell
        if (playerCaster.GetSpellCount() >= 4)
        {
            // Player already has max spells, show spell selection UI
            ShowSpellSelectionUI();
        }
        else
        {
            // Add the spell directly
            AddSpellToPlayer(currentRewardSpell);
            CloseRewardPanel();
        }
    }
    
    private void RejectSpell()
    {
        // Simply close the panel and continue to next wave
        CloseRewardPanel();
    }
    
    private void CloseRewardPanel()
    {
        // Hide the panel
        rewardPanel.SetActive(false);
        
        // Resume the game
        Time.timeScale = 1;
        
        // Continue to next wave
        GameManager.Instance.StartNextWave();
    }
    
    private void ShowSpellSelectionUI()
    {
        // Hide the reward panel
        rewardPanel.SetActive(false);
        
        // Show the spell selection panel
        spellSelectionPanel.SetActive(true);
        
        // Update the spell selection UIs to show current spells
        for (int i = 0; i < spellSelectionUIs.Length; i++)
        {
            if (i < playerCaster.GetSpellCount())
            {
                Spell spell = playerCaster.GetSpell(i);
                spellSelectionUIs[i].gameObject.SetActive(true);
                spellSelectionUIs[i].SetSpell(spell, i);
            }
            else
            {
                spellSelectionUIs[i].gameObject.SetActive(false);
            }
        }
    }
    
    private void ReplaceSpell(int index)
    {
        // Replace the selected spell with the new one
        if (index >= 0 && index < playerCaster.GetSpellCount())
        {
            playerCaster.ReplaceSpell(index, currentRewardSpell);
            
            // Update the UI
            UpdateSpellUI();
            
            // Close the selection panel
            spellSelectionPanel.SetActive(false);
            
            // Resume the game
            Time.timeScale = 1;
            
            // Continue to next wave
            GameManager.Instance.StartNextWave();
        }
    }
    
    private void AddSpellToPlayer(Spell spell)
    {
        // Add the spell to the player's spell list
        playerCaster.AddSpell(spell);
        
        // Update the UI
        UpdateSpellUI();
    }
    
    private void UpdateSpellUI()
    {
        // Find and update the spell UI container
        SpellUIContainer container = GameManager.Instance.player.GetComponent<SpellUIContainer>();
        if (container != null)
        {
            container.UpdateSpellUI();
        }
    }
}