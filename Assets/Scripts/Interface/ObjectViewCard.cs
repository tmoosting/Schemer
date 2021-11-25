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
        if (dataObject.dataType == DataObject.DataType.Institution)
            LoadSchemeObject((Institution)dataObject);
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
                    if (character.fieldValueDict[str] != "")
                      CreateTextObject(str + ": " + character.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        else
            CreateTextObject(character.ID);
        if (UIController.Instance.toggleShowPower.isOn)
        { 
            CreateTextObject("Fearfulness: "+ character.coercion);
            CreateTextObject("Charisma: "+ character.charisma);
            CreateTextObject("Skill: "+ character.capability);
            CreateTextObject("Pow.Potential: "+ character.powerPotential.ToString("F1"));
            CreateTextObject("Pow.Material: "+ character.materialPower.ToString("F1"));
            CreateTextObject("Pow.Instus: "+ character.institutionsPower.ToString("F1"));
            CreateTextObject("Pow.IndInstus: "+ character.indirectInstitutionsPower.ToString("F1"));
            CreateTextObject("Pow.Total: "+ character.totalPower.ToString("F1"));
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
                    if (material.fieldValueDict[str] != "")
                        CreateTextObject(str + ": " + material.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        else
            CreateTextObject(material.ID);
        if (UIController.Instance.toggleShowPower.isOn)
        {
            CreateTextObject("Base Amount: " + material.baseAmount.ToString("F1"));
            CreateTextObject("Pow.Potential: " + material.totalPower.ToString("F1"));
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
    void LoadSchemeObject(Institution scheme)
    {
        titleText.text = scheme.name;
        if (UIController.Instance.toggleShowDatabase.isOn)
        {
            foreach (string str in scheme.fieldValueDict.Keys)
                if (str != "Name")
                {
                    if (scheme.fieldValueDict[str] != "")
                        CreateTextObject(str + ": " + scheme.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }
        else
            CreateTextObject(scheme.ID);
        if (UIController.Instance.toggleShowPower.isOn)
        {
            CreateTextObject("Pow.NamedOwners: " + scheme.namedOwnerPower.ToString("F1")); 
            CreateTextObject("Pow.NamedCoops: " + scheme.namedCooperativePower.ToString("F1")); 
            CreateTextObject("Pow.NamedOwnees: " + scheme.namedOwneePower.ToString("F1")); 
            CreateTextObject("Pow.GenerOwners: " + scheme.genericOwnerPower.ToString("F1")); 
            CreateTextObject("Pow.GenerCoops: " + scheme.genericCooperativePower.ToString("F1")); 
            CreateTextObject("Pow.GenerOwnees: " + scheme.genericOwneePower.ToString("F1")); 
            CreateTextObject("Pow.Material: " + scheme.materialPower.ToString("F1")); 
            CreateTextObject("Pow.Cooperation: " + scheme.cooperationPower.ToString("F1")); 
            CreateTextObject("Pow.Total: " + scheme.totalPower.ToString("F1"));
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
        CreateTextObject(relation.ID);
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
        else if (containedObject.dataType == DataObject.DataType.Institution)
            color = UIController.Instance.colorViewCardScheme;
        else if (containedObject.dataType == DataObject.DataType.Relation)
            color = UIController.Instance.colorViewCardRelation; 
        selectionImage.color = color;
        //     mainImage.color = color; 
        if (UIController.Instance.primarySelectedObject == containedObject)
            mainImage.color = UIController.Instance.colorViewCardSelected; 
        else
            mainImage.color = UIController.Instance.colorViewCardDefault;

    }

    public int GetRelationAmount()
    {
        return DataController.Instance.GetRelationAmount(containedObject);
    }
   
}
