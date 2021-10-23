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
    public Button selectButton;
    public TextMeshProUGUI selectButtonText;
    public Image selectButtonImage;
    public Transform listParent;
    public Image selectionImage;
    public Image mainImage;
    public Color32 defaultColor;
    public Color32 selectionColor;

    public GameObject textPrefab;
    public GameObject linkPrefab;
    List<GameObject> textPrefabList = new List<GameObject>();
    List<GameObject> linkPrefabList = new List<GameObject>(); 

    [HideInInspector] public bool clickable = true;
    [HideInInspector] public string cardName;
     
    public void ClickSelectButton()
    {
        UIController.Instance.ClickSelectButton(this);
    }
    public void SetSelectButtonColor(Color32 color)
    {
        selectButtonText.color = color;
        selectButtonImage.color = color;
    }
    public void SetSelectedStatus(bool selected)
    {
        if (selected == true)
        {
            selectButton.gameObject.SetActive(false); 
        }
        else
        {
            selectButton.gameObject.SetActive(true);  
        }
        SetBodyColor();
    }

    // Delegator
    public void LoadDataObject(DataObject dataObject)
    {
        cardName = "CardFor" + dataObject.ID;
        containedObject = dataObject; 
        if (dataObject.dataType == DataObject.DataType.Character)
            LoadCharacterObject((Character)dataObject);
        if (dataObject.dataType == DataObject.DataType.Material)
            LoadMaterialObject((Material)dataObject);
        if (dataObject.dataType == DataObject.DataType.Scheme)
            LoadSchemeObject((Scheme)dataObject);
        if (dataObject.dataType == DataObject.DataType.Relation)
            LoadRelationObject((Relation)dataObject);
        SetSelectedStatus(containedObject == UIController.Instance.primarySelectedObject);
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
            CreateTextObject("Pow.Schemes: "+ character.schemesPower);
            CreateTextObject("Pow.Total: "+ character.totalPower);
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowRelations.isOn)
        {
            List<Relation> relations = DataController.Instance.GetRelationsThatIncludeObject(containedObject);
            foreach (Relation rel in relations)            
                if (rel.relationType == Relation.RelationType.Ownership)
                    if (rel.primaryDataObject == containedObject)
                        CreateLinkObject("Ownee: " , rel.secondaryDataObject);
            foreach (Relation rel in relations)
                if (rel.relationType == Relation.RelationType.Cooperative)
                {
                    if (rel.primaryDataObject == containedObject)
                        CreateLinkObject("Coops: " , rel.secondaryDataObject);
                    else if (rel.secondaryDataObject == containedObject)
                        CreateLinkObject("Coops: " , rel.primaryDataObject); 
                }
            foreach (Relation rel in relations)
                if (rel.relationType == Relation.RelationType.Ownership)
                    if (rel.secondaryDataObject == containedObject)
                        CreateLinkObject("Owner: ", rel.primaryDataObject); 
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
            CreateTextObject("Pow.Potential: " + material.totalPower);
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowRelations.isOn)
        {
            List<Relation> relations = DataController.Instance.GetRelationsThatIncludeObject(containedObject);
            foreach (Relation rel in relations)
                if (rel.relationType == Relation.RelationType.Ownership)
                    if (rel.secondaryDataObject == containedObject)
                        CreateLinkObject("Owner: ", rel.primaryDataObject);
        }
    }
    void LoadSchemeObject(Scheme scheme)
    {
        titleText.text = scheme.name;
        if (UIController.Instance.toggleShowDatabase.isOn)
        {
            foreach (string str in scheme.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + scheme.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowPower.isOn)
        {
            CreateTextObject("Pow.NamedOwners: " + scheme.namedOwnerPower); 
            CreateTextObject("Pow.NamedCoops: " + scheme.namedCooperativePower); 
            CreateTextObject("Pow.NamedOwnees: " + scheme.namedOwneePower); 
            CreateTextObject("Pow.GenerOwners: " + scheme.genericOwnerPower); 
            CreateTextObject("Pow.GenerCoops: " + scheme.genericCooperativePower); 
            CreateTextObject("Pow.GenerOwnees: " + scheme.genericOwnerPower); 
            CreateTextObject("Pow.Material: " + scheme.materialPower); 
            CreateTextObject("Pow.Cooperation: " + scheme.cooperationPower); 
            CreateTextObject("Pow.Total: " + scheme.totalPower);
            CreateTextObject("----------------------------");
        }
        if (UIController.Instance.toggleShowRelations.isOn)
        {
            List<Relation> relations = DataController.Instance.GetRelationsThatIncludeObject(containedObject);
            foreach (Relation rel in relations)
                if (rel.relationType == Relation.RelationType.Ownership)
                    if (rel.primaryDataObject == containedObject)
                        CreateLinkObject("Ownee: ", rel.secondaryDataObject);
            foreach (Relation rel in relations)
                if (rel.relationType == Relation.RelationType.Cooperative)
                {
                    if (rel.primaryDataObject == containedObject)
                        CreateLinkObject("Coops: ", rel.secondaryDataObject);
                    else if (rel.secondaryDataObject == containedObject)
                        CreateLinkObject("Coops: ", rel.primaryDataObject);
                }
            foreach (Relation rel in relations)
                if (rel.relationType == Relation.RelationType.Ownership)
                    if (rel.secondaryDataObject == containedObject)
                        CreateLinkObject("Owner: ", rel.primaryDataObject);
        }
    }
    void LoadRelationObject(Relation relation)
    {
        titleText.text = relation.name;
        CreateTextObject("Type: " + relation.relationType);
        CreateLinkObject("Pri: ", relation.primaryDataObject);
        CreateLinkObject("Sec: ", relation.secondaryDataObject);


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
        linkObj.GetComponent<TextMeshProUGUI>().text = content + linkedObject.ID ;
        linkObj.GetComponent<LinkPrefab>().linkedObject = linkedObject;
        linkObj.GetComponent<LinkPrefab>().parentCard = this;
        linkPrefabList.Add(linkObj);
    }
    public void SetBodyColor()
    {
        Color32 color = UIController.Instance.colorViewCardDefault;
        if (containedObject.dataType == DataObject.DataType.Character)
            color = UIController.Instance.colorViewCardCharacter;
        else if (containedObject.dataType == DataObject.DataType.Material)
            color = UIController.Instance.colorViewCardMaterial;
        else if (containedObject.dataType == DataObject.DataType.Scheme)
            color = UIController.Instance.colorViewCardScheme;
        else if (containedObject.dataType == DataObject.DataType.Relation)
            color = UIController.Instance.colorViewCardRelation; 
        selectionImage.color = color;
        //     mainImage.color = color; 
        if (UIController.Instance.primarySelectedObject == containedObject)
            selectionImage.color = UIController.Instance.colorViewCardSelected; 
    }

    public int GetRelationAmount()
    {
        return DataController.Instance.GetRelationAmount(containedObject);
    }
   
}
