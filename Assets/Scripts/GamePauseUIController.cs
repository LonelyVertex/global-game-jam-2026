using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseUIController : MonoBehaviour
{
    public Button resumeButton;
    public Button restartButton;

    public event Action OnFinishedEvent;


    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        resumeButton.onClick.AddListener(() => OnFinishedEvent?.Invoke());
        restartButton.onClick.AddListener(() => SceneManager.LoadScene("Scenes/Kofo"));
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
    }
}
