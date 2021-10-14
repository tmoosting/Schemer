using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation : DataObject
{
    public string ID;
    public string name;

    public enum RelationType { Personal, Ownership}

    public RelationType relationType;

    public DataObject activeDataObject;
    public DataObject passiveDataObject; 
   
    public Relation(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;
        dataType = DataType.Relation;
        ID = dict["ID"];
        name = dict["Name"]; 

        relationType = (RelationType)System.Enum.Parse(typeof(RelationType), dict["Type"]);

        if (dict["ActiveCharacter"] != "")
        {
            foreach (Character cha in data.characterList)            
                if (cha.ID == dict["ActiveCharacter"])
                    activeDataObject = cha;
        }
        else if (dict["ActiveMaterial"] != "")
        {
            foreach (Material mat in data.materialList)
                if (mat.ID == dict["ActiveMaterial"])
                    activeDataObject = mat;
        }
        else if (dict["ActiveInstitution"] != "")
        {
            foreach (Institution ins in data.institutionList)
                if (ins.ID == dict["ActiveInstitution"])
                    activeDataObject = ins;
        }
        if (dict["PassiveCharacter"] != "")
        {
            foreach (Character cha in data.characterList)
                if (cha.ID == dict["PassiveCharacter"])
                    passiveDataObject = cha;
        }
        else if (dict["PassiveMaterial"] != "")
        {
            foreach (Material mat in data.materialList)
                if (mat.ID == dict["PassiveMaterial"])
                    passiveDataObject = mat;
        }
        else if (dict["PassiveInstitution"] != "")
        {
            foreach (Institution ins in data.institutionList)
                if (ins.ID == dict["PassiveInstitution"])
                    passiveDataObject = ins;
        }
      
    }
}
