using UnityEngine;

public class FadeInOnLevelStart : MonoBehaviour
{
    private void Start()
    {
        FindFirstObjectByType<FadeController>().FadeIn();
    }
}
