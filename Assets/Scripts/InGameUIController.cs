using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthBarLabel;
    [SerializeField] private TextMeshProUGUI killsCounter;
    [SerializeField] private TextMeshProUGUI timeCounter;
    [SerializeField] private GameObject masksGrid;
    [SerializeField] private GameObject maskPrefab;
    [SerializeField] private TextMeshProUGUI levelsCounter;
    [SerializeField] private Slider xpBarSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private int masksCount = -1;
    void Start()
    {
        masksCount = -1;
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

        if (masksCount != PlayerStats.Instance._equippedMasks.Count)
        {
            UpdateMasksUI();
            masksCount = PlayerStats.Instance._equippedMasks.Count;
        }
        levelsCounter.text = PlayerStats.Instance.currentLevel.ToString();

        var exNeededForPreviousLevel = PlayerStats.Instance.currentLevel > 1 ? PlayerStats.Instance.TotalXpForLevel(PlayerStats.Instance.currentLevel) : 0;
        xpBarSlider.value = (PlayerStats.Instance.xp - exNeededForPreviousLevel) / PlayerStats.Instance.XpToNextLevel(PlayerStats.Instance.currentLevel);

    }
    private void UpdateMasksUI()
    {
        // Clear existing masks
        foreach (Transform child in masksGrid.transform)
        {
            Debug.Log("Destroying mask UI element.");
            Destroy(child.gameObject);
        }

        // Add current masks
        foreach (var maskEntry in PlayerStats.Instance._equippedMasks)
        {
            Debug.Log("Creating mask UI element.");
            GameObject maskUI = Instantiate(maskPrefab, masksGrid.transform);
            //Image maskImage = maskUI.GetComponent<Image>();
            //maskImage.sprite = maskEntry.Key.icon;
        }
    }
}
