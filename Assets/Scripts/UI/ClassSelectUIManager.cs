using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ClassSelectUIManager : MonoBehaviour
{
    public EnemySpawner enemyspawner;
    public GameObject classUIPrefab;
    public Dictionary<string, Class> classes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateClassSelectButtons()
    {
        int offset = 0;
        foreach (var (k,v) in classes)
        {
            GameObject selector = Instantiate(classUIPrefab, gameObject.transform);
            selector.transform.localPosition = new Vector3(130 - offset, 0);
            selector.GetComponent<ClassSelectUI>().spawner = enemyspawner;
            selector.GetComponent<ClassSelectUI>().parentObj = gameObject;
            selector.GetComponent<ClassSelectUI>().SetClass(v);
            offset = offset + 50;
        }
    }
}
