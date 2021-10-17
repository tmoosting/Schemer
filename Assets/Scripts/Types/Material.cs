using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material : DataObject
{ 
    public enum MaterialType { Building, Item}
    public enum MaterialBase {Mud, Iron, MeteoricIron}
    public enum MaterialSubtype 
    {
        Dwelling, // BUILDING
        Weapon    // ITEM
    }
    public MaterialType materialType;
    public MaterialSubtype materialSubtype;
    public MaterialBase materialBase;

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
        DataController data = DataController.Instance;
        fieldValueDict = dict;
        dataType = DataType.Material;

        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Type"] != "")
            materialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), dict["Type"]);
        if (dict["Subtype"] != "")
            materialSubtype = (MaterialSubtype)System.Enum.Parse(typeof(MaterialSubtype), dict["Subtype"]);
        if (dict["Base"] != "")
            materialBase = (MaterialBase)System.Enum.Parse(typeof(MaterialBase), dict["Base"]);
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
