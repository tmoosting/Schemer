using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DataObject
{
    DataController data;

  
    // From database
    public int age;
    public float fearfulness;
    public float charisma;
    public float decisionMaking;

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();

    // Calculated In Runtime
    public float powerPotential = 10f;
    public float materialPower = 0f;
    public float schemesPower = 0f; 



    public Character(Dictionary<string, string> dict)
    {
        data = DataController.Instance;
        fieldValueDict = dict;
        dataType = DataType.Character;


        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Age"] != "")
            age = int.Parse(dict["Age"]); 
        if (dict["Fearfulness"] != "")
            fearfulness = float.Parse(dict["Fearfulness"]);
        if (dict["Charisma"] != "")
            charisma = float.Parse(dict["Charisma"]);
        if (dict["DecisionMaking"] != "")
            decisionMaking = float.Parse(dict["DecisionMaking"]);

        
    }

 
    public void CreateCharacterRelations()
    {
        foreach (string memberID in fieldValueDict["OwnsMaterials"].Split(','))
            foreach (Material mat in data.materialList)
                if (mat.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Ownership, this, mat);
        foreach (string memberID in fieldValueDict["OwnsInstitutions"].Split(','))
            foreach (Institution ins in data.institutionList)
                if (ins.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Ownership, this, ins);
        foreach (string memberID in fieldValueDict["CoopsInstitutions"].Split(','))
            foreach (Institution ins in data.institutionList)
                if (ins.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Cooperative, this, ins);
        foreach (string memberID in fieldValueDict["OwnedByInstitutions"].Split(','))
            foreach (Institution ins in data.institutionList)
                if (ins.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Ownership, ins, this);
    }


}
