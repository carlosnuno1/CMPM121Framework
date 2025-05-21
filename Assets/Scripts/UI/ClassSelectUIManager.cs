using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class ClassSelectUIManager : MonoBehaviour
{
    public EnemySpawner enemyspawner;
    public GameObject classUIPrefab;
    public Dictionary<string, Class> classes;
    public GameObject parentObj; // Reference to the parent object to enable/disable

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load classes from JSON
        DeserializeClasses();
        
        // Generate class select buttons
        GenerateClassSelectButtons();
        
        // Show class select UI
        gameObject.SetActive(true);
        
        // set pregrame state
        GameManager.Instance.state = GameManager.GameState.PREGAME;
    }

    void DeserializeClasses()
    {
        classes = new Dictionary<string, Class>();
        var classText = Resources.Load<TextAsset>("classes");
        if (classText == null)
        {
            Debug.LogError("Failed to load classes.json from Resources!");
            return;
        }

        JToken jo = JToken.Parse(classText.text);
        foreach (var prop in (JObject)jo)
        {
            string className = prop.Key;
            var classData = prop.Value;
            
            Class classObj = new Class
            {
                name = className,
                health = classData["health"].ToString(),
                mana = classData["mana"].ToString(),
                mana_regeneration = classData["mana_regeneration"].ToString(),
                spellpower = classData["spellpower"].ToString(),
                speed = classData["speed"].ToString(),
                sprite = classData["sprite"].Value<int>()
            };
            
            if (className == "mage") classObj.sprite = 0;
            else if (className == "warrior") classObj.sprite = 1;
            else if (className == "rogue") classObj.sprite = 2;
            
            classes[className] = classObj;
        }
    }

    void GenerateClassSelectButtons()
    {
        int offset = 0;
        foreach (var (k,v) in classes)
        {
            GameObject selector = Instantiate(classUIPrefab, gameObject.transform);
            selector.transform.localPosition = new Vector3(offset - 300, 0, 0);
            selector.GetComponent<ClassSelectUI>().spawner = enemyspawner;
            selector.GetComponent<ClassSelectUI>().parentObj = gameObject;
            selector.GetComponent<ClassSelectUI>().SetClass(v);
            offset = offset + 250;
        }
    }
}
