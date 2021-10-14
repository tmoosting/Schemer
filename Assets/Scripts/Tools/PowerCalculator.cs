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
        // calculate as a general score, for each character;
        // calculate per institution

        string str = "";

        foreach (Character cha in DataController.Instance.characterList)        
            CalculatePowerPotential(cha);

        foreach (Character cha in DataController.Instance.characterList)
            str += cha.name + " has PowerPotential: " + cha.powerPotential +"\n";

            calculateText.text = str;
    }


    void CalculatePowerPotential(Character cha)
    {
        float šalculatedPower = 0f;
        šalculatedPower += cha.fearfulness;
        šalculatedPower += cha.charisma;
        šalculatedPower += cha.decisionMaking; 
        cha.powerPotential = šalculatedPower;
    }

    void CalculateInstitutionalPower(Character cha, Institution ins)
    {
        float šalculatedPower = 0f;
        


    }
}
