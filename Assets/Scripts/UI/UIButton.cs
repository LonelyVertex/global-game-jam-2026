using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : Button
{
    public TextMeshProUGUI text;
    public Color textNormal;
    public Color textHighlighted;
    public Color textPressed;
    public Color textSelected;
    public Color textDisabled;

    public Graphic background;
    public Color backgroundNormal;
    public Color backgroundHighlighted;
    public Color backgroundPressed;
    public Color backgroundSelected;
    public Color backgroundDisabled;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Normal:
                text.color = textNormal;
                background.color = backgroundNormal;
                break;
            case SelectionState.Highlighted:
                text.color = textHighlighted;
                background.color = backgroundHighlighted;
                break;
            case SelectionState.Pressed:
                text.color = textPressed;
                background.color = backgroundPressed;
                break;
            case SelectionState.Selected:
                text.color = textSelected;
                background.color = backgroundSelected;
                break;
            case SelectionState.Disabled:
                text.color = textDisabled;
                background.color = backgroundDisabled;
                break;
        }
    }
}
