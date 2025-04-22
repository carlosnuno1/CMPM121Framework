using UnityEngine;
using TMPro;

public class BetweenWaveUI : MonoBehaviour
{
    public TMP_Text waveText;
    public Hittable hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waveText = GetComponent<TMP_Text>();
        waveText.text = "Text working";
    }

    // Update is called once per frame
    void Update()
    {
        if (hp == null) return;
        waveText.text = "Current HP: " + hp.hp.ToString();
    }

    public void SetHealth(Hittable hp)
    {
        this.hp = hp;
    }
}
