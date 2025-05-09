using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;

    public Dictionary<string, Enemy> enemy_types;
    public Dictionary<string, Level> level_types;
    public int current_wave;
    public string current_level;

    private int spawns; // for preventing early wave ends when still spawning

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // read json files (in separate function)
        // can we put the duplicate code between these 2 into yet another separate function --------! todo
        DeserializeEnemies();
        DeserializeLevels();

        // can we move this to a separate function? ------------------------------------------------! todo
        int offset = 0;
        foreach (var (k,v) in level_types)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, 130 - offset);
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(k);
            offset = offset + 50;
        }
    }

    void DeserializeEnemies()
    {
        enemy_types = new Dictionary<string, Enemy>();
        var enemytext = Resources.Load<TextAsset>("enemies");
        JToken jo = JToken.Parse(enemytext.text);
        foreach (var enemy in jo)
        {
            Enemy en = enemy.ToObject<Enemy>();
            enemy_types[en.name] = en;
        }
    }
    void DeserializeLevels()
    {
        level_types = new Dictionary<string, Level>();
        var leveltext = Resources.Load<TextAsset>("levels");
        JToken jo = JToken.Parse(leveltext.text);
        foreach (var level in jo)
        {
            Level a = level.ToObject<Level>();
            level_types[a.name] = a;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        current_wave = 1; // i kind of want this to start at zero.
        if (!level_types.ContainsKey(levelname)) Debug.Log("Invalid levelname");
        current_level = levelname;
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        current_wave = current_wave + 1;
        // dont start a new wave if current_wave > level_types[current_level].waves ----------------! todo
        StartCoroutine(SpawnWave());
    }

    public void MainMenu()
    {
        GameManager.Instance.state = GameManager.GameState.PREGAME;
        level_selector.gameObject.SetActive(true);

    }

    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        
        spawns = 0;
        // spawn each type of monster simultaneously (separate coroutines)
        foreach (Spawn s in level_types[current_level].spawns)
        {
            spawns = spawns + 1;
            StartCoroutine(SpawnEnemyType(s));
        }
        yield return new WaitWhile(() => spawns > 0);
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        if (GameManager.Instance.state != GameManager.GameState.GAMEOVER)
        {
            if (current_wave >= level_types[current_level].waves && level_types[current_level].name != "Endless")
            {
                GameManager.Instance.state = GameManager.GameState.GAMEWIN;
            }
            else
            {
                GameManager.Instance.state = GameManager.GameState.WAVEEND;
            }
        }
    }

    IEnumerator SpawnEnemyType(Spawn s)
    {
        // default sequence is [1]
        // default delay is 2
        int count = RPNEvaluator.EvaluateRPN(s.count, 0, current_wave);
        List<int> sequence = new List<int>();
        sequence.Add(1);
        int delay = 2;
        if (s.sequence != null) sequence = s.sequence;
        int sequence_pointer = 0;
        int i = 0;
        // spawn up to the amount available in current sequence to add up to the count, separated by the delay
        while (i < count)
        {
            // checking that current sequence value wont cause total spawns to exceed count
            int amt = sequence[sequence_pointer];
            if (amt > count - i) amt = count - i;
            // spawn the enemies
            yield return SpawnEnemy(s, amt, delay);
            // update sequence/count tracking
            sequence_pointer = (sequence_pointer + 1) % sequence.Count();
            i = i + amt;
        }
        spawns = spawns - 1;
    }
    private SpawnPoint EvaluateSpawnLocation(string s)
    {
        string[] tokens = s.Split(' ');
        SpawnPoint outPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        if (tokens.Length < 2) return outPoint; // default to random if only one word, or no entry
        SpawnPoint.SpawnName type = SpawnPoint.SpawnName.RED;
        switch(tokens[1])
        {
            case "red":
                type = SpawnPoint.SpawnName.RED;
                break;
            case "green":
                type = SpawnPoint.SpawnName.GREEN;
                break;
            case "bone":
                type = SpawnPoint.SpawnName.BONE;
                break;
        }
        // REPLACE this later (this will cause an infinite loop if there isnt a valid spawn in SpawnPoints)
        while (outPoint.kind != type) { outPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)]; }
        return outPoint;
    }

    IEnumerator SpawnEnemy(Spawn s, int sequence_amount, int delay)
    {
        for (int i = 0; i < sequence_amount; i++)
        {
            // parse spawn location
            SpawnPoint spawn_point = EvaluateSpawnLocation(s.location);
            Vector2 offset = Random.insideUnitCircle * 1.8f;
                    
            Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);
            new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemy_types[s.enemy].sprite);
            EnemyController en = new_enemy.GetComponent<EnemyController>();

            en.hp = new Hittable(RPNEvaluator.EvaluateRPN(s.hp, enemy_types[s.enemy].hp, current_wave), Hittable.Team.MONSTERS, new_enemy);
            en.speed = enemy_types[s.enemy].speed;
            //en.damage = enemy_types[s.enemy].damage; this causes an error. i am unsure where we're supposed to implement it otherwise though
            GameManager.Instance.AddEnemy(new_enemy);
        }
        yield return new WaitForSeconds(delay);
    }
}
