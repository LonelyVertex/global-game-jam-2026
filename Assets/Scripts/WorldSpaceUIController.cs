using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIController : MonoBehaviour
{
    public static WorldSpaceUIController Instance;

    public Camera worldCamera;
    public Camera uiCamera;
    public Canvas canvas;

    [Space]
    public DamageNumberController damageNumberControllerPrefab;

    [Space]
    public RectTransform playerHealthTransform;
    public Vector2 playerHealthOffset;
    public Slider playerHealth;
    public Slider dashCooldown;

    private readonly Queue<DamageNumberController> _inactiveQueue = new();
    private readonly Queue<DamageNumberController> _activeQueue = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PlayerStats.Instance == null)
        {
            return;
        }

        var screenPoint = worldCamera.WorldToScreenPoint(PlayerStats.Instance.transform.position);
        bool visible = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint,
            uiCamera,
            out var localPoint
        );

        if (visible)
        {
            playerHealthTransform.anchoredPosition = localPoint + playerHealthOffset;
            playerHealth.value = PlayerStats.Instance.hitpoints / PlayerStats.Instance.TotalMaxHitpoints();

            dashCooldown.gameObject.SetActive(PlayerController.Instance.currentDashCooldown > 0.0f);
            dashCooldown.value = PlayerController.Instance.currentDashCooldown / PlayerController.Instance.dashCooldown;
        }
    }

    private void LateUpdate()
    {
        if (!_activeQueue.TryPeek(out var c) || c.IsVisible)
        {
            return;
        }

        c.gameObject.SetActive(false);
        _activeQueue.Dequeue();

        _inactiveQueue.Enqueue(c);
    }

    public void DamageNumber(Vector3 worldPosition, int damage)
    {
        var screenPoint = worldCamera.WorldToScreenPoint(worldPosition);
        bool visible = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint,
            uiCamera,
            out var localPoint
        );

        if (!visible)
        {
            return;
        }

        if (!_inactiveQueue.TryDequeue(out var c))
        {
            c = Instantiate(damageNumberControllerPrefab, canvas.transform);
        }

        c.ShowNumber(localPoint, damage);

        c.gameObject.SetActive(true);
        _activeQueue.Enqueue(c);
    }
}
