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

        foreach (Scheme ins in data.schemeList)
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
        calculatedPower += constants.materialSubtypeBaseValues[material.materialSubtype];
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
                if (rel.primaryDataObject == character && rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                {
                    Material mat = (Material)rel.secondaryDataObject;
                    matPower += mat.powerPotential; 
                }

        character.materialPower = matPower;
        character.totalPower += character.materialPower;
    }
    void CalculateInstitutionPower(Scheme institution)
    {  
        foreach (Character cha in institution.GetOwnerCharacters())
        {
            institution.namedOwnerPower += cha.powerPotential;
            institution.namedOwnerPower += cha.materialPower;
        }
        foreach (Character cha in institution.GetCooperativeCharacters())
        {
            institution.namedCooperativePower += cha.powerPotential;
            institution.namedCooperativePower += cha.materialPower;
        }
        foreach (Character cha in institution.GetOwneeCharacters())
        {
            institution.namedOwneePower += cha.powerPotential;
            institution.namedOwneePower += cha.materialPower;
        }

        institution.genericOwnerPower += institution.genericOwnerCount * constants.GENERIC_OWNER_AVERAGE_POWER_POTENTIAL;
        institution.genericCooperativePower += institution.genericCooperativeCount * constants.GENERIC_COOPERATIVE_AVERAGE_POWER_POTENTIAL;
        institution.genericOwneePower += institution.genericOwneeCount * constants.GENERIC_OWNEE_AVERAGE_POWER_POTENTIAL;

        float calcPower =
            institution.namedOwnerPower + institution.namedCooperativePower + institution.namedOwneePower +
               institution.genericOwnerPower + institution.genericCooperativePower + institution.genericOwneePower;

        
        foreach (Material mat in institution.GetOwnedMaterials())
            calcPower += mat.powerPotential;

        institution.totalPower += calcPower;
    }


    void CalculatePowerFromInstitutions(Character character)
    {
        float calcPower = 0f;
        foreach (Scheme ins in data.schemeList)
        {
            if (ins.GetOwnerCharacters().Contains(character))
            { 
                if (ins.GetOwnerCharacters()[0] == character)
                {
                    calcPower += (ins.totalPower * constants.PRIMARY_OWNER_POWERSHARE);
                    calcPower += ins.totalPower / ins.GetMemberCharacters().Count; 
                }
                else
                {
                    calcPower += (ins.totalPower * (1f- constants.PRIMARY_OWNER_POWERSHARE) / ins.GetOwnerCharacters().Count - 1);
                    calcPower += ins.totalPower / ins.GetMemberCharacters().Count; 
                }
            }
            else if (ins.GetMemberCharacters().Contains(character))
            {
                calcPower += ins.totalPower / ins.GetMemberCharacters().Count; 
            }        
        }
       character.institutionalPower = calcPower;
       character.totalPower += character.institutionalPower;
    }



    void ResetPowerValues()
    {
        foreach (Character cha in data.characterList)
        {
            cha.powerPotential = 0f;
            cha.materialPower = 0f;
            cha.institutionalPower = 0f;
            cha.totalPower = 0f;
        }
        foreach (Material mat in data.materialList)
        {
            mat.powerPotential = 0f;
        }
        foreach (Scheme ins in data.schemeList)
        {
            ins.namedOwnerPower = 0f;
            ins.namedCooperativePower = 0f;
            ins.namedOwneePower = 0f;
            ins.genericOwnerPower = 0f;
            ins.genericCooperativePower = 0f;
            ins.genericOwneePower = 0f;
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
        foreach (Scheme ins in data.schemeList)
        {
            institutionTextString += ins.name + " has TotP: " + ins.totalPower + "\n";
            institutionTextString += ins.namedOwnerPower + " from " + ins.GetOwnerCharacters().Count + " named Executives;\n";
            institutionTextString += ins.namedCooperativePower + " from " + ins.GetCooperativeCharacters().Count + " named Enforcers;\n";
            institutionTextString += ins.namedOwneePower + " from " + ins.GetOwneeCharacters().Count + " named Attendants;\n";
            institutionTextString += ins.genericOwnerPower + " from " + ins.genericOwnerCount + " generic Executives;\n";
            institutionTextString += ins.genericCooperativePower + " from " + ins.genericCooperativeCount + " generic Enforcers;\n";
            institutionTextString += ins.genericOwneePower + " from " + ins.genericOwneeCount + " generic Attendants;\n\n";
        }
        institutionsText.text = institutionTextString;

        string highscoreTextString = "";
        highscoreTextString += "Most powerful Institution: " + FindMostPowerfulInstitution().name + " at " + FindMostPowerfulInstitution().totalPower + "\n";
        highscoreTextString += "Most powerful Character: " + FindMostPowerfulCharacter().name + " at " + FindMostPowerfulCharacter().totalPower + "\n";
        //foreach (Institution ins in data.institutionList)
        //    highscoreTextString += "Most powerful Character in " + ins.name +
        //        ": " + ins.GetMostPowerfulMember().name + " at " + 
        //        ins.GetMostPowerfulMember().totalPower + "\n";

        highscoresText.text = highscoreTextString;
    }


    // HELPER FUNCTIONS

    Scheme FindMostPowerfulInstitution()
    {
        float highestPower = 0;
        Scheme highestIns = null;
        foreach (Scheme ins in DataController.Instance.schemeList)
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
