using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material : DataObject
{ 
    public enum MaterialType { Building, Item}
    public enum MaterialBase {Iron, MeteoricIron}
    public MaterialType materialType;
    public MaterialBase materialBase;

   
    public Material(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;

        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Type"] != "")
            materialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), dict["Type"]);
        if (dict["Base"] != "")
            materialBase = (MaterialBase)System.Enum.Parse(typeof(MaterialBase), dict["Base"]);
    }
}
