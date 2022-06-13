using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
[RequireComponent(typeof(BlackAndWitheButton))]
public class SwitchColorsButton : Button
{
    BlackAndWitheButton graphics;
    Graphic text, background;

    void Start()
    {

        base.Start();
    }
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        //get the graphics, if it could not get the graphics, return here
        if (!GetGraphics())
            return;
        var textColor =
            state == SelectionState.Disabled ? colors.normalColor :
            state == SelectionState.Highlighted ? colors.highlightedColor :
            state == SelectionState.Normal ? colors.disabledColor :
            state == SelectionState.Pressed ? colors.pressedColor :
            state == SelectionState.Selected ? colors.selectedColor : Color.white;
        var imageColor =
            state == SelectionState.Disabled ? colors.disabledColor :
            state == SelectionState.Highlighted ? colors.highlightedColor :
            state == SelectionState.Normal ? colors.normalColor :
            state == SelectionState.Pressed ? colors.pressedColor :
            state == SelectionState.Selected ? colors.selectedColor : Color.white;

        text.CrossFadeColor(textColor, instant ? 0 : colors.fadeDuration, true, true);
        background.CrossFadeColor(imageColor, instant ? 0 : colors.fadeDuration, true, true);
    }
    private bool GetGraphics()
    {
        if (!graphics) graphics = GetComponent<BlackAndWitheButton>();
        text = graphics?.text;
        background = graphics?.background;
        return graphics != null && text != null && background != null;
    }
}
