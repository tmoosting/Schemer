using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class DataController : MonoBehaviour
{
    public static DataController Instance;

    [Header("Assigns")]
    public DataLinker linker;
    public Button autoCreateButton; 

    [Header("Options")]
    public bool scanDatabaseAtStart;
    public bool scanUnityObjectsAtStart; 

    [HideInInspector]  public List<Character> characterList = new List<Character>();
    [HideInInspector]  public List<Material> materialList = new List<Material>();
    [HideInInspector]  public List<Institution> institutionList = new List<Institution>();
    [HideInInspector]  public List<Relation> relationList = new List<Relation>();



    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        AutoCreate();
        if (scanDatabaseAtStart == true)
            linker.DatabaseTestScan(false);
        if (scanUnityObjectsAtStart == true)
             ScanCreatedObjects();
        UIController.Instance.RunStartupToggles();
    }
    void AutoCreate()
    {
        // DATABASE 
        CreateObjectsFromDatabase();

        // AUTO GENERATE
        //  CreateInstitutionalRelations();  
        CreatePersonalRelationsFromInstitutions();
    }
    public void ButtonCreate()
    {
        autoCreateButton.interactable = false;
        CreateEnforcerEquipment();
    }

    void CreateObjectsFromDatabase()
    {
        // create a list of dicts with key: fieldname, value: fieldvalue

        foreach (string id in linker.GetIDValuesForTable("Character"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Character"))            
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Character", field, id));            
            characterList.Add(new Character(fieldNamesAndValues));
        } 
        foreach (string id in linker.GetIDValuesForTable("Material"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Material"))
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Material", field, id));
            materialList.Add(new Material(fieldNamesAndValues));
        } 
    
        foreach (string id in linker.GetIDValuesForTable("Relation"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Relation"))
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Relation", field, id));
            relationList.Add(new Relation(fieldNamesAndValues));
        }

        foreach (string id in linker.GetIDValuesForTable("Institution"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Institution"))
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Institution", field, id));
            institutionList.Add(new Institution(fieldNamesAndValues));
        }
    }

    void CreatePersonalRelationsFromInstitutions()
    {
        // for each character in its member list, create a relation to each other member
        foreach (Institution ins in institutionList)                  
            foreach (Character cha in ins.GetMemberCharacters())            
                foreach (Character otherCha in ins.GetMemberCharacters())                
                    if (cha != otherCha)                    
                        CreatePersonalRelation(cha, otherCha, ins);
    }
    //void CreateInstitutionalRelations()
    //{
    //    // create relations between characters and institution for enhanced membership defining 
    //    foreach (Institution ins in institutionList)
    //    {
    //        foreach  (Character cha in characterList)
    //        {

    //        }
    //    }
    //}

    void CreatePersonalRelation (Character activeCha, Character passiveCha, Institution sharedInstitution)
    {
        relationList.Add(new Relation(activeCha, passiveCha, sharedInstitution));
    }

    void CreateEnforcerEquipment()
    {
        List<Character> processedCharacters = new List<Character>();
        foreach (Institution ins in institutionList)
        {
            foreach (Character cha in ins.enforcerCharacters)
            {
                if (processedCharacters.Contains(cha) == false)
                {
                    processedCharacters.Add(cha);
                    bool weaponFound = false;
                    foreach (Relation rel in relationList)
                    {
                        if (rel.relationType == Relation.RelationType.Ownership)
                            if (rel.activeDataObject == cha)
                            {
                                Material mat = (Material)rel.passiveDataObject;
                                if (mat.materialSubtype == Material.MaterialSubtype.Weapon)                                
                                    weaponFound = true;                                
                            } 
                    } 
                    if (weaponFound == false)
                        CreateWeaponMaterialForCharacter(cha);
                }
            }
        }
    }

    // TODO: Make separate script for auto creation??
    void CreateWeaponMaterialForCharacter (Character character)
    {
        string matID = "AUTOENFORCERWEAPON" + character.ID;
        Material material = new Material(matID, Material.MaterialType.Item, Material.MaterialSubtype.Weapon);
        materialList.Add(material);
        Relation relation = new Relation(character, material);
        relationList.Add(relation);
    }




    // --------------- GET FUNCTIONS


    public Character GetCharacterWithName (string name)
    {
        Character character = null;

        foreach (Character cha in characterList)        
            if (cha.name == name)
                character = cha;

        return character;
    }














    void ScanCreatedObjects()
    {

        Debug.Log("------------- SCANNING OBJECTS -------------");
        foreach (Character cha in characterList)        
            Debug.Log("Character " + cha.name + " has ID " + cha.ID + " and age " + cha.age);
        foreach (Material mat in materialList)
            Debug.Log("Material " + mat.name + " has ID " + mat.ID + " and type " + mat.materialType + " and base: "+ mat.materialBase);
        foreach (Institution ins in institutionList)
            Debug.Log("Institution " + ins.name + " has ID " + ins.ID  );
        foreach (Relation rel in relationList)
            Debug.Log("Relation " + rel.name + " has ID " + rel.ID + " and type: "+ rel.relationType + " and active ID: "+ rel.activeDataObject.ID + ", type: "+ rel.activeDataObject.dataType + " and passive ID: "+ rel.passiveDataObject.ID);


        Debug.Log("------------- SCANNING OWNERSHIPS -------------");
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                Debug.Log(rel.activeDataObject.name + " Owns: "+ rel.passiveDataObject.name);

        Debug.Log("------------- SCANNING MEMBERSHIPS -------------");
        foreach (Institution ins in institutionList)
        {
            string charstr = "";
            foreach (Character cha in ins.GetMemberCharacters())            
                charstr += cha.name + "  ";
            string matstr = "";
            foreach (Material mat in ins.memberMaterials)
                matstr += mat.name + "  ";
            Debug.Log(ins.name + " has members: " + charstr + "  ///  and material: " + matstr + "  ///  Generics: "+ ins.genericExecutiveCount + ", "+ ins.genericEnforcerCount +", "+ ins.genericAttendantCount +" exec count: "+ ins.executiveCharacters.Count +  " enforcer count: "+ ins.enforcerCharacters.Count );
        } 

    }
}
