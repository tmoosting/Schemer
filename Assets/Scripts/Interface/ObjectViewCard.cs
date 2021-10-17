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
    List<GameObject> textPrefabList = new List<GameObject>();

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

    void CreateTextObject (string content)
    {
        GameObject textObj = Instantiate(textPrefab, listParent);
        textObj.GetComponent<TextMeshProUGUI>().text = content;
        textPrefabList.Add(textObj);
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
            CreateTextObject("Pow.NamedExec: " + institution.namedExecutivePower); 
            CreateTextObject("Pow.NamedEnfo: " + institution.namedEnforcerPower); 
            CreateTextObject("Pow.NamedAtte: " + institution.namedAttendantPower); 
            CreateTextObject("Pow.GenerExec: " + institution.genericExecutivePower); 
            CreateTextObject("Pow.GenerEnfo: " + institution.genericEnforcerPower); 
            CreateTextObject("Pow.GenerAtte: " + institution.genericAttendantPower); 
            CreateTextObject("Pow.Material: " + institution.materialPower); 
            CreateTextObject("Pow.Total: " + institution.totalPower);
            CreateTextObject("----------------------------");
        }
         
}
    void LoadRelationObject(Relation relation)
    {
        titleText.text = relation.name;
        if (ui.toggleShowDatabase.isOn)
        {
            foreach (string str in relation.fieldValueDict.Keys)
                if (str != "Name")
                {
                    CreateTextObject(str + ": " + relation.fieldValueDict[str]);
                }
            CreateTextObject("----------------------------");
        }


    }

}
