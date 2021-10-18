using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectViewCard : MonoBehaviour
{
    UIController ui;
    public DataObject containedObject;
    //  public DataObject previousViewObject; use to navigate back when clicking a link

    public TextMeshProUGUI titleText;
    public Transform listParent;

    public GameObject textPrefab;
    public GameObject linkPrefab;
    List<GameObject> textPrefabList = new List<GameObject>();
    List<GameObject> linkPrefabList = new List<GameObject>();

    private void Awake()
    {
        ui = UIController.Instance;
    }
    // Delegator
    public void LoadDataObject(DataObject dataObject)
    {
        if (dataObject.dataType == DataObject.DataType.Character)
            LoadCharacterObject((Character)dataObject);
        if (dataObject.dataType == DataObject.DataType.Material)
            LoadMaterialObject((Material)dataObject);
        if (dataObject.dataType == DataObject.DataType.Institution)
            LoadInstitutionObject((Institution)dataObject);
        if (dataObject.dataType == DataObject.DataType.Relation)
            LoadRelationObject((Relation)dataObject);
    }

   
    void LoadCharacterObject(Character character)
    {
        titleText.text = character.name;

        if (ui.toggleShowDatabase.isOn)
        {
            foreach (string str in character.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + character.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (ui.toggleShowPower.isOn)
        {
            CreateTextObject("Pow.Potential: "+ character.powerPotential);
            CreateTextObject("Pow.Material: "+ character.materialPower);
            CreateTextObject("Pow.Institual: "+ character.institutionalPower);
            CreateTextObject("Pow.Total: "+ character.totalPower);
            CreateTextObject("----------------------------");
        }


    }
    void LoadMaterialObject(Material material)
    {
        titleText.text = material.name;
        if (ui.toggleShowDatabase.isOn)
        {
            foreach (string str in material.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + material.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (ui.toggleShowPower.isOn)
        {
            CreateTextObject("Pow.Potential: " + material.powerPotential);
            CreateTextObject("----------------------------");
        }
    }
    void LoadInstitutionObject(Institution institution)
    {
        titleText.text = institution.name;
        if (ui.toggleShowDatabase.isOn)
        {
            foreach (string str in institution.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + institution.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (ui.toggleShowPower.isOn)
        {
            CreateTextObject("Pow.NamedOwners: " + institution.namedOwnerPower); 
            CreateTextObject("Pow.NamedCoops: " + institution.namedCooperativePower); 
            CreateTextObject("Pow.NamedOwnees: " + institution.namedOwneePower); 
            CreateTextObject("Pow.GenerOwners: " + institution.genericOwnerPower); 
            CreateTextObject("Pow.GenerCoops: " + institution.genericCooperativePower); 
            CreateTextObject("Pow.GenerOwnees: " + institution.genericOwnerPower); 
            CreateTextObject("Pow.Material: " + institution.materialPower); 
            CreateTextObject("Pow.Total: " + institution.totalPower);
            CreateTextObject("----------------------------");
        }
         
}
    void LoadRelationObject(Relation relation)
    {
        titleText.text = relation.name;
        CreateTextObject("Type: " + relation.relationType);
        CreateLinkObject("Primary: "+ relation.primaryDataObject.name, relation.primaryDataObject);
        CreateLinkObject("Secondary: "+ relation.secondaryDataObject.name, relation.secondaryDataObject);


    }
    void CreateTextObject(string content)
    {
        GameObject textObj = Instantiate(textPrefab, listParent);
        textObj.GetComponent<TextMeshProUGUI>().text = content;
        textPrefabList.Add(textObj);
    }
    void CreateLinkObject(string content, DataObject linkedObject)
    {
        GameObject linkObj = Instantiate(linkPrefab, listParent);
        linkObj.GetComponent<TextMeshProUGUI>().text = content;
        linkObj.GetComponent<LinkPrefab>().linkedObject = linkedObject;
        linkPrefabList.Add(linkObj);
    }
}
