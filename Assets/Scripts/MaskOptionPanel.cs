using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskOptionPanel : MonoBehaviour
{
    [SerializeField] private Image maskImage;
    [SerializeField] private TMP_Text maskNameText;
    [SerializeField] private TMP_Text maskDescriptionText;

    private MaskInfo _maskInfo;

    public void SetMaskInfo(MaskInfo maskInfo)
    {
        _maskInfo = maskInfo;

        maskNameText.text = maskInfo.name;
        maskDescriptionText.text = maskInfo.description;
    }

    public void Take()
    {
        if (!_maskInfo) return;

        GameManager.Instance.EquipMask(_maskInfo);
    }
}
