using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class DataController : MonoBehaviour
{
    public static DataController Instance;

    [Header("Assigns")]
    public DataLinker linker;
    public DataChanger changer;
    public Button autoCreateButton;
    public PowerCalculator powerCalculator;

    [Header("Options")]
    public bool scanDatabaseAtStart;
    public bool scanUnityObjectsAtStart; 

      public List<Character> characterList = new List<Character>();
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
        powerCalculator.CalculatePowers();
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



    // --------------- GET NUMERIC
    public int GetRelationAmount(DataObject dataObject)
    {
        int returnCount = 0;
        foreach (Relation relation in relationList)        
            if (relation.primaryDataObject == dataObject || relation.secondaryDataObject == dataObject)
                returnCount++;
        return returnCount;
    }
    public int GetTotalPower(DataObject dataObject)
    {
        if (dataObject.dataType == DataObject.DataType.Character)
        {
            Character cha = (Character)dataObject;
            return (int)cha.totalPower;
        }
        else if (dataObject.dataType == DataObject.DataType.Material)
        {
            Material mat = (Material)dataObject;
            return (int)mat.totalPower;
        }
        else if (dataObject.dataType == DataObject.DataType.Scheme)
        {
            Scheme sch = (Scheme)dataObject;
            return (int)sch.totalPower;
        }
        else return 0; // for relation
    }

    public DataObject GetMostPowerfulDataObjectFromDataObjectList (List<DataObject> contenders)
    {
        float highestPower = 0f;
        DataObject returnObject = null;
        foreach (DataObject obj in contenders)        
            if (obj.totalPower > highestPower)
            {
                highestPower = obj.totalPower;
                returnObject = obj;
            }        
        return returnObject;
    }
    public DataObject GetMostPowerfulDataObjectFromCharacterList(List<Character> contenders)
    {
        float highestPower = 0f;
        DataObject returnObject = null;
        foreach (DataObject obj in contenders)
            if (obj.totalPower > highestPower)
            {
                highestPower = obj.totalPower;
                returnObject = obj;
            }
        return returnObject;
    }
    public DataObject GetMostPowerfulDataObjectFromSchemeList(List<Scheme> contenders)
    {
        float highestPower = 0f;
        DataObject returnObject = null;
        foreach (DataObject obj in contenders)
            if (obj.totalPower > highestPower)
            {
                highestPower = obj.totalPower;
                returnObject = obj;
            }
        return returnObject;
    }
    // --------------- GET OWNERSHIP

    public List<Material> GetMaterialsOwnedByCharacter(Character character)
    {
        List<Material> returnList = new List<Material>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == character)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                        returnList.Add((Material)rel.secondaryDataObject);
        return returnList;
    }
    public List<Scheme> GetSchemesOwnedByCharacter(Character character)
    {
        List<Scheme> returnList = new List<Scheme>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == character)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Scheme)
                        returnList.Add((Scheme)rel.secondaryDataObject);
        return returnList;
    }
    public List<Scheme> GetSchemesCoopedByCharacter(Character character)
    {
        List<Scheme> returnList = new List<Scheme>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
            {
                if (rel.primaryDataObject == character && rel.secondaryDataObject.dataType == DataObject.DataType.Scheme) 
                    returnList.Add((Scheme)rel.secondaryDataObject);
                if (rel.secondaryDataObject == character && rel.primaryDataObject.dataType == DataObject.DataType.Scheme)
                    returnList.Add((Scheme)rel.primaryDataObject); 
            }                 
        return returnList;
    }
    public List<Scheme> GetSchemesOwningCharacter(Character character)
    {
        List<Scheme> returnList = new List<Scheme>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == character)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Scheme)
                        returnList.Add((Scheme)rel.primaryDataObject);
        return returnList;
    }


    public List<Material> GetMaterialsOwnedByScheme(Scheme scheme)
    {
        List<Material> returnList = new List<Material>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                        returnList.Add((Material)rel.secondaryDataObject);
        return returnList;
    }
    public List<Scheme> GetSchemesOwnedByScheme(Scheme scheme)
    {
        List<Scheme> returnList = new List<Scheme>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Scheme)
                        returnList.Add((Scheme)rel.secondaryDataObject);
        return returnList;
    }













    public Relation GetRelationWithTheseTwoDataObjects(DataObject primaryObject, DataObject secondaryObject)
    {
        Relation returnRelation = null;
        foreach (Relation relation in relationList)        
            if (relation.primaryDataObject == primaryObject && relation.secondaryDataObject == secondaryObject ||
                relation.primaryDataObject == secondaryObject && relation.secondaryDataObject == primaryObject)            
                       returnRelation = relation;

        return returnRelation;
    }


    public List<Relation> GetRelationsThatIncludeObject(DataObject dataObject)
    {
        List<Relation> returnList = new List<Relation>();
        foreach (Relation rel in relationList)        
            if (rel.primaryDataObject == dataObject || rel.secondaryDataObject == dataObject)
                returnList.Add(rel);        
        return returnList;
    }

    public List<DataObject> GetRelatedObjectsToObject(DataObject baseObject)
    {
        List<DataObject> returnList = new List<DataObject>();
        List<Relation> objectRelations = new List<Relation>();
        // grab relations first
        foreach (Relation rel in relationList)
            if (rel.primaryDataObject == baseObject || rel.secondaryDataObject == baseObject)
                objectRelations.Add(rel);
        foreach (Relation rel in objectRelations)
        {
            returnList.Add(rel);
            if (rel.primaryDataObject == baseObject)
                if (returnList.Contains(rel.secondaryDataObject) == false)
                    returnList.Add(rel.secondaryDataObject);
            if (rel.secondaryDataObject == baseObject)
                if (returnList.Contains(rel.primaryDataObject) == false)
                    returnList.Add(rel.primaryDataObject);
        }
        // if the baseObject is a relation..
        if (baseObject.dataType == DataObject.DataType.Relation)
        {
            Relation baseRelation = (Relation)baseObject;
            if (returnList.Contains(baseRelation.secondaryDataObject) == false)
                returnList.Add(baseRelation.secondaryDataObject);
            if (returnList.Contains(baseRelation.primaryDataObject) == false)
                returnList.Add(baseRelation.primaryDataObject);
        }
        return returnList;
    }
    public Character GetCharacterWithName(string name)
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
    public List<Character> GetSchemeOwnerCharacters(Scheme scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)            
                if (rel.secondaryDataObject == scheme)                
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.primaryDataObject);
        return returnList;
    }
    public List<Character> GetSchemeCoopCharacters(Scheme scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
            {
                if (rel.secondaryDataObject == scheme)
                {
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.primaryDataObject);
                }
                else if (rel.primaryDataObject == scheme)
                {
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.secondaryDataObject);
                }
            }
            
        return returnList;
    }
    public List<Character> GetSchemeOwneeCharacters(Scheme scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.secondaryDataObject);
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








    public bool IsFirstObjectOwnerOfSecondObject (DataObject firstObject, DataObject secondObject)
    {
        bool returnBool = false;
        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == firstObject && rel.secondaryDataObject == secondObject)
                    returnBool = true; 
        return returnBool;
    }

    public bool IsFirstObjectCoopWithSecondObject(DataObject firstObject, DataObject secondObject)
    {
        bool returnBool = false;
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
                if ((rel.primaryDataObject == firstObject && rel.secondaryDataObject == secondObject) ||
                   (rel.primaryDataObject == secondObject && rel.secondaryDataObject == firstObject))
                    returnBool = true;
        return returnBool;
    }

    public bool IsFirstObjectOwneeOfSecondObject(DataObject firstObject, DataObject secondObject)
    {
        bool returnBool = false;
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == secondObject && rel.secondaryDataObject == firstObject)
                    returnBool = true;
        return returnBool;
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
