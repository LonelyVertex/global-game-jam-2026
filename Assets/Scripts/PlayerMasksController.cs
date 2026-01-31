using System.Collections.Generic;
using UnityEngine;

public class PlayerMasksController : MonoBehaviour
{
    public static PlayerMasksController Instance { get; private set; }

    [SerializeField] private Transform masksRoot;
    [SerializeField] private float maskPlacementIncrement;

    private readonly Dictionary<MaskInfo, Transform> _masks = new();

    private float _zPosition;

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

    public void EquipMask(MaskInfo maskInfo)
    {
        if (!_masks.TryGetValue(maskInfo, out var maskTransform))
        {
            _masks[maskInfo] = maskTransform = Instantiate(maskInfo.playerMaskPrefab, masksRoot).transform;
            maskTransform.localPosition = new Vector3(0.0f, 0.0f, _zPosition);

            _zPosition += maskPlacementIncrement;
        }

        /*maskTransform.localScale =
            Vector3.one + (PlayerStats.Instance.GetMaskCount(maskInfo) * new Vector3(0.1f, 0.1f, 0.1f));*/
    }
}
