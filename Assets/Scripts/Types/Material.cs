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
        Settlement, // could be more specific: city, town, village
        Estate,
        Building,
        Tool,
        SmallArms,
        Nugget // Exchange Unit
    }
    public MaterialType materialType;
    public MaterialSubtype materialSubtype;
     
    public float baseAmount; // Nugget and others: amount; Settlement: #inhabitants
    public float bonusFear;
    public float bonusCharisma;
    public float bonusSkill; 
    public float inhabitants; 

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();

    public List<Material> materialCollection = new List<Material>();

    // Calculated In Runtime
 //   public float totalPower;  now set in DataObject parent


    // CONSTRUCTOR --- DATABASE and MANUAL
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
        if (dict["BaseAmount"] != "")
            baseAmount = int.Parse(dict["BaseAmount"]);
        if (dict["BonusFear"] != "")
            bonusFear = int.Parse(dict["BonusFear"]);
        if (dict["BonusCharisma"] != "")
            bonusCharisma = int.Parse(dict["BonusCharisma"]);
        if (dict["BonusSkill"] != "")
            bonusSkill = int.Parse(dict["BonusSkill"]);
    }

    public void LinkMaterialCollections()
    {
        if (fieldValueDict["MaterialCollection"] != "")
        {
            foreach (string memberID in fieldValueDict["MaterialCollection"].Split(','))
            {
                Material foundMat = DataController.Instance.GetMaterialWithID(memberID);
                if (foundMat != null)
                {
                    materialCollection.Add(foundMat); 
                    data.CreateRelation(Relation.RelationType.Ownership, this, foundMat);
                }
            }
        } 
    }



}
