using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    DataController data;
    [Header("Assigns")]
    public GameObject cardPrefab;
    public Transform cardParent;
    public Toggle toggleCharacter;
    public Toggle toggleMaterial;
    public Toggle toggleInstitution;
    public Toggle toggleRelation;
    public Toggle toggleShowDatabase;
    public Toggle toggleShowPower;
    public Toggle toggleShowRelations;
    public Slider sliderCardWidth;
    public Slider sliderCardHeight;
    [Header ("Startup - Objects")]
    public bool enableToggleCharactersOnStart;
    public bool enableToggleMaterialsOnStart;
    public bool enableToggleInstitutionsOnStart;
    public bool enableToggleRelationsOnStart;
    [Header("Startup - Details")]
    public bool enableToggleDatabaseInfoOnStart;
    public bool enableTogglePowerValuesOnStart;
    public bool enableToggleRelationDetailsOnStart;

    public List<ObjectViewCard> cardList = new List<ObjectViewCard>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
      cardParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sliderCardWidth.value, sliderCardHeight.value);
    }

    // Called from DataController when done creating
    public void RunStartupToggles()
    {
        toggleShowDatabase.isOn = enableToggleDatabaseInfoOnStart;
        toggleShowPower.isOn = enableTogglePowerValuesOnStart;
        toggleShowRelations.isOn = enableToggleRelationDetailsOnStart;
        toggleCharacter.isOn = enableToggleCharactersOnStart;
        toggleMaterial.isOn = enableToggleMaterialsOnStart;
        toggleInstitution.isOn = enableToggleInstitutionsOnStart;
        toggleRelation.isOn = enableToggleRelationsOnStart;
      


        // Below disabled because it does not update graphic
        //toggleCharacter.SetIsOnWithoutNotify(ToggleCharactersOnStart);
        //toggleMaterial.SetIsOnWithoutNotify(ToggleMaterialsOnStart);
        //toggleInstitution.SetIsOnWithoutNotify(ToggleInstitutionsOnStart);
        //toggleRelation.SetIsOnWithoutNotify(ToggleRelationsOnStart);
        //toggleShowDatabase.SetIsOnWithoutNotify(ToggleDatabaseInfoOnStart);
        //toggleShowPower.SetIsOnWithoutNotify(TogglePowerValuesOnStart);
        //toggleShowRelations.SetIsOnWithoutNotify(ToggleRelationDetailsOnStart);
        //ReloadObjectCards();
    }


    public void ReloadObjectCards()
    {
        data = DataController.Instance;

        ClearObjectCards();

        if (toggleCharacter.isOn)
            foreach (Character cha in data.characterList)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent);
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(cha);
                cardList.Add(card);
            }
        if (toggleMaterial.isOn)
            foreach (Material mat in data.materialList)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent);
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(mat);
                cardList.Add(card);
            }
        if (toggleInstitution.isOn)
            foreach (Institution ins in data.institutionList)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent);
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(ins);
                cardList.Add(card);
            }
        if (toggleRelation.isOn)
            foreach (Relation rel in data.relationList)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent);
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(rel);
                cardList.Add(card);
            }

    }



    void ClearObjectCards()
    {
        foreach (ObjectViewCard card in cardList)        
            Destroy(card.gameObject);
        cardList = new List<ObjectViewCard>();


    }


    public void SlideCardWidth (float width)
    {
        GridLayoutGroup cardGrid = cardParent.GetComponent<GridLayoutGroup>();
        cardGrid.cellSize = new Vector2(width, cardGrid.cellSize.y);
    }
    public void SlideCardHeight(float height)
    {
        GridLayoutGroup cardGrid = cardParent.GetComponent<GridLayoutGroup>();
        cardGrid.cellSize = new Vector2( cardGrid.cellSize.x, height);
    }
}
