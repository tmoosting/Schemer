using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerCalculator : MonoBehaviour
{
    public Button calculateButton;
    public TextMeshProUGUI charactersText;
    public TextMeshProUGUI institutionsText;
    public TextMeshProUGUI highscoresText;
    DataController data;

    private void Start()
    {
        data = DataController.Instance;
    }

    public void ClickCalculateButton()
    {
        // calculate as a general score, for each character;
        // calculate per institution

        string str = "";

        foreach (Character cha in data.characterList)        
            CalculatePowerPotential(cha);

        foreach (Material mat in data.materialList)        
            CalculatePowerPotential(mat); 
        
        // CALCULATE
        foreach (Institution ins in data.institutionList)        
            CalculateInstitutionPower(ins);        

        foreach (Character cha in data.characterList)
            CalculatePowerFromInstitutions(cha);



        // DISPLAY
        string charactersTextString = "";
        foreach (Character cha in data.characterList)        
            charactersTextString += cha.name + " has PotP: " + cha.powerPotential + ", TotP: " + cha.totalPower + "\n";
        charactersText.text = charactersTextString;

        string institutionTextString = "";
        foreach (Institution ins in data.institutionList)
            institutionTextString += ins.name + " has TotP: " + ins.totalPower + "\n";
        institutionsText.text = institutionTextString;

        string highscoreTextString = "";
        highscoreTextString += "Most powerful Institution: " + FindMostPowerfulInstitution().name + "\n";
        highscoreTextString += "Most powerful Character: " + FindMostPowerfulCharacter().name + "\n";

        highscoresText.text = highscoreTextString;
     

    }


    void CalculatePowerPotential(Character cha)
    {
        float calculatedPower = 0f;
        calculatedPower += cha.fearfulness;
        calculatedPower += cha.charisma;
        calculatedPower += cha.decisionMaking; 
        cha.powerPotential = calculatedPower;
        cha.totalPower += cha.powerPotential;
    }
    void CalculatePowerPotential(Material mat)
    {
        float calculatedPower = 0f;
        calculatedPower += Constants.Instance.materialSubtypePotentialPower[mat.materialSubtype]; 
        mat.powerPotential = calculatedPower;
    }
    void CalculateInstitutionPower(Institution institution)
    {
        float calcPower = 0f;

        // sum all members' potentialpower
        foreach (Character cha in institution.memberCharacters)        
            calcPower += cha.powerPotential;

        // sum all material addedpower
        foreach (Material mat in institution.memberMaterials)
            calcPower += mat.powerPotential;

        institution.totalPower += calcPower;
    }


    void CalculatePowerFromInstitutions(Character character)
    {
        float calcPower = 0f;
        foreach (Institution ins in data.institutionList)
        {
            if (ins.leaderCharacter == character)            
                calcPower += ins.totalPower;            
            else if (ins.memberCharacters.Contains(character))            
                calcPower += ins.totalPower / ins.memberCharacters.Count;
        }
        character.totalPower += calcPower;
    }














    // HELPER FUNCTIONS

    Institution FindMostPowerfulInstitution()
    {
        float highestPower = 0;
        Institution highestIns = null;
        foreach (Institution ins in DataController.Instance.institutionList)
        {
            if (ins.totalPower > highestPower)
            {
                highestIns = ins;
                highestPower = ins.totalPower;
            } 
        }
        return highestIns;
    }

    Character FindMostPowerfulCharacter()
    {
        float highestPower = 0;
        Character highestCha = null;
        foreach (Character cha in DataController.Instance.characterList)
        {
            if (cha.totalPower > highestPower)
            {
                highestCha = cha;
                highestPower = cha.totalPower;
            }
        }
        return highestCha;
    }
}
