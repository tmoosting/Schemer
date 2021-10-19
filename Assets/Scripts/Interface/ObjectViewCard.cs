using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectViewCard : MonoBehaviour
{ 
    public DataObject containedObject;
    //  public DataObject previousViewObject; use to navigate back when clicking a link

    public TextMeshProUGUI titleText;
    public Transform listParent;
    public Image selectionImage;
    public Color32 defaultColor;
    public Color32 selectionColor;

    public GameObject textPrefab;
    public GameObject linkPrefab;
    List<GameObject> textPrefabList = new List<GameObject>();
    List<GameObject> linkPrefabList = new List<GameObject>();

    [HideInInspector] public bool clickable = true;

    
    public void ClickSelectButton()
    {
        UIController.Instance.SelectObject(containedObject);
    }
    public void SetSelectedStatus(bool selected)
    {
        if (selected == true)
            selectionImage.color = selectionColor;
        else
            selectionImage.color = defaultColor;
        
    }

    // Delegator
    public void LoadDataObject(DataObject dataObject)
    {
        containedObject = dataObject;
        if (dataObject.dataType == DataObject.DataType.Character)
            LoadCharacterObject((Character)dataObject);
        if (dataObject.dataType == DataObject.DataType.Material)
            LoadMaterialObject((Material)dataObject);
        if (dataObject.dataType == DataObject.DataType.Scheme)
            LoadInstitutionObject((Scheme)dataObject);
        if (dataObject.dataType == DataObject.DataType.Relation)
            LoadRelationObject((Relation)dataObject);
    }

   
    void LoadCharacterObject(Character character)
    {
        titleText.text = character.name;

        if (UIController.Instance.toggleShowDatabase.isOn)
        {
            foreach (string str in character.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + character.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowPower.isOn)
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
        if (UIController.Instance.toggleShowDatabase.isOn)
        {
            foreach (string str in material.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + material.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowPower.isOn)
        {
            CreateTextObject("Pow.Potential: " + material.powerPotential);
            CreateTextObject("----------------------------");
        }
    }
    void LoadInstitutionObject(Scheme institution)
    {
        titleText.text = institution.name;
        if (UIController.Instance.toggleShowDatabase.isOn)
        {
            foreach (string str in institution.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + institution.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowPower.isOn)
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
        linkObj.GetComponent<LinkPrefab>().parentCard = this;
        linkPrefabList.Add(linkObj);
    }
}
