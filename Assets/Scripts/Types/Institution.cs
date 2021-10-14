using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Institution : DataObject
{
    public List<Character> memberCharacters = new List<Character>();
    public List<Material> memberMaterials = new List<Material>();
 
    public Institution(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;

        ID = dict["ID"];
        name = dict["Name"];

        foreach (Character cha in data.characterList)        
            foreach (string memberID in dict["Characters"].Split(','))            
                if (cha.ID == memberID)
                    memberCharacters.Add(cha);

        foreach (Material mat in data.materialList)
            foreach (string memberID in dict["Materials"].Split(','))
                if (mat.ID == memberID)
                    memberMaterials.Add(mat);

    }
}
