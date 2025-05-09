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
    public TextMeshProUGUI cooldownText;

    [Header("Buttons")]
    public Button acceptButton;
    public Button rejectButton;
    public GameObject MainMenuButton;

    // there should be a better way of getting these at runtime
    public SpellCaster playerCaster;
    public SpellUIContainer container;
    public EnemySpawner enemyspawner;

    private int spellcheck; // to make sure you only generate 1 spell per wave break
    private Spell currentRewardSpell;

    void Start()
    {
        // Hide the panels initially
        rewardPanel.SetActive(false);
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
            spellNameText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            rewardPanel.SetActive(true);
            MainMenuButton.SetActive(false);
            spellNameText.gameObject.transform.parent.gameObject.SetActive(true);
            if (spellcheck == 0) {
                
                // Get reference to player's spell caster
                playerCaster = GameManager.Instance.player.GetComponent<SpellCaster>();
                spellcheck = 1;
                ShowSpellReward();
            }
        }
        else if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            rewardPanel.SetActive(true);
            MainMenuButton.SetActive(true);
            spellNameText.gameObject.transform.parent.gameObject.SetActive(false);
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
        cooldownText.text = $"Cooldown: {currentRewardSpell.GetCooldown():F1}s";
        demoSpell.SetSpell(currentRewardSpell, 0);
        
        // Show the panel
        rewardPanel.SetActive(true);
        
        // Pause the game while showing reward
        Time.timeScale = 0;
    }
    
    public void AcceptSpell()
    {
        // Check if player has room for another spell
        if (playerCaster.GetSpellCount() >= 4)
        {
            // Player already has max spells, tell them to drop a spell
        }
        else
        {
            // Add the spell directly
            AddSpellToPlayer(currentRewardSpell);
            CloseRewardPanel();
        }
    }
    
    public void RejectSpell()
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
        enemyspawner.NextWave();
        //GameManager.Instance.StartNextWave();
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
        //SpellUIContainer container = GameManager.Instance.player.GetComponent<SpellUIContainer>();
        if (container != null)
        {
            container.UpdateSpellUI();
        }
    }
}