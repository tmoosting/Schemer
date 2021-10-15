using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DataObject
{ 
    public int age;
    public float fearfulness;
    public float charisma;
    public float decisionMaking;

    // Calculated In Runtime
    public float powerPotential = 0f;
    public float materialPower = 0f;
    public float totalPower = 0f;



    public Character(Dictionary<string, string> dict)
    {
        DataController data = DataController.Instance;

        ID = dict["ID"];
        name = dict["Name"];
        if (dict["Age"] != "")
            age = int.Parse(dict["Age"]); 
        if (dict["Fearfulness"] != "")
            fearfulness = float.Parse(dict["Fearfulness"]);
        if (dict["Charisma"] != "")
            charisma = float.Parse(dict["Charisma"]);
        if (dict["DecisionMaking"] != "")
            decisionMaking = float.Parse(dict["DecisionMaking"]);
    }

 


}
