// replaced by SpellRewardManager.cs ... delete this?
using UnityEngine;
using TMPro;
public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    public TMP_Text waveText;
    public Hittable hp;
    public GameObject MainMenuButton;
    public GameObject NextWaveButton;


    // to instantiate
    public GameObject button;
    public GameObject sprite;

    void Awake()
    {        
        if (waveText == null && rewardUI != null)
        {
            waveText = rewardUI.GetComponentInChildren<TMP_Text>(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (rewardUI == null || waveText == null)
        {
            return;
        }
        if (GameManager.Instance.state == GameManager.GameState.GAMEWIN)
        {
            rewardUI.SetActive(true);
            MainMenuButton.SetActive(true);
            NextWaveButton.SetActive(false);
            waveText.text = "You Win! \n All Waves Completed";
        }
        else if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            rewardUI.SetActive(true);
            MainMenuButton.SetActive(false);
            NextWaveButton.SetActive(true);
            if (hp == null) return;
            waveText.text = "Current HP: " + hp.hp.ToString();
        }
        else if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            rewardUI.SetActive(true);
            MainMenuButton.SetActive(true);
            NextWaveButton.SetActive(false);
            GameManager.Instance.ClearEnemies();
            waveText.text = "Game Over";
        }
        else
        {
            rewardUI.SetActive(false);
        }
    }

    public void SetHealth(Hittable hp)
    {
        this.hp = hp;
    }
}
