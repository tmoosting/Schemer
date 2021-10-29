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
    public Dictionary<Character, int> previousCharacterPowerRankings = new Dictionary<Character, int>(); 
    public Dictionary<Character, int> newCharacterPowerRankings = new Dictionary<Character, int>();
    List<Character> previousCharacterPowerRankingList = new List<Character>();
    List<Character> newCharacterPowerRankingList = new List<Character>(); 
    public Dictionary<Institution, int> previousInstitutionPowerRankings = new Dictionary<Institution, int>(); 
    public Dictionary<Institution, int> newInstitutionPowerRankings = new Dictionary<Institution, int>();
    List<Institution> previousInstitutionPowerRankingList = new List<Institution>();
    List<Institution> newInstitutionPowerRankingList = new List<Institution>();

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
                // CHA
                previousCharacterPowerRankingList = new List<Character>();
                previousCharacterPowerRankingList = DataController.Instance.GetCharacters();
                previousCharacterPowerRankingList.Sort(SortByPower);
                previousCharacterPowerRankings = new Dictionary<Character, int>();
                int chaRank = 0;
                foreach (DataObject dataObject in previousCharacterPowerRankingList)
                {
                    chaRank++;
                    previousCharacterPowerRankings.Add((Character)dataObject, chaRank);
                }
         


                // INS
                previousInstitutionPowerRankingList = new List<Institution>();
                previousInstitutionPowerRankingList = DataController.Instance.institutionList;
                previousInstitutionPowerRankingList.Sort(SortByPower);
                previousInstitutionPowerRankings = new Dictionary<Institution, int>();
                int insRank = 0;
                foreach (DataObject dataObject in previousInstitutionPowerRankingList)
                {
                    insRank++;
                    previousInstitutionPowerRankings.Add((Institution)dataObject, insRank);
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
                // CHA
                newCharacterPowerRankingList = new List<Character>();
                newCharacterPowerRankingList = DataController.Instance.GetCharacters();
                newCharacterPowerRankingList.Sort(SortByPower);
                newCharacterPowerRankings = new Dictionary<Character, int>();
                int chaRank = 0;
                foreach (Character dataObject in newCharacterPowerRankingList)
                {
                    chaRank++;
                    newCharacterPowerRankings.Add(dataObject, chaRank);
                }
               
                // INS
                newInstitutionPowerRankingList = new List<Institution>();
                newInstitutionPowerRankingList = DataController.Instance.institutionList;
                newInstitutionPowerRankingList.Sort(SortByPower);
                newInstitutionPowerRankings = new Dictionary<Institution, int>();
                int insRank = 0;
                foreach (Institution dataObject in newInstitutionPowerRankingList)
                {
                    insRank++;
                    newInstitutionPowerRankings.Add(dataObject, insRank);
                }

                if (UIController.Instance.actionWindow.actionTaken == true)
                {
                    LogLastAction();
                    LogHierarchyChanges();
                }


                UIController.Instance.actionWindow.actionTaken = false;


            }


        }

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
            baseBonus += material.baseAmount * Constants.Instance.BASE_CHARACTER_POWER_POTENTIAL;        
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
     //   Debug.Log(scheme.name + " gets  " + calcPower);
    }
    void CalculateSchemePowerFromCooperations(Institution scheme)
    {
        float calcPower = 0f;
        foreach (Institution coopScheme in DataController.Instance.GetSchemesCoopedByScheme(scheme)) 
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
                    calcPower += (sch.totalPower * constants.INSTITUTION_PRIMARY_OWNER_POWERPROPORTION);
                    calcPower += sch.totalPower / sch.GetMemberCharacters().Count; 
                }
                else
                {
                    int secondaryOwnersAmount = sch.GetSchemeOwnerCharacters().Count - 1 + sch.genericOwnerCount;
                    calcPower += ((sch.totalPower * (1f- constants.INSTITUTION_PRIMARY_OWNER_POWERPROPORTION)) / secondaryOwnersAmount);
                    calcPower += sch.totalPower / sch.GetMemberCharacters().Count; 
                }
            }
            else if (sch.GetCooperativeCharacters().Contains(character))
            {
                int cooperatorsAmount = sch.GetCooperativeCharacters().Count  + sch.genericCooperativeCount;
                calcPower += (sch.totalPower * Constants.Instance.INSTITUTION_COOPERATOR_POWERPROPORTION)  / cooperatorsAmount; 
            }

            else if (sch.GetOwneeCharacters().Contains(character))
            {
                int owneesAmount = sch.GetOwneeCharacters().Count   + sch.genericOwneeCount;
                calcPower += (sch.totalPower * Constants.Instance.INSTITUTION_OWNEE_POWERPROPORTION) / owneesAmount;
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
    public enum ActionType { DESTROY, GIFT, GIFTALL, CLAIM, CREATECOOP, BREAKCOOP }
    [HideInInspector]
    public ActionType lastTakenActionType;
    [HideInInspector]
    public Material.MaterialSubtype lastGivenMaterialSubtype;
    [HideInInspector]
    public int lastGivenMaterialAmount;
    [HideInInspector]
    public string lastTakenActionPrimaryObjectName;
    [HideInInspector]
    public string lastTakenActionSecondaryObjectName;
    void LogLastAction()
    {
        ActionWindow window = UIController.Instance.actionWindow;
        string actionString = "Action: ";
        if (window.lastTakenActionType == ActionWindow.ActionType.DESTROY)
            actionString += window.lastTakenActionType.ToString() +" "+ window.lastTakenActionPrimaryObjectName;
        else if (window.lastTakenActionType == ActionWindow.ActionType.GIFT)
            actionString += window.lastTakenActionType.ToString() + " " + window.lastGivenMaterialAmount + " " + window.lastUsedMaterialSubtype + " to " +  window.lastTakenActionPrimaryObjectName;
        else if (window.lastTakenActionType == ActionWindow.ActionType.GIFTALL)
            actionString += window.lastTakenActionType.ToString() + " " + window.lastGivenMaterialAmount + " " + window.lastUsedMaterialSubtype + " to ALL Characters of " + window.lastTakenActionPrimaryObjectName ;
        else if (window.lastTakenActionType == ActionWindow.ActionType.CLAIM)
            actionString +=  window.lastTakenActionPrimaryObjectName + " CLAIM " + window.lastTakenActionSecondaryObjectName;
        else if (window.lastTakenActionType == ActionWindow.ActionType.CREATECOOP)
            actionString += window.lastTakenActionPrimaryObjectName + " CREATES COOP WITH " + window.lastTakenActionSecondaryObjectName;
        else if (window.lastTakenActionType == ActionWindow.ActionType.BREAKCOOP)
            actionString += window.lastTakenActionPrimaryObjectName + " BREAKS COOP WITH " + window.lastTakenActionSecondaryObjectName;

        Debug.Log(actionString);
    }



    void LogHierarchyChanges()
    {
        // Describe the three biggest CHA and INS gainers and losers

        float firstRanksGainCharacter = 0;
        float secondRanksGainCharacter = 0;
        float thirdRanksGainCharacter = 0;
        float firstRanksLossCharacter = 0;
        float secondRanksLossCharacter = 0;
        float thirdRanksLossCharacter = 0;   
        float firstRanksGainInstitution = 0;
        float secondRanksGainInstitution = 0;
        float thirdRanksGainInstitution = 0;
        float firstRanksLossInstitution = 0;
        float secondRanksLossInstitution = 0;
        float thirdRanksLossInstitution = 0;
        Character firstWinnerCharacter = null;
        Character secondWinnerCharacter = null;
        Character thirdWinnerCharacter = null;
        Character firstLoserCharacter = null;
        Character secondLoserCharacter = null;
        Character thirdLoserCharacter = null;
        Institution firstWinnerInstitution = null;
        Institution secondWinnerInstitution = null;
        Institution thirdWinnerInstitution = null;
        Institution firstLoserInstitution = null;
        Institution secondLoserInstitution = null;
        Institution thirdLoserInstitution = null;

        // CHA 1
        foreach (Character dataObject in newCharacterPowerRankings.Keys)
        {
                if (newCharacterPowerRankings[dataObject] < previousCharacterPowerRankings[dataObject])
                {
                    float ranksGained = previousCharacterPowerRankings[dataObject] - newCharacterPowerRankings[dataObject];
                    if (ranksGained > firstRanksGainCharacter)
                    {
                        firstRanksGainCharacter = ranksGained;
                        firstWinnerCharacter = dataObject;
                    }
                }
                else if (newCharacterPowerRankings[dataObject] > previousCharacterPowerRankings[dataObject])
                {
                    float ranksLost = newCharacterPowerRankings[dataObject] - previousCharacterPowerRankings[dataObject];
                    if (ranksLost > firstRanksLossCharacter)
                    {
                    firstRanksLossCharacter = ranksLost;
                        firstLoserCharacter =    dataObject;
                    }
                } 
        }

        // CHA 2
        foreach (Character dataObject in newCharacterPowerRankings.Keys)
        {
            if (dataObject != firstWinnerCharacter && dataObject != firstLoserCharacter)
            {
                if (newCharacterPowerRankings[dataObject] < previousCharacterPowerRankings[dataObject])
                {
                    float ranksGained = previousCharacterPowerRankings[dataObject] - newCharacterPowerRankings[dataObject];
                    if (ranksGained > secondRanksGainCharacter)
                    {
                        secondRanksGainCharacter = ranksGained;
                        secondWinnerCharacter = dataObject;
                    }
                }
                else if (newCharacterPowerRankings[dataObject] > previousCharacterPowerRankings[dataObject])
                {
                    float ranksLost = newCharacterPowerRankings[dataObject] - previousCharacterPowerRankings[dataObject];
                    if (ranksLost > secondRanksLossCharacter)
                    {
                        secondRanksLossCharacter = ranksLost;
                        secondLoserCharacter = dataObject;
                    }
                }
            } 
        }

        // CHA 3
        foreach (Character dataObject in newCharacterPowerRankings.Keys)
        {
            if (dataObject != firstWinnerCharacter && dataObject != secondWinnerCharacter && 
                dataObject != firstLoserCharacter && dataObject != secondLoserCharacter)
            {
                if (newCharacterPowerRankings[dataObject] < previousCharacterPowerRankings[dataObject])
                {
                    float ranksGained = previousCharacterPowerRankings[dataObject] - newCharacterPowerRankings[dataObject];
                    if (ranksGained > thirdRanksGainCharacter)
                    {
                        thirdRanksGainCharacter = ranksGained;
                        thirdWinnerCharacter = dataObject;
                    }
                }
                else if (newCharacterPowerRankings[dataObject] > previousCharacterPowerRankings[dataObject])
                {
                    float ranksLost = newCharacterPowerRankings[dataObject] - previousCharacterPowerRankings[dataObject];
                    if (ranksLost > thirdRanksLossCharacter)
                    {
                        thirdRanksLossCharacter = ranksLost;
                        thirdLoserCharacter = dataObject;
                    }
                }
            }
        }


        // INS 1
        foreach (Institution dataObject in newInstitutionPowerRankings.Keys)
        {
            if (newInstitutionPowerRankings[dataObject] < previousInstitutionPowerRankings[dataObject])
            {
                float ranksGained = previousInstitutionPowerRankings[dataObject] - newInstitutionPowerRankings[dataObject];
                if (ranksGained > firstRanksGainInstitution)
                {
                    firstRanksGainInstitution = ranksGained;
                    firstWinnerInstitution = dataObject;
                }
            }
            else if (newInstitutionPowerRankings[dataObject] > previousInstitutionPowerRankings[dataObject])
            {
                float ranksLost = newInstitutionPowerRankings[dataObject] - previousInstitutionPowerRankings[dataObject];
                if (ranksLost > firstRanksLossInstitution)
                {
                    ranksLost = firstRanksLossInstitution;
                    firstLoserInstitution = dataObject;
                }
            }
        }

        // INS 2
        foreach (Institution dataObject in newInstitutionPowerRankings.Keys)
        {
            if (dataObject != firstWinnerInstitution && dataObject != firstLoserInstitution)
            {
                if (newInstitutionPowerRankings[dataObject] < previousInstitutionPowerRankings[dataObject])
                {
                    float ranksGained = previousInstitutionPowerRankings[dataObject] - newInstitutionPowerRankings[dataObject];
                    if (ranksGained > secondRanksGainInstitution)
                    {
                        secondRanksGainInstitution = ranksGained;
                        secondWinnerInstitution = dataObject;
                    }
                }
                else if (newInstitutionPowerRankings[dataObject] > previousInstitutionPowerRankings[dataObject])
                {
                    float ranksLost = newInstitutionPowerRankings[dataObject] - previousInstitutionPowerRankings[dataObject];
                    if (ranksLost > secondRanksLossInstitution)
                    {
                        ranksLost = secondRanksLossInstitution;
                        secondLoserInstitution = dataObject;
                    }
                }
            } 
        }

        // INS 3
        foreach (Institution dataObject in newInstitutionPowerRankings.Keys)
        {
            if (dataObject != firstWinnerInstitution && dataObject != secondWinnerInstitution &&
                 dataObject != firstLoserInstitution && dataObject != secondLoserInstitution )
            {
                if (newInstitutionPowerRankings[dataObject] < previousInstitutionPowerRankings[dataObject])
                {
                    float ranksGained = previousInstitutionPowerRankings[dataObject] - newInstitutionPowerRankings[dataObject];
                    if (ranksGained > thirdRanksGainInstitution)
                    {
                        thirdRanksGainInstitution = ranksGained;
                        thirdWinnerInstitution = dataObject;
                    }
                }
                else if (newInstitutionPowerRankings[dataObject] > previousInstitutionPowerRankings[dataObject])
                {
                    float ranksLost = newInstitutionPowerRankings[dataObject] - previousInstitutionPowerRankings[dataObject];
                    if (ranksLost > thirdRanksLossInstitution)
                    {
                        ranksLost = thirdRanksLossInstitution;
                        thirdLoserInstitution = dataObject;
                    }
                }
            }
        }
        // log action too
        if (firstWinnerCharacter != null)
            Debug.Log("GAIN: First CHA is " + firstWinnerCharacter.name + ": "+ firstRanksGainCharacter + " ranks");
        if (secondWinnerCharacter != null)
            Debug.Log("GAIN: Second CHA is " + secondWinnerCharacter.name + ": " + secondRanksGainCharacter + " ranks");
        if (thirdWinnerCharacter != null)
            Debug.Log("GAIN: Third CHA is " + thirdWinnerCharacter.name + ": " + thirdRanksGainCharacter + " ranks");
        if (firstLoserCharacter != null && firstRanksLossCharacter > 1 )
            Debug.Log("LOSS: First CHA is " + firstLoserCharacter.name + ": " + firstRanksLossCharacter + " ranks");
        if (secondLoserCharacter != null && secondRanksLossCharacter > 1 )
            Debug.Log("LOSS: Second CHA is " + secondLoserCharacter.name + ": " + secondRanksLossCharacter + " ranks");
        if (thirdLoserCharacter != null && thirdRanksLossCharacter > 1)
            Debug.Log("LOSS: Third CHA is " + thirdLoserCharacter.name + ": " + thirdRanksLossCharacter + " ranks");

        if (firstWinnerInstitution != null)
            Debug.Log("GAIN: First INS is " + firstWinnerInstitution.name + ": " + firstRanksGainInstitution + " ranks" );
        if (secondWinnerInstitution != null)
            Debug.Log("GAIN: Second INS is " + secondWinnerInstitution.name + ": " + secondRanksGainInstitution + " ranks");
        if (thirdWinnerInstitution != null)
            Debug.Log("GAIN: Third INS is " + thirdWinnerInstitution.name + ": " + thirdRanksGainInstitution + " ranks"); 
        if (firstLoserInstitution != null && firstRanksLossInstitution > 1)
            Debug.Log("LOSS: First INS is " + firstLoserInstitution.name + ": " + firstRanksLossInstitution + " ranks");
        if (secondLoserInstitution != null && secondRanksLossInstitution > 1)
            Debug.Log("LOSS: Second INS is " + secondLoserInstitution.name + ": " + secondRanksLossInstitution + " ranks");
        if (thirdLoserInstitution != null && thirdRanksLossInstitution > 1)
            Debug.Log("LOSS: Third INS is " + thirdLoserInstitution.name + ": " + thirdRanksLossInstitution + " ranks");
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
