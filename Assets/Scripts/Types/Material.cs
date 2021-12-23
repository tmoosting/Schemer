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
        Arms,
        Nugget // Exchange Unit
    }
    public MaterialType materialType;
    public MaterialSubtype materialSubtype;
     
    public float baseAmount; // Nugget and others: amount; Settlement: #inhabitants
    public float bonusFear;
    public float bonusCharisma;
    public float bonusSkill; 
    

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();

    public List<Material> materialCollection = new List<Material>();
    public bool createdThroughAction = false;

    // Calculated In Runtime
 //   public float totalPower;  now set in DataObject parent


    // CONSTRUCTOR --- DATABASE and MANUAL
    public Material(Dictionary<string, string> dict)
    {
        data = DataController.Instance;
        fieldValueDict = dict;
        dataType = DataType.Material;
        createdThroughAction = false;

        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Type"] != "")
            materialType = (MaterialType)System.Enum.Parse(typeof(MaterialType), dict["Type"]);
        if (dict["Subtype"] != "")
            materialSubtype = (MaterialSubtype)System.Enum.Parse(typeof(MaterialSubtype), dict["Subtype"]);
        if (dict["BaseAmount"] != "")
            baseAmount = int.Parse(dict["BaseAmount"]);
        if (dict["BonusCoercion"] != "")
            bonusFear = int.Parse(dict["BonusCoercion"]);
        if (dict["BonusCharisma"] != "")
            bonusCharisma = int.Parse(dict["BonusCharisma"]);
        if (dict["BonusCapability"] != "")
            bonusSkill = int.Parse(dict["BonusCapability"]);
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
