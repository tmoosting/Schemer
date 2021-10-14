using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Institution : DataObject
{

 
    public Institution(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;

        ID = dict["ID"];
        name = dict["Name"]; 
    }
}
