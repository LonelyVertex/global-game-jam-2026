using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillOptionPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private SkillInfo _skillInfo;
    private InputAction _inputAction;

    public void SetSkillInfo(SkillInfo skillInfo, InputAction inputAction)
    {
        _skillInfo = skillInfo;
        _inputAction = inputAction;

        nameText.text = skillInfo.skillName;
        descriptionText.text = skillInfo.description;

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
        _inputAction.performed -= HandleInputAction;
        _inputAction.Disable();

        Take();
    }
}
