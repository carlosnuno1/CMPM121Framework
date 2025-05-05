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
}
