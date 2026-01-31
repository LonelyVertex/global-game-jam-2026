using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class FadeController : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    [Space]
    public float duration;

    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(1.0f, 0.0f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(0.0f, 1.0f));
    }

    private IEnumerator FadeCoroutine(float start, float end)
    {
        canvasGroup.alpha = start;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, t / duration);

            yield return null;
        }

        canvasGroup.alpha = end;
    }
}
