using System.Linq;
using UnityEngine;

public class MaskSelectionPanel : MonoBehaviour
{
    [SerializeField] Transform maskOptionsParent;
    [SerializeField] GameObject maskOptionsPrefab;
    [SerializeField] MaskList maskList;

    public void GenerateOptions()
    {
        ClearOptions();

        var options = Utils.Shuffle(maskList.masks).Take(3);

        foreach (var maskInfo in options)
        {
            var optionPanelObj = Instantiate(maskOptionsPrefab, maskOptionsParent);
            var optionPanel = optionPanelObj.GetComponent<MaskOptionPanel>();
            optionPanel.SetMaskInfo(maskInfo);
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
