using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillOptionPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private TMP_Text slotText;

    [SerializeField] private Color hpColor;
    [SerializeField] private Color armorColor;
    [SerializeField] private Color damangeColor;
    [SerializeField] private Color attackSpeedColor;
    [SerializeField] private Color movementColor;


    private SkillInfo _skillInfo;
    private InputAction _inputAction;

    private void OnDestroy()
    {
        if (_inputAction == null)
        {
            return;
        }

        _inputAction.performed -= HandleInputAction;
    }

    public void SetSkillInfo(int idx, SkillInfo skillInfo, InputAction inputAction)
    {
        _skillInfo = skillInfo;
        _inputAction = inputAction;

        image.sprite = skillInfo.icon;
        nameText.text = skillInfo.skillName;
        descriptionText.text = skillInfo.description;
        slotText.text = idx.ToString();

        SetTotalText();
        SetTextColor();

        _inputAction.Enable();
        inputAction.performed += HandleInputAction;
    }

    public void Take()
    {
        if (!_skillInfo) return;
        GameManager.Instance.TakeSkill(_skillInfo);
    }

    private void HandleInputAction(InputAction.CallbackContext obj)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        _inputAction.performed -= HandleInputAction;
        _inputAction.Disable();

        Take();
    }

    private void SetTotalText()
    {
        var stats = PlayerStats.Instance;
        switch (_skillInfo.type)
        {
            case SkillInfo.SkillType.movementSpeed:
                totalText.text = $"{stats.movementSpeed + stats.movementSpeedBonus:F1}";
                break;
            case SkillInfo.SkillType.armor:
                totalText.text = $"{stats.armor:F1}";
                break;
            case SkillInfo.SkillType.evasion:
                totalText.text = $"{stats.evasion:F1}";
                break;
            case SkillInfo.SkillType.regeneration:
                totalText.text = $"{stats.hitpointsRegen + stats.regenerationBonus:F1}";
                break;
            case SkillInfo.SkillType.attack_speed:
                totalText.text = $"{stats.attackSpeed}";
                break;
            case SkillInfo.SkillType.hitpoints:
                totalText.text = $"{stats.maxHitpoints + stats.hitpointsBonus}";
                break;
            case SkillInfo.SkillType.damage:
                totalText.text = $"{stats.GetDamangeScaler():F1}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetTextColor()
    {
        switch (_skillInfo.type)
        {
            case SkillInfo.SkillType.movementSpeed:
                descriptionText.color = movementColor;
                break;
            case SkillInfo.SkillType.armor:
            case SkillInfo.SkillType.evasion:
                descriptionText.color = armorColor;
                break;
            case SkillInfo.SkillType.attack_speed:
                descriptionText.color = attackSpeedColor;
                break;
            case SkillInfo.SkillType.hitpoints:
            case SkillInfo.SkillType.regeneration:
                descriptionText.color = hpColor;
                break;
            case SkillInfo.SkillType.damage:
                descriptionText.color = damangeColor;
                break;
        }
    }
}
