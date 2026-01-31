using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthBarLabel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBarSlider.value = PlayerStats.Instance.hitpoints / PlayerStats.Instance.maxHitpoints;
        healthBarLabel.text = PlayerStats.Instance.hitpoints.ToString("F0") + " / " + PlayerStats.Instance.maxHitpoints.ToString("F0");
    }
}
