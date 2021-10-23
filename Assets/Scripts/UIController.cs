using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    DataController data;
    [Header("Options")]
    public bool showRelationsInFocusMode;
    public bool createKillEffect;
    public bool createGiftEffect;
    [Header("Startup - Objects")]
    public bool enableToggleCharactersOnStart;
    public bool enableToggleMaterialsOnStart;
    public bool enableToggleInstitutionsOnStart;
    public bool enableToggleRelationsOnStart;
    [Header("Startup - Details")]
    public bool enableToggleDatabaseInfoOnStart;
    public bool enableTogglePowerValuesOnStart;
    public bool enableToggleRelationDetailsOnStart;
    [Header("ViewCards")]
    public Color32 colorViewCardDefault;
    public Color32 colorViewCardCharacter;
    public Color32 colorViewCardMaterial;
    public Color32 colorViewCardScheme;
    public Color32 colorViewCardRelation;
    public Color32 colorViewCardSelected;
    public Color32 colorSelectButtonNormal;
    public Color32 colorSelectButtonSecondary;

    [Header("Assigns")]
    public GameObject cardPrefab;
    public GameObject panelRegularView;
    public GameObject panelRegularViewGrid;
    public GameObject panelFocusView;
    public GameObject panelFocusViewGridOwners;
    public GameObject panelFocusViewGridCoops;
    public GameObject panelFocusViewGridOwnees;
    public Toggle toggleCharacter;
    public Toggle toggleMaterial;
    public Toggle toggleInstitution;
    public Toggle toggleRelation;
    public Toggle toggleShowDatabase;
    public Toggle toggleShowPower;
    public Toggle toggleShowRelations;
    public Slider sliderCardWidth;
    public Slider sliderCardHeight;
    public TextMeshProUGUI textPrimarySelectedObject;
    public TextMeshProUGUI textSecondarySelectedObject;
    public GameObject focusViewSwitch;
    public GameObject focusViewLabel;
    public Image imageToggleColorCharacter;
    public Image imageToggleColorMaterial;
    public Image imageToggleColorScheme;
    public Image imageToggleColorRelation;
    public SwitchManager focusSwitch;
    public ActionWindow actionWindow;
    public GameObject explosionPrefab;
    public GameObject giftPrefab;


    [HideInInspector] 
    public List<ObjectViewCard> cardListRegular = new List<ObjectViewCard>();
    [HideInInspector] 
    public List<ObjectViewCard> cardListFocusView = new List<ObjectViewCard>();
    [HideInInspector] 
    public DataObject primarySelectedObject = null;
    [HideInInspector] 
    public DataObject secondarySelectedObject = null;
    bool focusViewMode = false;
    
    bool secondarySelectMode = false;


    enum SortMode { ID, Relations, Power}
    SortMode currentSortMode;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        panelRegularView.SetActive(true);
        panelFocusView.SetActive(false);
        focusViewSwitch.SetActive(false);
        focusViewLabel.SetActive(false);
        actionWindow.gameObject.SetActive(false); 
        textPrimarySelectedObject.gameObject.SetActive(false);
        textSecondarySelectedObject.gameObject.SetActive(false);
        panelRegularViewGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sliderCardWidth.value, sliderCardHeight.value);
        panelFocusViewGridOwners.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sliderCardWidth.value, sliderCardHeight.value);
        panelFocusViewGridCoops.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sliderCardWidth.value, sliderCardHeight.value);
        panelFocusViewGridOwnees.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sliderCardWidth.value, sliderCardHeight.value);
        imageToggleColorCharacter.color = colorViewCardCharacter;
        imageToggleColorMaterial.color = colorViewCardMaterial ;
        imageToggleColorScheme.color = colorViewCardScheme;
        imageToggleColorRelation.color = colorViewCardRelation;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            if (primarySelectedObject != null)
                focusSwitch.AnimateSwitch();
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl) ||
           Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.RightControl) ||
           Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand))
        {
            secondarySelectMode = true;
            foreach (ObjectViewCard card in cardListRegular)            
                card.SetSelectButtonColor(colorSelectButtonSecondary);
            foreach (ObjectViewCard card in cardListFocusView)
                card.SetSelectButtonColor(colorSelectButtonSecondary);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl) ||
        Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.RightControl) ||
        Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.RightCommand))
        {
            secondarySelectMode = false;
            foreach (ObjectViewCard card in cardListRegular)
                card.SetSelectButtonColor(colorSelectButtonNormal);
            foreach (ObjectViewCard card in cardListFocusView)
                card.SetSelectButtonColor(colorSelectButtonNormal);
        }
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

    public void PlayExplosionAtObject(DataObject dataObject)
    {
        if (createKillEffect == true)
        {
            bool playExplosion = false;
            ObjectViewCard card = null;
            if (focusViewMode == true)
                foreach (ObjectViewCard card1 in cardListFocusView)
                    if (card1.containedObject == dataObject)
                    {
                        card = card1;
                        playExplosion = true;
                    }
            if (focusViewMode == false)
                foreach (ObjectViewCard card1 in cardListRegular)
                    if (card1.containedObject == dataObject)
                    {
                        card = card1;
                        playExplosion = true;
                    }
            if (playExplosion == true)
            {
                GameObject obj = Instantiate(explosionPrefab);
                obj.transform.position = card.transform.position;
                ParticleSystem particles = obj.GetComponent<ParticleSystem>();
                SoundController.Instance.PlayKillAudio();

                particles.Play();

            }
        } 
      }
    public void PlayGiftSparksAtObject(DataObject dataObject)
    {
        if (createKillEffect == true)
        {
            bool playEffect = false;
            ObjectViewCard card = null;
            if (focusViewMode == true)
                foreach (ObjectViewCard card1 in cardListFocusView)
                    if (card1.containedObject == dataObject)
                    {
                        card = card1;
                        playEffect = true;
                    }
            if (focusViewMode == false)
                foreach (ObjectViewCard card1 in cardListRegular)
                    if (card1.containedObject == dataObject)
                    {
                        card = card1;
                        playEffect = true;
                    }
            if (playEffect == true)
            {
                GameObject obj = Instantiate(giftPrefab);
                obj.transform.position = card.transform.position;
                ParticleSystem particles = obj.GetComponent<ParticleSystem>();
                SoundController.Instance.PlayGiftAudio();

                particles.Play();

            }
        }
    }
    public void ReloadObjectCards()
    { 
        data = DataController.Instance; 
        if (focusViewMode == false)
            RecreateRegularViewCards();
        else if (focusViewMode == true)
            RecreateFocusViewCards();
    }

    void RecreateRegularViewCards()
    {
        ClearObjectCards(false);
        List<DataObject> cardObjects = new List<DataObject>(); 

        if (toggleCharacter.isOn)
            foreach (Character cha in data.characterList)           
                cardObjects.Add(cha);
        if (toggleMaterial.isOn)
            foreach (Material mat in data.materialList) 
                cardObjects.Add(mat); 
        if (toggleInstitution.isOn)
            foreach (Scheme ins in data.schemeList)
                cardObjects.Add(ins);
        if (toggleRelation.isOn)
            foreach (Relation rel in data.relationList)
                cardObjects.Add(rel);
         

        if (currentSortMode == SortMode.Relations)
            cardObjects.Sort(SortByRelationAmount);
        else if (currentSortMode == SortMode.Power)
            cardObjects.Sort(SortByPower);


        foreach (DataObject dataObject in cardObjects)
        {
            GameObject cardObj = Instantiate(cardPrefab, panelRegularViewGrid.GetComponent<Transform>());
            ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
            card.LoadDataObject(dataObject);
            card.SetBodyColor();
            cardListRegular.Add(card); 
        }

         

    }
    
    void RecreateFocusViewCards()
    {
        ClearObjectCards(true);

        // add base card
        GameObject cardObj = Instantiate(cardPrefab, panelFocusViewGridCoops.GetComponent<Transform>());
        ObjectViewCard card = cardObj.GetComponent<ObjectViewCard>();
        card.LoadDataObject(primarySelectedObject);
        card.SetSelectedStatus(true);
        cardListFocusView.Add(card);

        List<DataObject> relatedObjects = new List<DataObject>();
        if (primarySelectedObject.dataType != DataObject.DataType.Relation)
        {
            foreach (DataObject obj in data.GetRelatedObjectsToObject(primarySelectedObject))
                if (showRelationsInFocusMode == false && obj.dataType == DataObject.DataType.Relation)
                {
                    // do nothing because relation excluded is enabled
                }
                else
                {
                    if (toggleCharacter.isOn == false && obj.dataType == DataObject.DataType.Character ||
                 toggleMaterial.isOn == false && obj.dataType == DataObject.DataType.Material ||
                 toggleInstitution.isOn == false && obj.dataType == DataObject.DataType.Scheme ||
                 toggleRelation.isOn == false && obj.dataType == DataObject.DataType.Relation)
                    {
                        // exclude unselected object types, so do nothing here
                    }
                    else
                        relatedObjects.Add(obj);
                }
        }
        else
        {
            Relation rel = (Relation)primarySelectedObject;
            relatedObjects.Add(rel.primaryDataObject);
            relatedObjects.Add(rel.secondaryDataObject);
        }
            
      





        if (currentSortMode == SortMode.Relations)
            relatedObjects.Sort(SortByRelationAmount);
        else if (currentSortMode == SortMode.Power)
            relatedObjects.Sort(SortByPower);


        foreach (DataObject obj in relatedObjects)
        {
            Transform rt = null;
            if (data.IsFirstObjectOwnerOfSecondObject(primarySelectedObject, obj) == true)
                rt = panelFocusViewGridOwnees.GetComponent<Transform>();
            else if (data.IsFirstObjectCoopWithSecondObject(primarySelectedObject, obj) == true)
                rt = panelFocusViewGridCoops.GetComponent<Transform>();
            else if (data.IsFirstObjectOwneeOfSecondObject(primarySelectedObject, obj) == true)
                rt = panelFocusViewGridOwners.GetComponent<Transform>();
            else if (obj.dataType == DataObject.DataType.Relation)
                rt = panelFocusViewGridOwnees.GetComponent<Transform>();
            else if (primarySelectedObject.dataType == DataObject.DataType.Relation)
            {
                Relation rel = (Relation)primarySelectedObject;
                if (obj == rel.primaryDataObject)
                    rt = panelFocusViewGridOwners.GetComponent<Transform>();
                else if (obj == rel.secondaryDataObject)
                    rt = panelFocusViewGridOwnees.GetComponent<Transform>(); 
            } 
            else
                Debug.LogWarning("Strange! " + primarySelectedObject.ID + " found no focusview-position for " + obj.ID); 

            GameObject cardObject = Instantiate(cardPrefab, rt);
            ObjectViewCard cardCard = cardObject.GetComponent<ObjectViewCard>();
            cardCard.LoadDataObject(obj);
            cardListFocusView.Add(cardCard);
        } 
        foreach (ObjectViewCard carrd in cardListFocusView)
            carrd.SetBodyColor();
    }
    int SortByRelationAmount(DataObject obj1, DataObject obj2)
    {
        return DataController.Instance.GetRelationAmount(obj2).CompareTo(DataController.Instance.GetRelationAmount(obj1));
    }
    int SortByPower(DataObject obj1, DataObject obj2)
    {
        return DataController.Instance.GetTotalPower(obj2).CompareTo(DataController.Instance.GetTotalPower(obj1));

    }

    void ClearObjectCards(bool focusView)
    {
        if (focusView == false)
        {
            foreach (ObjectViewCard card in cardListRegular)
                Destroy(card.gameObject);
            cardListRegular = new List<ObjectViewCard>();
        }
        else if (focusView == true)
        {
            foreach (ObjectViewCard card in cardListFocusView)
                Destroy(card.gameObject);
            cardListFocusView = new List<ObjectViewCard>();
        }


    }


    public void SortCardsByAlphabet(bool enabled)
    { 
        if (enabled)
        {
            currentSortMode = SortMode.ID;
            ReloadObjectCards();
        } 
    }
    public void SortCardsByRelationAmount(bool enabled)
    {
        if (enabled)
        {
            currentSortMode = SortMode.Relations;
            ReloadObjectCards();
        } 
    }
    public void SortCardsByTotalPower(bool enabled)
    {
        if (enabled)
        {
            currentSortMode = SortMode.Power;
            ReloadObjectCards();
        } 
    }


    public void ToggleRegularView ( )
    {
        focusViewMode = false;
        panelRegularView.SetActive(true );
        panelFocusView.SetActive(false);
     //   focusViewLabel.SetActive(false);
        ReloadObjectCards();
    }
    public void ToggleFocusView( )
    {
        focusViewMode = true;
        panelRegularView.SetActive(false);
        panelFocusView.SetActive(true);
    //    focusViewLabel.SetActive(true);
        ReloadObjectCards();
    }
    public void SlideCardWidth (float width)
    {
        GridLayoutGroup cardGrid = panelRegularViewGrid.GetComponent<GridLayoutGroup>();
        cardGrid.cellSize = new Vector2(width, cardGrid.cellSize.y);
        GridLayoutGroup cardGridFocusOwners = panelFocusViewGridOwners.GetComponent<GridLayoutGroup>();
        cardGridFocusOwners.cellSize = new Vector2(width, cardGridFocusOwners.cellSize.y);
        GridLayoutGroup cardGridFocusCoops = panelFocusViewGridCoops.GetComponent<GridLayoutGroup>();
        cardGridFocusCoops.cellSize = new Vector2(width, cardGridFocusCoops.cellSize.y);
        GridLayoutGroup cardGridFocusOwnees = panelFocusViewGridOwnees.GetComponent<GridLayoutGroup>();
        cardGridFocusOwnees.cellSize = new Vector2(width, cardGridFocusOwnees.cellSize.y);
    }
    public void SlideCardHeight(float height)
    {
        GridLayoutGroup cardGrid = panelRegularViewGrid.GetComponent<GridLayoutGroup>();
        cardGrid.cellSize = new Vector2( cardGrid.cellSize.x, height);
        GridLayoutGroup cardGridFocusOwners = panelFocusViewGridOwners.GetComponent<GridLayoutGroup>();
        cardGridFocusOwners.cellSize = new Vector2(cardGridFocusOwners.cellSize.x, height);
        GridLayoutGroup cardGridFocusCoops = panelFocusViewGridCoops.GetComponent<GridLayoutGroup>();
        cardGridFocusCoops.cellSize = new Vector2(cardGridFocusCoops.cellSize.x, height);
        GridLayoutGroup cardGridFocusOwnees = panelFocusViewGridOwnees.GetComponent<GridLayoutGroup>();
        cardGridFocusOwnees.cellSize = new Vector2(cardGridFocusOwnees.cellSize.x, height);
    }

    // called from LinkPrefab
    public void ClickLinkPrefab(LinkPrefab clickedLink)
    {
        SelectObject(clickedLink.linkedObject);
    }
    public void ClickSelectButton(ObjectViewCard card)
    {
        if (secondarySelectMode == true)        
            SecondarySelectObject(card.containedObject);
        else
           SelectObject(card.containedObject);

    }
    public void SelectObject(DataObject dataObject)
    {
        
        primarySelectedObject = dataObject;
        textPrimarySelectedObject.gameObject.SetActive(true); 
        textPrimarySelectedObject.text = "PRIMARY: " + dataObject.ID + " - "+ dataObject.name;
        ObjectViewCard viewCard = null;
        foreach (ObjectViewCard card in cardListRegular)
        {
            card.SetSelectedStatus(false);
            if (card.containedObject == dataObject)
                viewCard = card; 
        }
        if (viewCard != null)
            viewCard.SetSelectedStatus(true);
        if (focusViewMode)
            ReloadObjectCards(); 

        focusViewSwitch.SetActive(true);
        focusViewLabel.SetActive(true);
        actionWindow.gameObject.SetActive(true);
        foreach (ObjectViewCard carrd in cardListFocusView)
            carrd.SetBodyColor();

        if (dataObject == secondarySelectedObject)
            DeselectSecondarySelectionObject();
        else   // because update included in deselectsecondary function 
          actionWindow.UpdateActionWindow();
    }
    public void SecondarySelectObject(DataObject dataObject)
    {
        textSecondarySelectedObject.gameObject.SetActive(true); 
        secondarySelectedObject = dataObject;
        textSecondarySelectedObject.text = "SECONDARY: " + dataObject.ID + " - " + dataObject.name; 
        actionWindow.UpdateActionWindow();
    }
    public void DeselectPrimarySelectionObject()
    {
        if (focusViewMode == true)
            focusSwitch.AnimateSwitch();
        primarySelectedObject = null;
        textPrimarySelectedObject.gameObject.SetActive(false); 
        focusViewSwitch.SetActive(false);
        focusViewLabel.SetActive(false);
        actionWindow.gameObject.SetActive(false); 
    }
    public void DeselectSecondarySelectionObject()
    { 
        secondarySelectedObject = null;
        textSecondarySelectedObject.gameObject.SetActive(false);
        actionWindow.UpdateActionWindow();
    }
}
