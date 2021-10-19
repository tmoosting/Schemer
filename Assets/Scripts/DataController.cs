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
    [HideInInspector]  public List<Scheme> schemeList = new List<Scheme>();
    [HideInInspector]  public List<Relation> relationList = new List<Relation>();


    // Private 
    int createdRelationsAmount = 0;


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
        CreateObjectsFromDatabase(); 
        CreateRelationsFromObjects(); 
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
     
        foreach (string id in linker.GetIDValuesForTable("Scheme"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Scheme"))
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Scheme", field, id));
            schemeList.Add(new Scheme(fieldNamesAndValues));
        }

        // DISABLED because no separate relation table for now
        //foreach (string id in linker.GetIDValuesForTable("Relation"))
        //{
        //    Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
        //    foreach (string field in linker.GetFieldNamesForTable("Relation"))
        //        fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Relation", field, id));
        //    relationList.Add(new Relation(fieldNamesAndValues));
        //}
    }

    void CreateRelationsFromObjects()
    {
        foreach (Character cha in characterList)
            cha.CreateCharacterRelations();
        foreach (Scheme ins in schemeList)
            ins.CreateSchemeRelations(); 
    }

    // Called from Cha,Mat,Ins constructors
    public void CreateRelation(Relation.RelationType type, DataObject primaryObject, DataObject secondaryObject)
    {
        createdRelationsAmount++;
        Relation relation = new Relation(type, primaryObject, secondaryObject, createdRelationsAmount);
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


    public List<Character> GetSchemeCharacters(Scheme scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
        {
            if (rel.primaryDataObject == scheme)
            {
                if (rel.secondaryDataObject.dataType == DataObject.DataType.Character)
                    returnList.Add((Character)rel.secondaryDataObject);
            }
            else if (rel.secondaryDataObject == scheme)
            {
                if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                    returnList.Add((Character)rel.primaryDataObject);
            }
        } 
        return returnList;
    }
    
    public List<Material> GetSchemeMaterials(Scheme scheme)
    {
        List<Material> returnList = new List<Material>();
        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)            
                if (rel.primaryDataObject == scheme)                
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                        returnList.Add((Material)rel.secondaryDataObject);    
        return returnList;
    }  
    
    public List<Material> GetSchemeCharacterMaterials(Scheme scheme)
    {
        List<Material> returnList = new List<Material>();
        List<Character> charList = GetSchemeCharacters(scheme); 
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                    if (charList.Contains((Character)rel.primaryDataObject))
                        if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                            returnList.Add((Material)rel.secondaryDataObject);
        return returnList;
    }











    void ScanCreatedObjects()
    {

        Debug.Log("------------- SCANNING OBJECTS -------------");
        foreach (Character cha in characterList)        
            Debug.Log("Character " + cha.name + " has ID " + cha.ID + " and age " + cha.age);
        foreach (Material mat in materialList)
            Debug.Log("Material " + mat.name + " has ID " + mat.ID + " and type " + mat.materialType);
        foreach (Scheme ins in schemeList)
            Debug.Log("Scheme " + ins.name + " has ID " + ins.ID  );
     // DISABLED because no separate relation table for now
        //  foreach (Relation rel in relationList)
       //     Debug.Log("Relation " + rel.name + " has ID " + rel.ID + " and type: "+ rel.relationType + " and active ID: "+ rel.activeDataObject.ID + ", type: "+ rel.activeDataObject.dataType + " and passive ID: "+ rel.passiveDataObject.ID);


        Debug.Log("------------- SCANNING RELATIONS -------------");
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                Debug.Log(rel.primaryDataObject.name + " Owns: "+ rel.secondaryDataObject.name);
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
                Debug.Log(rel.primaryDataObject.name + " Coops with: " + rel.secondaryDataObject.name);

        Debug.Log("------------- SCANNING MEMBERSHIPS -------------");
        foreach (Scheme ins in schemeList)
        {
            string charstr = "";
            foreach (Character cha in ins.GetMemberCharacters())            
                charstr += cha.name + "  ";
            string matstr = "";
            foreach (Material mat in ins.GetOwnedMaterials())
                matstr += mat.name + "  ";
            string charmatstr = "";
            foreach (Material mat in ins.GetCharacterOwnedMaterials())
                charmatstr += mat.name + "  ";
            Debug.Log(ins.name + " has members: " + charstr + "  ///  and directly-owned-materials: " + matstr + " and indirectly-owned-materials: " + charmatstr + " ///  Generics: "+ ins.genericOwnerCount + ", "+ ins.genericCooperativeCount +", "+ ins.genericOwneeCount );
        } 

    }





    // DISABLED because no personal relations (for now)
    //void CreatePersonalRelationsFromInstitutions()
    //{
    //    // for each character in its member list, create a relation to each other member
    //    foreach (Institution ins in institutionList)                  
    //        foreach (Character cha in ins.GetMemberCharacters())            
    //            foreach (Character otherCha in ins.GetMemberCharacters())                
    //                if (cha != otherCha)                    
    //                    CreatePersonalRelation(cha, otherCha, ins);
    //}
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


    // DISABLED because no autogenerate for now
    //void CreateEnforcerEquipment()
    //{
    //    List<Character> processedCharacters = new List<Character>();
    //    foreach (Institution ins in institutionList)
    //    {
    //        foreach (Character cha in ins.enforcerCharacters)
    //        {
    //            if (processedCharacters.Contains(cha) == false)
    //            {
    //                processedCharacters.Add(cha);
    //                bool weaponFound = false;
    //                foreach (Relation rel in relationList)
    //                {
    //                    if (rel.relationType == Relation.RelationType.Ownership)
    //                        if (rel.primaryDataObject == cha)
    //                        {
    //                            Material mat = (Material)rel.secondaryDataObjecfft;
    //                            if (mat.materialType == Material.MaterialType.Destructive)                                
    //                                weaponFound = true;                                
    //                        } 
    //                } 
    //                if (weaponFound == false)
    //                    CreateWeaponMaterialForCharacter(cha);
    //            }
    //        }
    //    }
    //}
    //void CreateWeaponMaterialForCharacter(Character character)
    //{
    //    string matID = "AUTOENFORCERWEAPON" + character.ID;
    //    Material material = new Material(matID, Material.MaterialType.Destructive, Material.MaterialSubtype.SmallArms);
    //    materialList.Add(material);
    //    Relation relation = new Relation(character, material);
    //    relationList.Add(relation);
    //}
    //public void ButtonCreate()
    //{
    //    autoCreateButton.interactable = false;
    //    CreateEnforcerEquipment();
    //}

}
