using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicRewardIconManager : MonoBehaviour
{
    
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Relic relic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        if (relic != null)
        {
            //icon
            GameManager.Instance.relicIconManager.PlaceSprite(relic.sprite, icon);
            //name
            nameText.text = relic.GetName();
            //description
            descriptionText.text = relic.GetDescription();
        }
    }
}
