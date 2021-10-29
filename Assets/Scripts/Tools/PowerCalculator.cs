using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerCalculator : MonoBehaviour
{

    [Header("Options")]
    public bool trackPowerRankings;

    [Header ("Assigns")]
    public Button calculateButton;
    public TextMeshProUGUI charactersText;
    public TextMeshProUGUI institutionsText;
    public TextMeshProUGUI highscoresText;

    bool firstRun = true;

    DataController data;
    Constants constants;

    [HideInInspector] 
    public Dictionary<DataObject, int> previousPowerRankings = new Dictionary<DataObject, int>(); 
    public Dictionary<DataObject, int> newPowerRankings = new Dictionary<DataObject, int>();
    List<DataObject> previousPowerRankingList = new List<DataObject>();
    List<DataObject> newPowerRankingList = new List<DataObject>();

    private void Start()
    {
        data = DataController.Instance;
        constants = Constants.Instance;
    }


    public void ClickCalculateButton()
    {
        CalculatePowers(); 
    }
    int SortByPower(DataObject obj1, DataObject obj2)
    {
        return DataController.Instance.GetTotalPower(obj2).CompareTo(DataController.Instance.GetTotalPower(obj1));
    }

    public void CalculatePowers()
    {
        if (firstRun == true)
        {
            firstRun = false;
        }
        else
        {
            if (trackPowerRankings == true)
            {
                previousPowerRankingList = new List<DataObject>();
                previousPowerRankingList = DataController.Instance.GetAllNonRelationNonMaterialObjects();
                previousPowerRankingList.Sort(SortByPower);
            //    Debug.Log("oldrel has pos 1 " + previousPowerRankingList[0].ID);

                previousPowerRankings = new Dictionary<DataObject, int>();
                int rank = 0;
                foreach (DataObject dataObject in previousPowerRankingList)
                {
                    rank++;
                    previousPowerRankings.Add(dataObject, rank);
                }
            }
      
            
            ResetPowerValues();

            foreach (Character cha in data.characterList)
                CalculateCharacterPowerPotential(cha);

            foreach (Material mat in data.materialList)
                CalculateMaterialPowerPotential(mat);

            foreach (Character cha in data.characterList)
                CalculateCharacterOwnedMaterialPower(cha);

            foreach (Institution ins in data.institutionList)
                AddSchemePowerFromCharacters(ins);

            foreach (Institution ins in data.institutionList)
                AddSchemePowerFromMaterials(ins);

            foreach (Institution ins in data.institutionList)
                CalculateSchemePowerFromCooperations(ins);

            foreach (Institution ins in data.institutionList)
                AddSchemePowerFromCooperations(ins);

            foreach (Character cha in data.characterList)
                AddCharacterPowerFromSchemes(cha);

            UIController.Instance.ReloadObjectCards();

            if (trackPowerRankings == true)
            {
                newPowerRankingList = new List<DataObject>();
                newPowerRankingList = DataController.Instance.GetAllNonRelationNonMaterialObjects();
                newPowerRankingList.Sort(SortByPower);
           //     Debug.Log("newrel has pos 1 " + newPowerRankingList[0].ID);
                newPowerRankings = new Dictionary<DataObject, int>();
                int rank = 0;
                foreach (DataObject dataObject in newPowerRankingList)
                {
                    rank++;
                    newPowerRankings.Add(dataObject, rank);
                }

                foreach (DataObject dataObject in newPowerRankings.Keys)
                {
                    if (newPowerRankings[dataObject] > previousPowerRankings[dataObject])                    
                        Debug.Log(dataObject.ID + " UP " + (newPowerRankings[dataObject] - previousPowerRankings[dataObject]).ToString());
                    else if (newPowerRankings[dataObject] < previousPowerRankings[dataObject])
                        Debug.Log(dataObject.ID + " DOWN " + (previousPowerRankings[dataObject] - newPowerRankings[dataObject]).ToString());

                }
            }


        }

    }

   
    void LogPowerChanges()
    {

    }

    void CalculateCharacterPowerPotential(Character character)
    {  
        float calculatedPower = Constants.Instance.BASE_CHARACTER_POWER_POTENTIAL;
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

        float baseBonus = 0;
        if (material.materialSubtype == Material.MaterialSubtype.Settlement)        
            baseBonus += material.baseAmount *= Constants.Instance.BASE_CHARACTER_POWER_POTENTIAL;        
        else 
            baseBonus += material.baseAmount * Constants.Instance.materialSubtypeBaseValues[material.materialSubtype];
        calculatedPower += baseBonus;

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
    void AddSchemePowerFromCharacters(Institution scheme)
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

        scheme.genericOwnerPower += scheme.genericOwnerCount * constants.BASE_CHARACTER_POWER_POTENTIAL;
        scheme.genericCooperativePower += scheme.genericCooperativeCount * constants.BASE_CHARACTER_POWER_POTENTIAL;
        scheme.genericOwneePower += scheme.genericOwneeCount * constants.BASE_CHARACTER_POWER_POTENTIAL;

        float calcPower =
            scheme.namedOwnerPower + scheme.namedCooperativePower + scheme.namedOwneePower +
               scheme.genericOwnerPower + scheme.genericCooperativePower + scheme.genericOwneePower; 

        scheme.totalPower += calcPower;
    }
    void AddSchemePowerFromMaterials(Institution scheme)
    { 
        float calcPower = 0f;

        foreach (Material mat in scheme.GetOwnedMaterials())
            calcPower += mat.totalPower;
        scheme.materialPower = calcPower;
        scheme.totalPower += calcPower;
    }
    void CalculateSchemePowerFromCooperations(Institution scheme)
    {
        float calcPower = 0f;
        foreach (Institution coopScheme in DataController.Instance.GetSchemesCoopedByScheme(scheme)) 
            calcPower += (coopScheme.totalPower * Constants.Instance.SCHEME_COOPERATION_POWER_PERCENTAGE_BONUS); 
        scheme.cooperationPower = calcPower;        
    }
    void AddSchemePowerFromCooperations(Institution scheme)
    {        
        scheme.totalPower += scheme.cooperationPower;
    }
    void AddCharacterPowerFromSchemes(Character character)
    {
        float calcPower = 0f;
        foreach (Institution sch in data.institutionList)
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
        foreach (Institution ins in data.institutionList)
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











    //public void UpdatePowerTexts()
    //{
    //    string charactersTextString = "";
    //    foreach (Character cha in data.characterList)
    //        charactersTextString += cha.name + " has PotP: " + cha.powerPotential + ", MatP: " + cha.materialPower + ", TotP: " + cha.totalPower + "\n";
    //    charactersText.text = charactersTextString;

    //    string institutionTextString = "";
    //    foreach (Institution ins in data.schemeList)
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
