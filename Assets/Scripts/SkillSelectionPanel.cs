using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] Transform optionsParent;
    [SerializeField] GameObject optionsPrefab;
    [SerializeField] SkillsList list;

    [SerializeField] private InputActionReference[] slotInputs;

    public void GenerateOptions()
    {
        ClearOptions();

        var options = Utils.Shuffle(list.skills).Take(3).ToList();

        for (int i = 0; i < options.Count; i++)
        {
            var skillInfo = options[i];

            var optionPanelObj = Instantiate(optionsPrefab, optionsParent);
            var optionPanel = optionPanelObj.GetComponent<SkillOptionPanel>();
            optionPanel.SetSkillInfo(i + 1, skillInfo, slotInputs[i].action);
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
