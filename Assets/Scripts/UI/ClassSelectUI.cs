using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectUI : MonoBehaviour
{
    public GameObject parentObj;
    public EnemySpawner spawner;
    public Image icon;
    public TextMeshProUGUI nameText;
    private Class myClass;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClass(Class classinfo)
    {
        if (name != null)
        {
            //icon
            GameManager.Instance.playerSpriteManager.PlaceSprite(classinfo.sprite, icon);
            //name
            nameText.text = classinfo.name;
            myClass = classinfo;
        }
    }

    public void PickClass()
    {
        GameManager.Instance.player.GetComponent<PlayerController>().playerclass = myClass;
        spawner.GenerateLevelSelectButtons();
        parentObj.SetActive(false);
    }
}
