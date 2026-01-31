using UnityEngine;
using UnityEngine.SceneManagement;
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

        Invoke(nameof(StartLevel), 1.0f);
    }

    private void StartLevel()
    {
        SceneManager.LoadScene("Scenes/Kofo");
    }
}
