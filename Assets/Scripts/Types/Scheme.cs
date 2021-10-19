using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheme : DataObject
{

    DataController data;

    // From database
    //public List<Material> memberMaterials = new List<Material>(); 
    //public List<Character> executiveCharacters = new List<Character>();
    //public List<Character> enforcerCharacters = new List<Character>();
    //public List<Character> attendantCharacters = new List<Character>();
    public int genericOwnerCount;
    public int genericCooperativeCount;
    public int genericOwneeCount;

    // From database - meta
    public Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();

    public float namedOwnerPower = 0f;
    public float namedCooperativePower = 0f;
    public float namedOwneePower = 0f;
    public float genericOwnerPower = 0f;
    public float genericCooperativePower = 0f;
    public float genericOwneePower = 0f;
    public float materialPower = 0f;
    public float totalPower = 0f;



    public Scheme(Dictionary<string, string> dict)
    {
        data = DataController.Instance;
        fieldValueDict = dict;
        base.dataType = DataObject.DataType.Scheme;

        ID = dict["ID"];
        name = dict["Name"];

        if (dict["GenericOwnerCharacters"] != "")
            genericOwnerCount = int.Parse(dict["GenericOwnerCharacters"]);
        if (dict["GenericCooperativeCharacters"] != "")
            genericCooperativeCount = int.Parse(dict["GenericCooperativeCharacters"]);
        if (dict["GenericOwneeCharacters"] != "")
            genericOwneeCount = int.Parse(dict["GenericOwneeCharacters"]);
    }

    public void CreateSchemeRelations()
    {
        foreach (string memberID in fieldValueDict["OwnsMaterials"].Split(','))
            foreach (Material mat in data.materialList)
                if (mat.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Ownership, this, mat);
        foreach (string memberID in fieldValueDict["OwnsSchemes"].Split(','))
            foreach (Scheme ins in data.schemeList)
                if (ins.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Ownership, this, ins);
        foreach (string memberID in fieldValueDict["CoopsSchemes"].Split(','))
            foreach (Scheme ins in data.schemeList)
                if (ins.ID == memberID)
                    data.CreateRelation(Relation.RelationType.Cooperative, this, ins);
    }
    // ---------- GET FUNCTIONS

    public Character GetMostPowerfulMember()
    {
        Character returnChar = null;
        float highPower = 0f;
        foreach (Character cha in GetMemberCharacters())
            if (cha.totalPower > highPower)
            {
                returnChar = cha;
                highPower = cha.totalPower;
            }
        if (returnChar == null)
            Debug.LogWarning("returning a null character for most powerful!");
        return returnChar;
    }

    public float GetTotalPower()
    {
        return
            namedOwnerPower + namedCooperativePower + namedOwneePower +
            genericOwnerPower + genericCooperativePower + genericOwneePower +
            materialPower;
    }

    public List<Character> GetMemberCharacters()
    {
        return data.GetSchemeCharacters(this);
    }
    public List<Character> GetOwnerCharacters()
    {
        List<Character> charList = data.GetSchemeCharacters(this);
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in data.relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject.dataType == DataType.Character)
                    if (charList.Contains((Character)rel.primaryDataObject))
                        returnList.Add((Character)rel.primaryDataObject);
        return returnList;
    }
    public List<Character> GetCooperativeCharacters()
    {
        List<Character> charList = data.GetSchemeCharacters(this);
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in data.relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
            {
                if (rel.primaryDataObject.dataType == DataType.Character)
                {
                    if (charList.Contains((Character)rel.primaryDataObject)) // might only need check primary because coop is set in Character table so relations are created only with Character as primary
                        returnList.Add((Character)rel.primaryDataObject);
                }
                else if (rel.secondaryDataObject.dataType == DataType.Character)
                {
                    if (charList.Contains((Character)rel.secondaryDataObject))
                        returnList.Add((Character)rel.secondaryDataObject);
                }

            }
        return returnList;
    }
    public List<Character> GetOwneeCharacters()
    {
        List<Character> charList = data.GetSchemeCharacters(this);
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in data.relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject.dataType == DataType.Character)
                    if (charList.Contains((Character)rel.secondaryDataObject))
                        returnList.Add((Character)rel.secondaryDataObject);
        return returnList;
    }
    public List<Material> GetOwnedMaterials()
    {
        return data.GetSchemeMaterials(this);
    }
    public List<Material> GetCharacterOwnedMaterials()
    {
        return data.GetSchemeCharacterMaterials(this);
    }
}
