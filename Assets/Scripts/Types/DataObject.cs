using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataObject  
{
    public string ID;
    public string name;

    public enum DataType { Character, Material, Scheme, Relation }
    public DataType dataType;
}
