using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation : DataObject
{
     
    public enum RelationType {  Ownership, Cooperative} 
        

    public RelationType relationType;

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();
     
    public DataObject primaryDataObject;
    public DataObject secondaryDataObject;    

    // Permutations:
    // INS owns INS
    // INS coops INS
    // INS owns MAT
    // INS owns CHA
    // INS coops CHA
    // CHA owns INS 
    // CHA owns MAT


    // CONSTRUCTOR from DataController's CreateRelation()
    public Relation (RelationType type, DataObject primary, DataObject secondary, int relNumber)
    {
        DataController data = DataController.Instance;
        dataType = DataType.Relation;
        relationType = type;
        primaryDataObject = primary;
        secondaryDataObject = secondary;
        ID = "REL" + relNumber;
        name = "REL_" + primary.name + "_" + type.ToString() + "_" + secondary.name; 
    }





    //// CONSTRUCTOR --- INSTITUTIONAL
    //public Relation(Character character, Institution institution)
    //{
    //    DataController data = DataController.Instance;
    //    dataType = DataType.Relation;
    //    ID = "RELINS" + character.ID + institution.ID;
    //    name = "RELINS" + character.name + institution.name;
    //    relationType = RelationType.Cooperative;
    //    activeDataObject = character;
    //    passiveDataObject = institution;
    //}

    //// CONSTRUCTOR --- OWNERSHIP
    //public Relation(Character character, Material material)
    //{
    //    DataController data = DataController.Instance;
    //    dataType = DataType.Relation;
    //    ID = "RELOWN" + character.ID + material.ID;
    //    name = "RELOWN" + character.name + material.name;
    //    relationType = RelationType.Ownership;
    //    activeDataObject = character;
    //    passiveDataObject = material;
    //}




    // CONSTRUCTOR --- FROM DATABASE
    // DISABLED because table removed
    //public Relation(Dictionary<string, string> dict)
    //{
    //    DataController data = DataController.Instance;
    //    fieldValueDict = dict;
    //    dataType = DataType.Relation;
    //    ID = dict["ID"];
    //    name = dict["Name"]; 

    //    relationType = (RelationType)System.Enum.Parse(typeof(RelationType), dict["Type"]);

    //    if (dict["ActiveCharacter"] != "")
    //    {
    //        foreach (Character cha in data.characterList)            
    //            if (cha.ID == dict["ActiveCharacter"])
    //                activeDataObject = cha;
    //    }
    //    else if (dict["ActiveMaterial"] != "")
    //    {
    //        foreach (Material mat in data.materialList)
    //            if (mat.ID == dict["ActiveMaterial"])
    //                activeDataObject = mat;
    //    }
    //    else if (dict["ActiveInstitution"] != "")
    //    {
    //        foreach (Institution ins in data.institutionList)
    //            if (ins.ID == dict["ActiveInstitution"])
    //                activeDataObject = ins;
    //    }
    //    if (dict["PassiveCharacter"] != "")
    //    {
    //        foreach (Character cha in data.characterList)
    //            if (cha.ID == dict["PassiveCharacter"])
    //                passiveDataObject = cha;
    //    }
    //    else if (dict["PassiveMaterial"] != "")
    //    {
    //        foreach (Material mat in data.materialList)
    //            if (mat.ID == dict["PassiveMaterial"])
    //                passiveDataObject = mat;
    //    }
    //    else if (dict["PassiveInstitution"] != "")
    //    {
    //        foreach (Institution ins in data.institutionList)
    //            if (ins.ID == dict["PassiveInstitution"])
    //                passiveDataObject = ins;
    //    } 
    //}

    // CONSTRUCTOR --- INTERPERSONAL
    //public Relation(Character activeCha, Character passiveCha, Institution sharedInstitution)
    //{
    //    DataController data = DataController.Instance;
    //    dataType = DataType.Relation;
    //    ID = "RELPERS" + activeCha.ID + passiveCha.ID;
    //    name = "RELPERS" + activeCha.name + passiveCha.name + "through" + sharedInstitution.name;
    //    //   relationType = RelationType.Personal;
    //    activeDataObject = activeCha;
    //    passiveDataObject = passiveCha;
    //    this.sharedInstitution = sharedInstitution;
    //}
}
