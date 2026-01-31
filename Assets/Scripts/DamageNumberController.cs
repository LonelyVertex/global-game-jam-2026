using TMPro;
using UnityEngine;

public class DamageNumberController : MonoBehaviour
{
    public TextMeshProUGUI damageNumber;

    [Space]
    public RectTransform rectTransform;

    [Space]
    public float duration;

    public AnimationCurve upCurve;
    public AnimationCurve alphaCurve;
    public AnimationCurve scaleCurve;

    public bool IsVisible { get; private set; }

    private float _currentDuration;

    public void ShowNumber(Vector2 screenPoint, int damage)
    {
        rectTransform.anchoredPosition = screenPoint;

        damageNumber.text = damage.ToString();

        _currentDuration = 0.0f;
    }

    private void Update()
    {
        _currentDuration += Time.deltaTime;
        IsVisible = _currentDuration <= duration;

        float t =  _currentDuration / duration;

        float s = scaleCurve.Evaluate(t);
        rectTransform.anchoredPosition += new Vector2(0.0f, upCurve.Evaluate(t));
        rectTransform.localScale = Vector3.one + new Vector3(s, s, 0.0f);

        damageNumber.alpha = alphaCurve.Evaluate(t);
    }
}
