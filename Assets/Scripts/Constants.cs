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
        materialSubtypePotentialPower.Add(Material.MaterialSubtype.Weapon, 30f);
        materialSubtypePotentialPower.Add(Material.MaterialSubtype.Dwelling, 80f);
    }

     
}
