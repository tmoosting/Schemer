using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material : DataObject
{
    DataController data;

    public enum MaterialType 
    { 
        Destructive,  // anything used for war, conquest, violence, physical aggression
        Constructive,  // anything that generates or helps generate resources
        Exchange      // any commercial commodity or medium of exchange
    } 
    public enum MaterialSubtype 
    {
        Land, // Constructive, order of magnitude
        City,
        Village,
        Estate,
        Building,
        BigTool,
        SmallTool,
        BigArms,    // Destructive, order of magnitude
        Fortification,
        SmallArms
    }
    public MaterialType materialType;
    public MaterialSubtype materialSubtype;
     
    public float bonusPower; // also used for exchange type value, eco and power= interchangeable
    public float bonusFear;
    public float bonusCharisma;
    public float bonusSkill; 

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();

    // Calculated In Runtime
    public float powerPotential;


    // CONSTRUCTOR --- DATABASE
    public Material(Dictionary<string, string> dict)
    {
        data = DataController.Instance;
        fieldValueDict = dict;
        dataType = DataType.Material;

        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Type"] != "")
            materialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), dict["Type"]);
        if (dict["Subtype"] != "")
            materialSubtype = (MaterialSubtype)System.Enum.Parse(typeof(MaterialSubtype), dict["Subtype"]);
        if (dict["BonusPower"] != "")
            bonusPower = int.Parse(dict["BonusPower"]);
        if (dict["BonusFear"] != "")
            bonusFear = int.Parse(dict["BonusFear"]);
        if (dict["BonusCharisma"] != "")
            bonusCharisma = int.Parse(dict["BonusCharisma"]);
        if (dict["BonusSkill"] != "")
            bonusSkill = int.Parse(dict["BonusSkill"]); 
    }


    // CONSTRUCTOR --- AUTOCREATE
    public Material(string id, MaterialType type, MaterialSubtype subType)
    {
        this.ID = id;
        dataType = DataType.Material; 
        materialType = type;
        materialSubtype = subType;
    }
}
