using System.Collections.Generic;
using UnityEngine;

public class MasksProvider : MonoBehaviour
{
    public static MasksProvider Instance { get; private set; }

    public RectTransform masksRoot;

    public int Version { get; private set; } = 0;

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
        Instantiate(maskInfo.screenMaskPrefab, masksRoot);

        MarkDirty();
    }

    public void HideMasks()
    {
        masksRoot.gameObject.SetActive(false);
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
