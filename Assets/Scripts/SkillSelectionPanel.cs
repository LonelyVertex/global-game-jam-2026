using System.Linq;
using UnityEngine;

public class SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] Transform optionsParent;
    [SerializeField] GameObject optionsPrefab;
    [SerializeField] SkillsList list;

    public void GenerateOptions()
    {
        ClearOptions();

        var options = Utils.Shuffle(list.skills).Take(3);

        foreach (var skillInfo in options)
        {
            var optionPanelObj = Instantiate(optionsPrefab, optionsParent);
            var optionPanel = optionPanelObj.GetComponent<SkillOptionPanel>();
            optionPanel.SetSkillInfo(skillInfo);
        }
    }

    private void ClearOptions()
    {
        for (int i = optionsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(optionsParent.GetChild(i).gameObject);
        }
    }
}
