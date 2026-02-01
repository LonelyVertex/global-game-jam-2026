using UnityEngine;
using UnityEngine.UI;

public class MaskGridItem : MonoBehaviour
{
    public Image mask;

    public void SetImage(Sprite icon)
    {
        mask.sprite = icon;
    }
}
