using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillOptionPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private SkillInfo _skillInfo;

    public void SetSkillInfo(SkillInfo skillInfo)
    {
        _skillInfo = skillInfo;

        nameText.text = skillInfo.skillName;
        descriptionText.text = skillInfo.description;
    }

    public void Take()
    {
        if (!_skillInfo) return;
        GameManager.Instance.TakeSkill(_skillInfo);
    }
}
