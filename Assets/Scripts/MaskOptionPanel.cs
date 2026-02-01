using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MaskOptionPanel : MonoBehaviour
{
    [SerializeField] private Image maskImage;
    [SerializeField] private TMP_Text maskNameText;
    [SerializeField] private TMP_Text maskDescriptionText;

    private MaskInfo _maskInfo;
    private InputAction _inputAction;

    private void OnDestroy()
    {
        if (_inputAction == null)
        {
            return;
        }

        _inputAction.performed -= HandleInputAction;
    }

    public void SetMaskInfo(MaskInfo maskInfo, InputAction inputAction)
    {
        _maskInfo = maskInfo;
        _inputAction = inputAction;

        maskImage.sprite = maskInfo.icon;
        maskNameText.text = maskInfo.maskName;
        maskDescriptionText.text = maskInfo.description;

        _inputAction.Enable();
        inputAction.performed += HandleInputAction;
    }

    public void Take()
    {
        if (!_maskInfo) return;

        GameManager.Instance.EquipMask(_maskInfo);
    }

    private void HandleInputAction(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        _inputAction.performed -= HandleInputAction;
        _inputAction.Disable();

        Take();
    }
}
