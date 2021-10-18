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
    public GameObject panelRegularView;
    public GameObject panelObjectView;
    public Toggle toggleCharacter;
    public Toggle toggleMaterial;
    public Toggle toggleInstitution;
    public Toggle toggleRelation;
    public Toggle toggleShowDatabase;
    public Toggle toggleShowPower;
    public Toggle toggleShowRelations;
    public Slider sliderCardWidth;
    public Slider sliderCardHeight;
    public TextMeshProUGUI textSelectedObject;
    public GameObject objectViewSwitch;
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
    public List<ObjectViewCard> cardListObjectView = new List<ObjectViewCard>();
    [HideInInspector] public DataObject selectedObject;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        panelRegularView.SetActive(true);
        panelObjectView.SetActive(false);
        objectViewSwitch.SetActive(false);
        panelRegularView.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sliderCardWidth.value, sliderCardHeight.value);
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
    }


    public void ReloadObjectCards()
    {
        data = DataController.Instance;

        ClearObjectCards();

        if (toggleCharacter.isOn)
            foreach (Character cha in data.characterList)
            {
                GameObject cardObj = Instantiate(cardPrefab, panelRegularView.GetComponent<Transform>());
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(cha);
                cardList.Add(card);
            }
        if (toggleMaterial.isOn)
            foreach (Material mat in data.materialList)
            {
                GameObject cardObj = Instantiate(cardPrefab, panelRegularView.GetComponent<Transform>());
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(mat);
                cardList.Add(card);
            }
        if (toggleInstitution.isOn)
            foreach (Institution ins in data.institutionList)
            {
                GameObject cardObj = Instantiate(cardPrefab, panelRegularView.GetComponent<Transform>());
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(ins);
                cardList.Add(card);
            }
        if (toggleRelation.isOn)
            foreach (Relation rel in data.relationList)
            {
                GameObject cardObj = Instantiate(cardPrefab, panelRegularView.GetComponent<Transform>());
                ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
                card.LoadDataObject(rel);
                cardList.Add(card);
            }

    }
    void ReloadObjectView()
    {
        // clear previous cards
        foreach (ObjectViewCard existingCard in cardListObjectView)
            Destroy(existingCard.gameObject);
        cardListObjectView = new List<ObjectViewCard>();

        GameObject cardObj = Instantiate(cardPrefab, panelObjectView.GetComponent<Transform>());
        ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
        card.LoadDataObject(selectedObject);
        card.SetSelectedStatus(true);
        cardListObjectView.Add(card);

    }


    void ClearObjectCards()
    {
        foreach (ObjectViewCard card in cardList)        
            Destroy(card.gameObject);
        cardList = new List<ObjectViewCard>(); 
    }

    public void ToggleRegularView ( )
    {
        panelRegularView.SetActive(true );
        panelObjectView.SetActive(false);
    }
    public void ToggleObjectView( )
    {
        panelRegularView.SetActive(false);
        panelObjectView.SetActive(true);
        ReloadObjectView();
    }
    public void SlideCardWidth (float width)
    {
        GridLayoutGroup cardGrid = panelRegularView.GetComponent<GridLayoutGroup>();
        cardGrid.cellSize = new Vector2(width, cardGrid.cellSize.y);
       
    }
    public void SlideCardHeight(float height)
    {
        GridLayoutGroup cardGrid = panelRegularView.GetComponent<GridLayoutGroup>();
        cardGrid.cellSize = new Vector2( cardGrid.cellSize.x, height);
    }

    // called from LinkPrefab
    public void ClickLinkPrefab(LinkPrefab clickedLink)
    {
        SelectObject(clickedLink.linkedObject);
    }
    public void SelectObject(DataObject dataObject)
    {
        selectedObject = dataObject;
        textSelectedObject.text = "SELECTED:\n\n" + dataObject.ID;
        ObjectViewCard viewCard = null;
        foreach (ObjectViewCard card in cardList)
        {
            card.SetSelectedStatus(false);
            if (card.containedObject == dataObject)
                viewCard = card; 
        }
        if (viewCard != null)
            viewCard.SetSelectedStatus(true);
        ReloadObjectView();
        objectViewSwitch.SetActive(true);

    }
}
