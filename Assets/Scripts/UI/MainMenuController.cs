using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button playButton;

    public void OnEnable()
    {
        playButton.onClick.AddListener(HandlePlayButtonOnClick);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(HandlePlayButtonOnClick);
    }

    private void HandlePlayButtonOnClick()
    {
        FindFirstObjectByType<FadeController>().FadeOut();
    }
}
