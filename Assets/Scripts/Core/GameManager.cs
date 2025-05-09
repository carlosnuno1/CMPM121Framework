using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER,
        GAMEWIN
    }
    public GameState state;

    public int countdown;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    // Add these properties for spell scaling
    public int power { get; private set; } = 1;  // Start at power level 1
    public int wave { get; private set; } = 1;   // Start at wave 1

    // Method to update power based on wave number
    public void UpdatePower()
    {
        // "wave 10 *" from the requirements
        power = (int)RPNEvaluator.EvaluateRPNFloat("wave 10 *", 0, 0, wave);
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
    }

    public void ClearEnemies()
    {
        while(enemies.Count > 0)
        {
            UnityEngine.Object.Destroy(enemies[0]);
            enemies.RemoveAt(0);
        }
    }

    /*
    // Reference to the SpellRewardManager
    private SpellRewardManager spellRewardManager;

    void Start() // lol um this isnt a monobehaviour so it doesnt call start -kirsten
    {
        // Find the SpellRewardManager
        spellRewardManager = UnityEngine.Object.FindAnyObjectByType<SpellRewardManager>();
    }

    // Call this method when a wave is completed
    public void OnWaveCompleted()
    {
        // Change state to WAVEEND
        state = GameState.WAVEEND;
        
        // Show the spell reward
        if (spellRewardManager != null)
        {
            spellRewardManager.ShowSpellReward();
        }
        else
        {
            Debug.LogWarning("SpellRewardManager not found!");
            // Start the next wave directly if no reward manager is found
            StartNextWave();
        }
    }

    // Start the next wave
    public void StartNextWave()
    {
        // Increment the wave counter
        wave++;
        
        // Update power based on new wave number
        UpdatePower();
        
        // Update player stats based on wave number
        //UpdatePlayerStats(); (currently doing this in playercontroller)
        
        // Change state to COUNTDOWN
        state = GameState.COUNTDOWN;
        countdown = 3; // 3 second countdown
        
        // Your existing wave starting code...
    }
    
    // Add method to update player stats based on wave number
    private void UpdatePlayerStats()
    {
        if (player == null) return;
        
        // Get the player's components
        Hittable playerHittable = player.GetComponent<Hittable>();
        SpellCaster playerCaster = player.GetComponent<SpellCaster>();
        
        if (playerHittable != null)
        {
            // Set player HP: "95 wave 5 * +"
            int newMaxHP = (int)RPNEvaluator.EvaluateRPNFloat("95 wave 5 * +", 0, 0, wave);
            playerHittable.SetMaxHP(newMaxHP);
        }
        
        if (playerCaster != null)
        {
            // Set player mana: "90 wave 10 * +"
            playerCaster.max_mana = (int)RPNEvaluator.EvaluateRPNFloat("90 wave 10 * +", 0, 0, wave);
            
            // Set player mana regeneration: "10 wave +"
            playerCaster.mana_regen = RPNEvaluator.EvaluateRPNFloat("10 wave +", 0, 0, wave);
            
            // Set player spell power: "wave 10 *"
            playerCaster.power = (int)RPNEvaluator.EvaluateRPNFloat("wave 10 *", 0, 0, wave);
        }
        
        // Player speed remains constant at 5
        // If you have a movement component, you could set it here
    }
    */
}
