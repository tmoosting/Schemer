using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Institution : DataObject
{ 
    public List<Material> memberMaterials = new List<Material>(); 
    public List<Character> executiveCharacters = new List<Character>();
    public List<Character> enforcerCharacters = new List<Character>();
    public List<Character> attendantCharacters = new List<Character>();
    public int genericExecutiveCount;
    public int genericEnforcerCount;
    public int genericAttendantCount;

    public float namedExecutivePower = 0f;
    public float namedEnforcerPower = 0f;
    public float namedAttendantPower = 0f;
    public float genericExecutivePower = 0f;
    public float genericEnforcerPower = 0f;
    public float genericAttendantPower = 0f;
    public float materialPower = 0f;
    public float totalPower = 0f;




   
    public Institution(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;

        ID = dict["ID"];
        name = dict["Name"];

       

        foreach (string memberID in dict["Executives"].Split(',')) 
            foreach (Character cha in data.characterList) 
                if (cha.ID == memberID)
                    executiveCharacters.Add(cha);  

        foreach (string memberID in dict["Enforcers"].Split(','))
            foreach (Character cha in data.characterList)
                if (cha.ID == memberID)
                    enforcerCharacters.Add(cha);

        foreach (string memberID in dict["Attendants"].Split(','))
            foreach (Character cha in data.characterList)
                if (cha.ID == memberID)
                    attendantCharacters.Add(cha);

        if (dict["GenericExecutives"] != "")
            genericExecutiveCount = int.Parse(dict["GenericExecutives"]);
        if (dict["GenericEnforcers"] != "")
            genericEnforcerCount = int.Parse(dict["GenericEnforcers"]);
        if (dict["GenericAttendants"] != "")
            genericAttendantCount = int.Parse(dict["GenericAttendants"]);

        foreach (Material mat in data.materialList)
            foreach (string memberID in dict["Materials"].Split(','))
                if (mat.ID == memberID)
                    memberMaterials.Add(mat);
         


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
            namedExecutivePower + namedEnforcerPower + namedAttendantPower +
            genericExecutivePower + genericEnforcerPower + genericAttendantPower +
            materialPower;
    }

    public List<Character> GetMemberCharacters()
    {
        List<Character> charList = new List<Character>();
        foreach (Character cha in executiveCharacters)
            charList.Add(cha);
        foreach (Character cha in enforcerCharacters)
            charList.Add(cha);
        foreach (Character cha in attendantCharacters)
            charList.Add(cha);
        return charList;
    }


}
