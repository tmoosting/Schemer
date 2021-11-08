using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DataObject
{
    DataController data;

  
    // From database
    public int age;
    public float coercion;
    public float charisma;
    public float capability;
    public int baseWealth;

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();

    // Calculated In Runtime
    public float powerPotential = 10f;
    public float materialPower = 0f;
    public float institutionsPower = 0f; 
    public float indirectInstitutionsPower = 0f;
    public List<Institution> powerAddedAlreadyInstitutions = new List<Institution>();


    public Character(Dictionary<string, string> dict)
    {
        data = DataController.Instance;
        fieldValueDict = dict;
        dataType = DataType.Character;


        ID = dict["ID"];
        name = dict["Name"];
        //if (dict["Age"] != "")
        //    age = int.Parse(dict["Age"]); 
        if (dict["Coercion"] != "")
            coercion = float.Parse(dict["Coercion"]);
        if (dict["Charisma"] != "")
            charisma = float.Parse(dict["Charisma"]);
        if (dict["Capability"] != "")
            capability = float.Parse(dict["Capability"]);
        if (dict["Wealth"] != "")
            baseWealth = int.Parse(dict["Wealth"]);

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
