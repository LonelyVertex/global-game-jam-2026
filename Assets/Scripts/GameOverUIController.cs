using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    public Button restartButton;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killText;

    public event Action OnFinishedEvent;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);

        restartButton.onClick.AddListener(() => OnFinishedEvent?.Invoke());

        timeText.text = GameManager.Instance.totalTime.ToString();
        killText.text = GameManager.Instance.totalKills.ToString();
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
    }
}
