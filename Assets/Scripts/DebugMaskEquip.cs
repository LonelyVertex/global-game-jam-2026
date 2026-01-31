using UnityEngine;

public class DebugMaskEquip : MonoBehaviour
{
    [SerializeField] private MaskInfo maskInfo;

    void Start()
    {
        GameManager.Instance.EquipMask(maskInfo);
    }
}
