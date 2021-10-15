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


    // Calculated In Runtime
    public float powerPotential;

    public Material(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;

        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Type"] != "")
            materialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), dict["Type"]);
        if (dict["Subtype"] != "")
            materialSubtype = (MaterialSubtype)System.Enum.Parse(typeof(MaterialSubtype), dict["Subtype"]);
        if (dict["Base"] != "")
            materialBase = (MaterialBase)System.Enum.Parse(typeof(MaterialBase), dict["Base"]);
    }
}
