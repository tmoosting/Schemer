using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpIcon : MonoBehaviour
{
    private void OnMouseOver()
    {
        UIController.Instance.ShowHelpScreen(true);
    }
    private void OnMouseExit()
    {
        UIController.Instance.ShowHelpScreen(false); 
    }
}
