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
    Constants constants;
   
    private void Start()
    {
        data = DataController.Instance;
        constants = Constants.Instance;
    }


    public void ClickCalculateButton()
    {
        CalculatePowers();
        UpdatePowerTexts();
    }
  

    void CalculatePowers()
    {
        ResetPowerValues();
        foreach (Character cha in data.characterList)
            CalculateCharacterPowerPotential(cha);

        foreach (Material mat in data.materialList)
            CalculateMaterialPowerPotential(mat);

        foreach (Character cha in data.characterList)
            CalculateCharacterOwnedMaterialPower(cha);

        foreach (Institution ins in data.institutionList)
            CalculateInstitutionPower(ins);

        foreach (Character cha in data.characterList)
            CalculatePowerFromInstitutions(cha);
    }

   

    void CalculateCharacterPowerPotential(Character character)
    {  
        float calculatedPower = 0f;
        calculatedPower += character.fearfulness;
        calculatedPower += character.charisma;
        calculatedPower += character.decisionMaking; 
        character.powerPotential = calculatedPower;
        character.totalPower += character.powerPotential;
    }
    void CalculateMaterialPowerPotential(Material material)
    {
        float calculatedPower = 0f;
        calculatedPower += constants.materialSubtypePotentialPower[material.materialSubtype];
        calculatedPower += material.bonusFear;
        calculatedPower += material.bonusCharisma;
        calculatedPower += material.bonusSkill;
        material.powerPotential += calculatedPower;
    }
    void CalculateCharacterOwnedMaterialPower(Character character)
    {
        float matPower = 0f;
        foreach (Relation rel in data.relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.activeDataObject == character)
                {
                    Material mat = (Material)rel.passiveDataObject;
                    matPower += mat.powerPotential; 
                }

        character.materialPower = matPower;
        character.totalPower += character.materialPower;
    }
    void CalculateInstitutionPower(Institution institution)
    {  
        foreach (Character cha in institution.executiveCharacters)
        {
            institution.namedExecutivePower += cha.powerPotential;
            institution.namedExecutivePower += cha.materialPower;
        }
        foreach (Character cha in institution.enforcerCharacters)
        {
            institution.namedEnforcerPower += cha.powerPotential;
            institution.namedEnforcerPower += cha.materialPower;
        }
        foreach (Character cha in institution.attendantCharacters)
        {
            institution.namedAttendantPower += cha.powerPotential;
            institution.namedAttendantPower += cha.materialPower;
        }

        institution.genericExecutivePower += institution.genericExecutiveCount * constants.GENERIC_EXECUTIVE_AVERAGE_POWER_POTENTIAL;
        institution.genericEnforcerPower += institution.genericEnforcerCount * constants.GENERIC_ENFORCER_AVERAGE_POWER_POTENTIAL;
        institution.genericAttendantPower += institution.genericAttendantCount * constants.GENERIC_ATTENDANT_AVERAGE_POWER_POTENTIAL;

        float calcPower =
            institution.namedExecutivePower + institution.namedEnforcerPower + institution.namedAttendantPower +
               institution.genericExecutivePower + institution.genericEnforcerPower + institution.genericAttendantPower;

        
        foreach (Material mat in institution.memberMaterials)
            calcPower += mat.powerPotential;

        institution.totalPower += calcPower;
    }


    void CalculatePowerFromInstitutions(Character character)
    {
        float calcPower = 0f;
        foreach (Institution ins in data.institutionList)
        {
            if (ins.executiveCharacters.Contains(character))
            { 
                if (ins.executiveCharacters[0] == character)
                {
                    calcPower += (ins.totalPower * constants.PRIMARY_EXECUTIVE_POWERSHARE);
                    calcPower += ins.totalPower / ins.GetMemberCharacters().Count; 
                }
                else
                {
                    calcPower += (ins.totalPower * (1f- constants.PRIMARY_EXECUTIVE_POWERSHARE) / ins.executiveCharacters.Count - 1);
                    calcPower += ins.totalPower / ins.GetMemberCharacters().Count; 
                }
            }
            else if (ins.GetMemberCharacters().Contains(character))
            {
                calcPower += ins.totalPower / ins.GetMemberCharacters().Count; 
            }        
        }
       character.totalPower += calcPower;
    }



    void ResetPowerValues()
    {
        foreach (Character cha in data.characterList)
        {
            cha.powerPotential = 0f;
            cha.materialPower = 0f;
            cha.totalPower = 0f;
        }
        foreach (Material mat in data.materialList)
        {
            mat.powerPotential = 0f;
        }
        foreach (Institution ins in data.institutionList)
        {
            ins.namedExecutivePower = 0f;
            ins.namedEnforcerPower = 0f;
            ins.namedAttendantPower = 0f;
            ins.genericExecutivePower = 0f;
            ins.genericEnforcerPower = 0f;
            ins.genericAttendantPower = 0f;
            ins.materialPower = 0f;
            ins.totalPower = 0f;
        }
    }







    public void UpdatePowerTexts()
    {
        string charactersTextString = "";
        foreach (Character cha in data.characterList)
            charactersTextString += cha.name + " has PotP: " + cha.powerPotential + ", MatP: " + cha.materialPower + ", TotP: " + cha.totalPower + "\n";
        charactersText.text = charactersTextString;

        string institutionTextString = "";
        foreach (Institution ins in data.institutionList)
        {
            institutionTextString += ins.name + " has TotP: " + ins.totalPower + "\n";
            institutionTextString += ins.namedExecutivePower + " from " + ins.executiveCharacters.Count + " named Executives;\n";
            institutionTextString += ins.namedEnforcerPower + " from " + ins.enforcerCharacters.Count + " named Enforcers;\n";
            institutionTextString += ins.namedAttendantPower + " from " + ins.attendantCharacters.Count + " named Attendants;\n";
            institutionTextString += ins.genericExecutivePower + " from " + ins.genericExecutiveCount + " generic Executives;\n";
            institutionTextString += ins.genericEnforcerPower + " from " + ins.genericEnforcerCount + " generic Enforcers;\n";
            institutionTextString += ins.genericAttendantPower + " from " + ins.genericAttendantCount + " generic Attendants;\n\n";
        }
        institutionsText.text = institutionTextString;

        string highscoreTextString = "";
        highscoreTextString += "Most powerful Institution: " + FindMostPowerfulInstitution().name + " at " + FindMostPowerfulInstitution().totalPower + "\n";
        highscoreTextString += "Most powerful Character: " + FindMostPowerfulCharacter().name + " at " + FindMostPowerfulCharacter().totalPower + "\n";
        foreach (Institution ins in data.institutionList)
            highscoreTextString += "Most powerful Character in " + ins.name + ": " + ins.GetMostPowerfulMember().name + " at " + ins.GetMostPowerfulMember().totalPower + "\n";

        highscoresText.text = highscoreTextString;
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
