using System.Collections.Generic;
using UnityEngine;

public class MasksProvider : MonoBehaviour
{
    public static MasksProvider Instance { get; private set; }

    public RectTransform masksRoot;

    public int Version { get; private set; } = 0;

    private readonly Dictionary<MaskInfo, Transform> _masksProvider = new();

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
        if (!_masksProvider.TryGetValue(maskInfo, out Transform maskObject))
        {
            maskObject = _masksProvider[maskInfo] = Instantiate(maskInfo.screenMaskPrefab, masksRoot).transform;
        }

        MarkDirty();
    }

    private void MarkDirty()
    {
        Version++;
    }

    private void Start()
    {
        MarkDirty();
    }

    private void OnValidate()
    {
        MarkDirty();
    }
}
