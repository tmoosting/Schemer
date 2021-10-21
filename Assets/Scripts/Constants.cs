using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static Constants Instance;

  

    public Dictionary<Material.MaterialSubtype, float> materialSubtypeBaseValues = new Dictionary<Material.MaterialSubtype, float>();

    private void Awake()
    {
        Instance = this;
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Land, SUBTYPE_VALUE_LAND); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.City, SUBTYPE_VALUE_CITY); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Town, SUBTYPE_VALUE_TOWN); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Village, SUBTYPE_VALUE_VILLAGE); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Estate, SUBTYPE_VALUE_ESTATE); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Building, SUBTYPE_VALUE_BUILDING); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Tool, SUBTYPE_VALUE_TOOL); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.WarMachine, SUBTYPE_VALUE_WARMACHINE); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Fortification, SUBTYPE_VALUE_FORTIFICATION); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.SmallArms, SUBTYPE_VALUE_SMALLARMS);  
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Nugget, SUBTYPE_VALUE_NUGGET);  
    } 

    // Base Values      
    public float SUBTYPE_VALUE_LAND = 1500f;
    public float SUBTYPE_VALUE_CITY = 800f;
    public float SUBTYPE_VALUE_TOWN = 450;
    public float SUBTYPE_VALUE_VILLAGE = 250f;
    public float SUBTYPE_VALUE_ESTATE = 150f;
    public float SUBTYPE_VALUE_BUILDING = 50f;
    public float SUBTYPE_VALUE_TOOL = 15f;
    public float SUBTYPE_VALUE_WARMACHINE = 200f;
    public float SUBTYPE_VALUE_FORTIFICATION = 150f;
    public float SUBTYPE_VALUE_SMALLARMS = 30f;
    public float SUBTYPE_VALUE_NUGGET = 1f;



    // Power Calculation
    public float PRIMARY_OWNER_POWERSHARE = 0.5f;
    public float GENERIC_OWNER_AVERAGE_POWER_POTENTIAL = 120f;
    public float GENERIC_COOPERATIVE_AVERAGE_POWER_POTENTIAL = 100f;
    public float GENERIC_OWNEE_AVERAGE_POWER_POTENTIAL = 80f;


     
}
