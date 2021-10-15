using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static Constants Instance;


    public Dictionary<Material.MaterialSubtype, float> materialSubtypePotentialPower = new Dictionary<Material.MaterialSubtype, float>();

    private void Awake()
    {
        Instance = this;
        materialSubtypePotentialPower.Add(Material.MaterialSubtype.Weapon, SUBTYPE_POWER_WEAPON);
        materialSubtypePotentialPower.Add(Material.MaterialSubtype.Dwelling, SUBTYPE_POWER_DWELLING);
    }


    public float SUBTYPE_POWER_WEAPON = 30f;
    public float SUBTYPE_POWER_DWELLING = 80f; 
    public float PRIMARY_EXECUTIVE_POWERSHARE = 0.5f;
    public float GENERIC_EXECUTIVE_AVERAGE_POWER_POTENTIAL = 120f;
    public float GENERIC_ENFORCER_AVERAGE_POWER_POTENTIAL = 100f;
    public float GENERIC_ATTENDANT_AVERAGE_POWER_POTENTIAL = 80f;


     
}
