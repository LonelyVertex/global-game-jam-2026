using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthBarLabel;
    [SerializeField] private TextMeshProUGUI killsCounter;
    [SerializeField] private TextMeshProUGUI timeCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerStats.Instance == null || GameManager.Instance == null) return;

        healthBarSlider.value = PlayerStats.Instance.hitpoints / PlayerStats.Instance.maxHitpoints;
        healthBarLabel.text = PlayerStats.Instance.hitpoints.ToString("F0") + " / " + PlayerStats.Instance.maxHitpoints.ToString("F0");
        killsCounter.text = GameManager.Instance.totalKills.ToString();
        //print time in seconds formatted to mm:ss
        float totalTime = GameManager.Instance.totalTime;
        int minutes = Mathf.FloorToInt(totalTime / 60F);
        int seconds = Mathf.FloorToInt(totalTime - minutes * 60);
        timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
