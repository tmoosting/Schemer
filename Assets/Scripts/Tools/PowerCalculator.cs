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
        //       UpdatePowerTexts();
        UIController.Instance.ReloadObjectCards();
    }
  

    public void CalculatePowers()
    {
        ResetPowerValues();

        foreach (Character cha in data.characterList)
            CalculateCharacterPowerPotential(cha);

        foreach (Material mat in data.materialList)
            CalculateMaterialPowerPotential(mat);

        foreach (Character cha in data.characterList)
            CalculateCharacterOwnedMaterialPower(cha);

        foreach (Scheme ins in data.schemeList)
            CalculateSchemesPower(ins);

        foreach (Character cha in data.characterList)
            CalculatePowerFromSchemes(cha);
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
        calculatedPower += material.bonusPower;
        calculatedPower += material.bonusFear;
        calculatedPower += material.bonusCharisma;
        calculatedPower += material.bonusSkill;
        material.totalPower += calculatedPower;
    }
    void CalculateCharacterOwnedMaterialPower(Character character)
    {
        float matPower = 0f;
        foreach (Relation rel in data.relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == character && rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                {
                    Material mat = (Material)rel.secondaryDataObject;
                    matPower += mat.totalPower; 
                }

        character.materialPower = matPower;
        character.totalPower += character.materialPower;
    }
    void CalculateSchemesPower(Scheme scheme)
    {  
        foreach (Character cha in scheme.GetSchemeOwnerCharacters())
        {
            scheme.namedOwnerPower += cha.powerPotential;
            scheme.namedOwnerPower += cha.materialPower;
        }
        foreach (Character cha in scheme.GetCooperativeCharacters())
        {
            scheme.namedCooperativePower += cha.powerPotential;
            scheme.namedCooperativePower += cha.materialPower;
        }
        foreach (Character cha in scheme.GetOwneeCharacters())
        {
            scheme.namedOwneePower += cha.powerPotential;
            scheme.namedOwneePower += cha.materialPower;
        }

        scheme.genericOwnerPower += scheme.genericOwnerCount * constants.GENERIC_OWNER_AVERAGE_POWER_POTENTIAL;
        scheme.genericCooperativePower += scheme.genericCooperativeCount * constants.GENERIC_COOPERATIVE_AVERAGE_POWER_POTENTIAL;
        scheme.genericOwneePower += scheme.genericOwneeCount * constants.GENERIC_OWNEE_AVERAGE_POWER_POTENTIAL;

        float calcPower =
            scheme.namedOwnerPower + scheme.namedCooperativePower + scheme.namedOwneePower +
               scheme.genericOwnerPower + scheme.genericCooperativePower + scheme.genericOwneePower;

        
        foreach (Material mat in scheme.GetOwnedMaterials())
            calcPower += mat.totalPower;

        scheme.totalPower += calcPower;
    }


    void CalculatePowerFromSchemes(Character character)
    {
        float calcPower = 0f;
        foreach (Scheme sch in data.schemeList)
        {
            if (sch.GetSchemeOwnerCharacters().Contains(character))
            { 
                if (sch.GetSchemeOwnerCharacters()[0] == character)
                {
                    calcPower += (sch.totalPower * constants.PRIMARY_OWNER_POWERSHARE);
                    calcPower += sch.totalPower / sch.GetMemberCharacters().Count; 
                }
                else
                {
                    calcPower += (sch.totalPower * (1f- constants.PRIMARY_OWNER_POWERSHARE) / sch.GetSchemeOwnerCharacters().Count - 1);
                    calcPower += sch.totalPower / sch.GetMemberCharacters().Count; 
                }
            }
            else if (sch.GetMemberCharacters().Contains(character))
            {
                calcPower += sch.totalPower / sch.GetMemberCharacters().Count; 
            }
        }
       character.schemesPower = calcPower;
       character.totalPower += character.schemesPower;
    }



    void ResetPowerValues()
    {
        foreach (Character cha in data.characterList)
        {
            cha.powerPotential = 0f;
            cha.materialPower = 0f;
            cha.schemesPower = 0f;
            cha.totalPower = 0f;
        }
        foreach (Material mat in data.materialList)
        {
            mat.totalPower = 0f;
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











    //public void UpdatePowerTexts()
    //{
    //    string charactersTextString = "";
    //    foreach (Character cha in data.characterList)
    //        charactersTextString += cha.name + " has PotP: " + cha.powerPotential + ", MatP: " + cha.materialPower + ", TotP: " + cha.totalPower + "\n";
    //    charactersText.text = charactersTextString;

    //    string institutionTextString = "";
    //    foreach (Scheme ins in data.schemeList)
    //    {
    //        institutionTextString += ins.name + " has TotP: " + ins.totalPower + "\n";
    //        institutionTextString += ins.namedOwnerPower + " from " + ins.GetOwnerCharacters().Count + " named Executives;\n";
    //        institutionTextString += ins.namedCooperativePower + " from " + ins.GetCooperativeCharacters().Count + " named Enforcers;\n";
    //        institutionTextString += ins.namedOwneePower + " from " + ins.GetOwneeCharacters().Count + " named Attendants;\n";
    //        institutionTextString += ins.genericOwnerPower + " from " + ins.genericOwnerCount + " generic Executives;\n";
    //        institutionTextString += ins.genericCooperativePower + " from " + ins.genericCooperativeCount + " generic Enforcers;\n";
    //        institutionTextString += ins.genericOwneePower + " from " + ins.genericOwneeCount + " generic Attendants;\n\n";
    //    }
    //    institutionsText.text = institutionTextString;

    //    string highscoreTextString = "";
    //    highscoreTextString += "Most powerful Institution: " + FindMostPowerfulInstitution().name + " at " + FindMostPowerfulInstitution().totalPower + "\n";
    //    highscoreTextString += "Most powerful Character: " + FindMostPowerfulCharacter().name + " at " + FindMostPowerfulCharacter().totalPower + "\n";
    //    //foreach (Institution ins in data.institutionList)
    //    //    highscoreTextString += "Most powerful Character in " + ins.name +
    //    //        ": " + ins.GetMostPowerfulMember().name + " at " + 
    //    //        ins.GetMostPowerfulMember().totalPower + "\n";

    //    highscoresText.text = highscoreTextString;
    //}
}
