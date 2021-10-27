using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static Constants Instance;


    public Dictionary<Material.MaterialSubtype, Material.MaterialType> materialTyping = new Dictionary<Material.MaterialSubtype, Material.MaterialType>();

    public Dictionary<Material.MaterialSubtype, float> materialSubtypeBaseValues = new Dictionary<Material.MaterialSubtype, float>();

    

    private void Awake()
    {
        Instance = this;
        materialTyping.Add (Material.MaterialSubtype.Land, Material.MaterialType.Constructive);
        materialTyping.Add( Material.MaterialSubtype.Settlement, Material.MaterialType.Constructive); 
        materialTyping.Add( Material.MaterialSubtype.Estate, Material.MaterialType.Constructive);
        materialTyping.Add( Material.MaterialSubtype.Building, Material.MaterialType.Constructive);
        materialTyping.Add( Material.MaterialSubtype.Tool, Material.MaterialType.Constructive);
        materialTyping.Add( Material.MaterialSubtype.WarMachine, Material.MaterialType.Destructive);
        materialTyping.Add( Material.MaterialSubtype.Fortification, Material.MaterialType.Destructive);
        materialTyping.Add( Material.MaterialSubtype.SmallArms, Material.MaterialType.Destructive);
        materialTyping.Add( Material.MaterialSubtype.Nugget, Material.MaterialType.Exchange);


        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Land, SUBTYPE_VALUE_LAND); 
        materialSubtypeBaseValues.Add(Material.MaterialSubtype.Settlement, SUBTYPE_VALUE_SETTLEMENT);  
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
    public float SUBTYPE_VALUE_SETTLEMENT = 600f; 
    public float SUBTYPE_VALUE_ESTATE = 150f;
    public float SUBTYPE_VALUE_BUILDING = 50f;
    public float SUBTYPE_VALUE_TOOL = 15f;
    public float SUBTYPE_VALUE_WARMACHINE = 200f;
    public float SUBTYPE_VALUE_FORTIFICATION = 150f;
    public float SUBTYPE_VALUE_SMALLARMS = 30f;
    public float SUBTYPE_VALUE_NUGGET = 1f;



    // Power Calculation
    public float SCHEME_COOPERATION_POWER_PERCENTAGE_BONUS = 0.1f;
    public float PRIMARY_OWNER_POWERSHARE = 0.5f;
    public float GENERIC_OWNER_AVERAGE_POWER_POTENTIAL = 120f;
    public float GENERIC_COOPERATIVE_AVERAGE_POWER_POTENTIAL = 100f;
    public float GENERIC_OWNEE_AVERAGE_POWER_POTENTIAL = 80f;


     
}
