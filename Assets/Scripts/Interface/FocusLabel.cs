using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class FocusLabel : MonoBehaviour
{
    public SwitchManager focusSwitch;

    private void OnMouseUp()
    {
        focusSwitch.AnimateSwitch();
    }
}
