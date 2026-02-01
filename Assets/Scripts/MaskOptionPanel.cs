using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MaskOptionPanel : MonoBehaviour
{
    [SerializeField] private Image maskImage;
    [SerializeField] private TMP_Text maskNameText;
    [SerializeField] private TMP_Text maskDescriptionText;
    [SerializeField] private Image maskEquipModeGO;
    [SerializeField] private TMP_Text maskEquipModeText;
    [SerializeField] private TMP_Text slotText;

    [Space]
    [SerializeField] private Color blueColor;
    [SerializeField] private Color redColor;
    [SerializeField] private Color purpleColor;

    private MaskInfo _maskInfo;
    private InputAction _inputAction;
    private EMaskEquipMode _maskEquipMode;

    private void OnDestroy()
    {
        if (_inputAction == null)
        {
            return;
        }

        _inputAction.performed -= HandleInputAction;
    }

    public void SetMaskInfo(int idx, MaskInfo maskInfo, InputAction inputAction)
    {
        _maskInfo = maskInfo;
        _inputAction = inputAction;
        _maskEquipMode = ChooseMaskEquipMode();

        maskImage.sprite = maskInfo.icon;
        maskNameText.text = maskInfo.maskName;
        maskDescriptionText.text = maskInfo.description;
        slotText.SetText(idx.ToString());
        SetMaskEquipModeText();

        _inputAction.Enable();
        inputAction.performed += HandleInputAction;
    }

    private EMaskEquipMode ChooseMaskEquipMode()
    {
        if (!PlayerStats.Instance.HasMask(_maskInfo))
        {
            return EMaskEquipMode.newMask;
        }

        var modes = new List<EMaskEquipMode>();

        modes.Add(EMaskEquipMode.damage);

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
        {
            modes.Add(EMaskEquipMode.cooldown);
        }

        if (_maskInfo.spawnType != MaskInfo.ESpawnType.melee)
        {
            modes.Add(EMaskEquipMode.projectileCount);
        }

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.target)
        {
            modes.Add(EMaskEquipMode.projectileBounce);
        }

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
        {
            modes.Add(EMaskEquipMode.orbitalSpeed);
        }

        return modes[Random.Range(0, modes.Count)];
    }

    private void SetMaskEquipModeText()
    {
        maskEquipModeGO.gameObject.SetActive(_maskEquipMode != EMaskEquipMode.newMask);

        switch (_maskEquipMode)
        {
            case EMaskEquipMode.damage:
                maskEquipModeText.text = "+ damage";
                maskEquipModeGO.color = redColor;
                break;
            case EMaskEquipMode.cooldown:
                maskEquipModeText.text = "- cooldown";
                maskEquipModeGO.color = blueColor;
                break;
            case EMaskEquipMode.projectileCount:
                maskEquipModeText.text = "+ projectile count";
                maskEquipModeGO.color = purpleColor;
                break;
            case EMaskEquipMode.projectileBounce:
                maskEquipModeText.text = "+ bounce";
                maskEquipModeGO.color = purpleColor;
                break;
            case EMaskEquipMode.orbitalSpeed:
                maskEquipModeText.text = "+ orbital speed";
                maskEquipModeGO.color = purpleColor;
                break;
        }
    }

    public void Take()
    {
        if (!_maskInfo) return;

        GameManager.Instance.EquipMask(_maskInfo, _maskEquipMode);
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
