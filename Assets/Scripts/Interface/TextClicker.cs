using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextClicker : MonoBehaviour
{
    public List<Toggle> toggleList = new List<Toggle>();

    private void OnMouseUp()
    {
        // if all toggles selected: deselect all. otherwise select all

        bool allTogglesOn = true;
        foreach (Toggle toggle in toggleList)        
            if (toggle.isOn == false)
                allTogglesOn = false;

        if (allTogglesOn == true)
            foreach (Toggle toggle in toggleList)
                toggle.isOn = false;
        else
            foreach (Toggle toggle in toggleList)
                toggle.isOn = true;

    }
}
