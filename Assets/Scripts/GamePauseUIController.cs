using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePauseUIController : MonoBehaviour
{
    public Button resumeButton;

    public event Action OnFinishedEvent;


    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        resumeButton.onClick.AddListener(() => OnFinishedEvent?.Invoke());
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveAllListeners();
    }
}
