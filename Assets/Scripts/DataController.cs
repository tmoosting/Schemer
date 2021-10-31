using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class DataController : MonoBehaviour
{
    public static DataController Instance;


    [Header("Options")]
    public bool addRandomCharacterTraits;
    public bool calculatePowerOnStart;
    public bool sweepUnownedMaterials;
    public bool sweepUnownedSchemes;
    public bool verifyRelationsAtStart;
    public bool scanDatabaseAtStart;
    public bool scanUnityObjectsAtStart;
    

    [Header("Assigns")]
    public DataLinker linker;
    public DataChanger changer;
    public Button autoCreateButton;
    public PowerCalculator powerCalculator;


      public List<Character> characterList = new List<Character>();
    [HideInInspector]  public List<Material> materialList = new List<Material>();
    [HideInInspector]  public List<Institution> institutionList = new List<Institution>();
    [HideInInspector]  public List<Relation> relationList = new List<Relation>();

    // template for manual material create
    Dictionary<string, string> materialDictionaryTemplate = new Dictionary<string, string>();

    // Private 
    int createdRelationsAmount = 0;
    int createdMaterialAmount = 0;


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
        if (verifyRelationsAtStart == true)
            VerifyDuplicateRelations();
        UIController.Instance.RunStartupToggles();

        powerCalculator.CalculatePowers(); // once here because first one invalid for not sure what reason anymore

        if (calculatePowerOnStart == true)
        {
            powerCalculator.CalculatePowers(); 
        }
    }
    void AutoCreate()
    { 
        CreateObjectsFromDatabase(); 
        CreateRelationsFromObjects();      
    }

    void CreateObjectsFromDatabase()
    { 

        // Add database values to dictionaries, then create new objects using dictionary

        // CHARACTER
        foreach (string id in linker.GetIDValuesForTable("Character"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Character"))            
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Character", field, id));            
            characterList.Add(new Character(fieldNamesAndValues));
        }

        if (addRandomCharacterTraits == true)
            foreach (Character character in characterList)
            {
                if (character.fearfulness == 0)
                    character.fearfulness = Random.Range(0, 99);
                if (character.charisma == 0)
                    character.charisma = Random.Range(0, 99);
                if (character.decisionMaking == 0)
                    character.decisionMaking = Random.Range(0, 99);
            }
        // MATERIAL
        // Manual-create template
        foreach (string field in linker.GetFieldNamesForTable("Material"))
            materialDictionaryTemplate.Add(field, "");

        // Create Material Objects
        foreach (string id in linker.GetIDValuesForTable("Material"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Material"))
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Material", field, id));
            materialList.Add(new Material(fieldNamesAndValues));
        }

        // Link Material-MaterialCollection after all are created
        foreach (Material mat in materialList)        
            mat.LinkMaterialCollections();

   

            // INSTITUTIONS

            foreach (string id in linker.GetIDValuesForTable("Institution"))
        {
            Dictionary<string, string> fieldNamesAndValues = new Dictionary<string, string>();
            foreach (string field in linker.GetFieldNamesForTable("Institution"))
                fieldNamesAndValues.Add(field, linker.GetEntryForTableAndFieldWithID("Institution", field, id));
            institutionList.Add(new Institution(fieldNamesAndValues));
        }





        // Create Nuggets from baseWealth for CHA and INS
        foreach (Character character in characterList)
            for (int i = 0; i < character.baseWealth; i++)
            {
                if (DoesObjectOwnMaterialSubtypeAlready(character, Material.MaterialSubtype.Nugget) == true)
                    GetAnyOwnedMaterialOfSubtype(character, Material.MaterialSubtype.Nugget).baseAmount++;
                else
                    CreateStandardMaterial(Material.MaterialSubtype.Nugget, character);
            }
        foreach (Institution institution in institutionList)
            for (int i = 0; i < institution.baseWealth; i++)
            {
                if (DoesObjectOwnMaterialSubtypeAlready(institution, Material.MaterialSubtype.Nugget) == true)
                    GetAnyOwnedMaterialOfSubtype(institution, Material.MaterialSubtype.Nugget).baseAmount++;
                else
                    CreateStandardMaterial(Material.MaterialSubtype.Nugget, institution);
            }

        // Create Equipped materials

        foreach (Institution institution in institutionList)
        {
            if (institution.equippedWith != "")
            {
                // give each named character the material.
                // give the institution the material for each generic character. preferably stacked
                Material.MaterialSubtype subType = (Material.MaterialSubtype)System.Enum.Parse(typeof(Material.MaterialSubtype), institution.equippedWith);
                foreach (Character character in GetSchemeCharacters(institution))
                    CreateStandardMaterial(subType, character);

                Material createdMaterial = CreateStandardMaterial(subType, institution);
                createdMaterial.baseAmount = (institution.genericOwnerCount + institution.genericCooperativeCount + institution.genericOwneeCount);


            }
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
        //foreach (Material mat in materialList)
        //    mat.CreateMaterialRelations();
        foreach (Institution ins in institutionList)
            ins.CreateSchemeRelations();
      
    }
    void VerifyDuplicateRelations()
    {
        // check ownership duplicates for mat
        List<Material> foundOwnedMaterials = new List<Material>();

        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                {
                    if (foundOwnedMaterials.Contains((Material)rel.secondaryDataObject))
                        Debug.LogWarning("Found a duplicate material: " + rel.secondaryDataObject.name);
                    else
                        foundOwnedMaterials.Add((Material)rel.secondaryDataObject);                    
                }
    }

    // Called from Cha,Mat,Ins constructors
    public void CreateRelation(Relation.RelationType type, DataObject primaryObject, DataObject secondaryObject)
    {
        createdRelationsAmount++;
        Relation relation = new Relation(type, primaryObject, secondaryObject, createdRelationsAmount);
        relationList.Add(relation); 
    }

    // standard means no attribute bonuses
    public Material CreateStandardMaterial(Material.MaterialSubtype subType, DataObject owner)
    {
        createdMaterialAmount++;
        Dictionary<string, string> creationDict = new Dictionary<string, string>();
        foreach (string keyString in materialDictionaryTemplate.Keys)
            creationDict.Add(keyString, "");

        string idString = "";
        if (createdMaterialAmount <= 9)
            idString = "MATC00" + createdMaterialAmount;
        else if (createdMaterialAmount <= 99)
            idString = "MATC0" + createdMaterialAmount;
        else
            idString = "MATC" + createdMaterialAmount;

        creationDict["ID"] = idString;    
        creationDict["Name"] = subType.ToString() + createdMaterialAmount.ToString();
        creationDict["Subtype"] = subType.ToString();
        creationDict["Type"] = Constants.Instance.materialTyping[subType].ToString(); 

        Material material = new Material(creationDict);
        material.totalPower = Constants.Instance.materialSubtypeBaseValues[subType];
        materialList.Add(material);
        CreateRelation(Relation.RelationType.Ownership, owner, material);
        return material;
    }


    // --------------- GET FUNCTIONS
 
    public List<Character> GetCharacters()
    {
        return characterList;
    }
    public Character GetCharacterWithID(string id)
    {
        Character character = null;
        foreach (Character cha in characterList)
            if (cha.ID == id)
                character = cha;
        return character;
    }
    public Material GetMaterialWithID(string id)
    {
        Material material = null;
        foreach (Material mat in materialList)
            if (mat.ID == id)
                material = mat; 
        return material;
    }

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
        else if (dataObject.dataType == DataObject.DataType.Institution)
        {
            Institution sch = (Institution)dataObject;
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
    public DataObject GetMostPowerfulDataObjectFromSchemeList(List<Institution> contenders)
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

    public DataObject GetOwnerOfMaterial(Material material)
    {
        DataObject returnObject = null;
        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == material)
                    returnObject = rel.primaryDataObject;
        return returnObject;        
    }
    public Character GetCharacterOwnerOfScheme(Institution scheme)
    {
        Character returnObject = null;
        List<Character> owners = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == scheme)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                       owners.Add((Character)rel.primaryDataObject);

        float highestPower = 0f;
        foreach (Character cha in owners)        
            if (cha.totalPower > highestPower)
            {
                highestPower = cha.totalPower;
                returnObject = cha;
            } 
        return returnObject;
    }
    public Institution GetSchemeOwnerOfScheme(Institution scheme)
    {
        Institution returnObject = null;
        List<Institution> owners = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == scheme)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Institution)
                        owners.Add((Institution)rel.primaryDataObject);

        float highestPower = 0f;
        foreach (Institution sch in owners)
            if (sch.totalPower > highestPower)
            {
                highestPower = sch.totalPower;
                returnObject = sch;
            }
        return returnObject;
    }
  
    public List<Character> GetCharactersOwningScheme(Institution scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == scheme)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.primaryDataObject);
        return returnList;
    }   
   
    public List<Character> GetCharactersCoopedByScheme(Institution scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
            {
                if (rel.secondaryDataObject == scheme)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.primaryDataObject);
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.secondaryDataObject);
            }
                
        return returnList;
    }
    public List<Character> GetCharactersOwnedByScheme(Institution scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.secondaryDataObject);
        return returnList;
    }
    public Character GetNextSchemeOwnerCharacter (Institution scheme)
    {
        Character returnCharacter = null;
        // find next most powerful member in coop, then ownees as owner
        if (GetCharactersOwningScheme(scheme).Count > 1)
        {
            float highestPower = 0f;
            foreach (Character character in GetCharactersOwningScheme(scheme))            
                if (character.totalPower > highestPower)                
                    if (GetCharacterOwnerOfScheme(scheme) != character)
                    {
                        highestPower = character.totalPower;
                        returnCharacter = character;
                    }  
        }
        else if (GetCharactersCoopedByScheme(scheme).Count != 0)
        {
            float highestPower = 0f;
            foreach (Character character in GetCharactersCoopedByScheme(scheme))
                if (character.totalPower > highestPower)
                {
                    highestPower = character.totalPower;
                    returnCharacter = character;
                }
        }
        else if (GetCharactersOwnedByScheme(scheme).Count != 0)
        {
            float highestPower = 0f;
            foreach (Character character in GetCharactersOwnedByScheme(scheme))
                if (character.totalPower > highestPower)
                {
                    highestPower = character.totalPower;
                    returnCharacter = character;
                }
        }
        else
            Debug.LogWarning("Could not find next owner for Institution: " + scheme.name);
        return returnCharacter;
    }
    public List<Material> GetMaterialsOwnedByDataObject(DataObject dataObject)
    {
        List<Material> returnList = new List<Material>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == dataObject)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                        returnList.Add((Material)rel.secondaryDataObject);
        return returnList;
    }
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
    public List<Material> GetMaterialsOwnedByScheme(Institution scheme)
    {
        List<Material> returnList = new List<Material>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)                    
                        returnList.Add((Material)rel.secondaryDataObject);
        return returnList;
    }
    public List<Institution> GetSchemesOwnedByCharacter(Character character)
    {
        List<Institution> returnList = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == character)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Institution)
                        returnList.Add((Institution)rel.secondaryDataObject);
        return returnList;
    }
    public List<Institution> GetSchemesCoopedByCharacter(Character character)
    {
        List<Institution> returnList = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
            {
                if (rel.primaryDataObject == character && rel.secondaryDataObject.dataType == DataObject.DataType.Institution) 
                    returnList.Add((Institution)rel.secondaryDataObject);
                if (rel.secondaryDataObject == character && rel.primaryDataObject.dataType == DataObject.DataType.Institution)
                    returnList.Add((Institution)rel.primaryDataObject); 
            }                 
        return returnList;
    }
    public List<Institution> GetSchemesOwningCharacter(Character character)
    {
        List<Institution> returnList = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == character)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Institution)
                        returnList.Add((Institution)rel.primaryDataObject);
        return returnList;
    }


  
    public List<Institution> GetSchemesOwnedByScheme(Institution scheme)
    {
        List<Institution> returnList = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Institution)
                        returnList.Add((Institution)rel.secondaryDataObject);
        return returnList;
    }
    public List<Institution> GetSchemesCoopedByScheme(Institution scheme)
    {
        List<Institution> returnList = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Cooperative)
            {
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Institution)
                        returnList.Add((Institution)rel.secondaryDataObject);
                if (rel.secondaryDataObject == scheme)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Institution)
                        returnList.Add((Institution)rel.primaryDataObject);
            } 
        return returnList;
    }
    public List<Institution> GetSchemesOwningScheme(Institution scheme)
    {
        List<Institution> returnList = new List<Institution>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.secondaryDataObject == scheme)
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Institution)
                        returnList.Add((Institution)rel.primaryDataObject);
        return returnList;
    }

    public Material GetAnyOwnedMaterialOfSubtype(DataObject dataObject, Material.MaterialSubtype subType)
    {
        Material material = null;
        foreach (Material  mat in GetMaterialsOwnedByDataObject(dataObject))        
            if (mat.materialSubtype == subType)
                material = mat;
        return material; 
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
   
  
    public List<Character> GetSchemeCharacters(Institution scheme)
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
    public List<Character> GetSchemeOwnerCharacters(Institution scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)            
                if (rel.secondaryDataObject == scheme)                
                    if (rel.primaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.primaryDataObject);
        return returnList;
    }
    public List<Character> GetSchemeCoopCharacters(Institution scheme)
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
    public List<Character> GetSchemeOwneeCharacters(Institution scheme)
    {
        List<Character> returnList = new List<Character>();
        foreach (Relation rel in relationList)
            if (rel.relationType == Relation.RelationType.Ownership)
                if (rel.primaryDataObject == scheme)
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Character)
                        returnList.Add((Character)rel.secondaryDataObject);
        return returnList;
    }
    public List<Material> GetSchemeMaterials(Institution scheme)
    {
        List<Material> returnList = new List<Material>();
        foreach (Relation rel in relationList)        
            if (rel.relationType == Relation.RelationType.Ownership)            
                if (rel.primaryDataObject == scheme)                
                    if (rel.secondaryDataObject.dataType == DataObject.DataType.Material)
                        returnList.Add((Material)rel.secondaryDataObject); 
        return returnList;
    }  
    
    public List<Material> GetSchemeCharacterMaterials(Institution scheme)
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




    public bool DoesRelationExistBetweenObjects(DataObject primaryObject, DataObject secondaryObject)
    {
        bool returnBool = false;
        foreach (Relation rel in relationList)
            if ((rel.primaryDataObject == primaryObject && rel.secondaryDataObject == secondaryObject) ||
               (rel.primaryDataObject == secondaryObject && rel.secondaryDataObject == primaryObject))
                returnBool = true;
        return returnBool;
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

    public bool DoesObjectOwnMaterialSubtypeAlready (DataObject dataObject, Material.MaterialSubtype subType)
    {
        bool returnBool = false;
        if (GetAnyOwnedMaterialOfSubtype(dataObject, subType) != null)
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
        foreach (Institution ins in institutionList)
            Debug.Log("Institution " + ins.name + " has ID " + ins.ID  );
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
        foreach (Institution ins in institutionList)
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
