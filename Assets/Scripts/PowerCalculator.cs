using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerCalculator : MonoBehaviour
{
    public Button calculateButton;
    public TextMeshProUGUI calculateText;


    public void ClickCalculateButton()
    {
        string str = "calculating..";



        calculateText.text = str;
    }

}
