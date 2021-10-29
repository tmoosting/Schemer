using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataObject  
{
    public string ID;
    public string name;

    public enum DataType { Character, Material, Institution, Relation }
    public DataType dataType;

    public float totalPower;

    
}
