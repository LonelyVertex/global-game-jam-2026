using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskSelectionPanel : MonoBehaviour
{
    [SerializeField] Transform maskOptionsParent;
    [SerializeField] GameObject maskOptionsPrefab;
    [SerializeField] MaskList maskList;

    [SerializeField] private InputActionReference[] slotInputs;

    public void GenerateOptions()
    {
        ClearOptions();

        var options = Utils.Shuffle(maskList.masks).Take(3).ToList();

        for (int i = 0; i < options.Count; i++)
        {
            var maskInfo = options[i];

            var optionPanelObj = Instantiate(maskOptionsPrefab, maskOptionsParent);
            var optionPanel = optionPanelObj.GetComponent<MaskOptionPanel>();
            optionPanel.SetMaskInfo(i + 1, maskInfo, slotInputs[i].action);
        }
    }

    private void ClearOptions()
    {
        for (int i = maskOptionsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(maskOptionsParent.GetChild(i).gameObject);
        }
    }
}
