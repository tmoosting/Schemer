using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation : DataObject
{
     
    public enum RelationType { Personal, Ownership, Institutional}

    public RelationType relationType;

    public DataObject activeDataObject;
    public DataObject passiveDataObject;
    public Institution sharedInstitution;
   


    // CONSTRUCTOR --- FROM DATABASE
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
    // CONSTRUCTOR --- INTERPERSONAL
    public Relation(Character activeCha, Character passiveCha, Institution sharedInstitution)
    {
        DataController data = DataController.Instance;
        dataType = DataType.Relation;
        ID = "RELPERS" + activeCha.ID + passiveCha.ID;
        name = "RELPERS" + activeCha.name + passiveCha.name + "through" + sharedInstitution.name;
        relationType = RelationType.Personal;
        activeDataObject = activeCha;
        passiveDataObject = passiveCha;
        this.sharedInstitution = sharedInstitution;
    }

    // CONSTRUCTOR --- INSTITUTIONAL
    public Relation(Character character, Institution institution)
    {
        DataController data = DataController.Instance;
        dataType = DataType.Relation;
        ID = "RELINS" + character.ID + institution.ID;
        name = "RELINS" + character.name + institution.name;
        relationType = RelationType.Institutional;
        activeDataObject = character;
        passiveDataObject = institution;
    }

    // CONSTRUCTOR --- OWNERSHIP
    public Relation(Character character, Material material)
    {
        DataController data = DataController.Instance;
        dataType = DataType.Relation;
        ID = "RELOWN" + character.ID + material.ID;
        name = "RELOWN" + character.name + material.name;
        relationType = RelationType.Ownership;
        activeDataObject = character;
        passiveDataObject = material;
    }
}
