using UnityEngine;
using TMPro;

public class BetweenWaveMenu : MonoBehaviour
{
    public TMP_Text waveText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveText = GetComponent<TMP_Text>();
        waveText.text = "Text working";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
