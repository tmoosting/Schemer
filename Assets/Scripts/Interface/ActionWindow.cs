using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class ActionWindow : MonoBehaviour
{
    UIController ui;
    DataObject.DataType primaryType;
    DataObject.DataType secondaryType;
    public Button destroyButton;
    public Button giftButton;
    public Button claimButton;
    public Button createCoopButton;
    public Button breakNeutralButton;
    public Button breakHostileButton;
    public TextMeshProUGUI destroyText;
    public TextMeshProUGUI giftText;
    public TextMeshProUGUI claimText;
    public TextMeshProUGUI createCoopText; 
    public TextMeshProUGUI breakCoopText;
    public CustomDropdown giftDropdown;

     
    public void UpdateActionWindow()
    {
        ui = UIController.Instance;
        primaryType = ui.primarySelectedObject.dataType;
        if (ui.secondarySelectedObject != null)
            secondaryType = ui.secondarySelectedObject.dataType;

        // DESTROY
        string str1 = "";
        if (primaryType == DataObject.DataType.Character)
            str1 += "KILL ";
        else if (primaryType == DataObject.DataType.Material)
            str1 += "DESTROY ";
        else if (primaryType == DataObject.DataType.Scheme)
            str1 += "DISBAND ";     
        else if (primaryType == DataObject.DataType.Relation)
            str1 += "REMOVE ";
        str1 += ui.primarySelectedObject.ID;
        destroyText.text = str1;

        // GIFT
     //   if (primaryType != DataObject.DataType.Material && primaryType != DataObject.DataType.Relation)
        if (primaryType != DataObject.DataType.Relation)
        {
            giftButton.gameObject.SetActive(true);
            giftText.gameObject.SetActive(true);
            giftDropdown.gameObject.SetActive(true);
            giftText.text = "GIFT TO " + ui.primarySelectedObject.ID;
            // TODO: gift elements and logic
        }
        else
        {
            giftButton.gameObject.SetActive(false);
            giftText.gameObject.SetActive(false);
            giftDropdown.gameObject.SetActive(false);
        }

        if (ui.secondarySelectedObject != null)
        {
            secondaryType = ui.secondarySelectedObject.dataType;
            // CLAIM

            if (primaryType == DataObject.DataType.Character && secondaryType == DataObject.DataType.Character)
            {
                //disable, cha cannot own cha
                claimButton.gameObject.SetActive(false);
                claimText.gameObject.SetActive(false);
            }
            else if (primaryType == DataObject.DataType.Character || primaryType == DataObject.DataType.Scheme)
            {
                // check that ownership does not already exist
                bool existingOwnership = false;
                foreach (Relation rel in DataController.Instance.relationList)   
                    if (rel.relationType == Relation.RelationType.Ownership)
                        if (rel.primaryDataObject == ui.primarySelectedObject && rel.secondaryDataObject == ui.secondarySelectedObject)
                            existingOwnership = true;
                
                if (existingOwnership == false)
                {
                    claimButton.gameObject.SetActive(true);
                    claimText.gameObject.SetActive(true);
                    claimText.text = ui.primarySelectedObject.ID + " CLAIMS " + ui.secondarySelectedObject.ID;
                }
                else
                {
                    claimButton.gameObject.SetActive(false);
                    claimText.gameObject.SetActive(true);
                    claimText.text = ui.primarySelectedObject.ID + " ALREADY OWNS " + ui.secondarySelectedObject.ID;
                }
            }
            else
            {
                //disable
                claimButton.gameObject.SetActive(false);
                claimText.gameObject.SetActive(false);
            }

            // CREATECOOP
            // BREAKNEUTRAL 
            // BREAKHOSTILE

            if ((primaryType == DataObject.DataType.Character && secondaryType == DataObject.DataType.Character) ||
                primaryType == DataObject.DataType.Material || secondaryType == DataObject.DataType.Material)
            {
                //disable, none of these configurations allowed for either create or break
                createCoopButton.gameObject.SetActive(false);
                createCoopButton.gameObject.SetActive(false);
                breakNeutralButton.gameObject.SetActive(false);
                breakHostileButton.gameObject.SetActive(false);
                createCoopText.gameObject.SetActive(false);
                breakCoopText.gameObject.SetActive(false); 
            }
            else
            {

                bool coopRelationExists = DataController.Instance.IsFirstObjectCoopWithSecondObject(ui.primarySelectedObject, ui.secondarySelectedObject);
               
                if (coopRelationExists == false)
                {
                    // CREATE  
                    createCoopButton.gameObject.SetActive(true);
                    createCoopText.gameObject.SetActive(true);
                    breakNeutralButton.gameObject.SetActive(false);
                    breakHostileButton.gameObject.SetActive(false);
                    breakCoopText.gameObject.SetActive(false);
                    createCoopText.text = "CREATE COOP: " + ui.primarySelectedObject.ID + " - " + ui.secondarySelectedObject.ID;
                }
                else if (coopRelationExists == true)
                {
                    // BREAK 
                    createCoopButton.gameObject.SetActive(false);
                    createCoopText.gameObject.SetActive(false);
                    breakNeutralButton.gameObject.SetActive(true);
                //    breakHostileButton.gameObject.SetActive(true);
                    breakCoopText.gameObject.SetActive(true);
                    breakCoopText.text = "BREAK COOP: "+  ui.primarySelectedObject.ID + " - " + ui.secondarySelectedObject.ID;
                } 
            }
        }
        else
        {
            claimButton.gameObject.SetActive(false);
            claimText.gameObject.SetActive(false);
            createCoopButton.gameObject.SetActive(false);
            breakNeutralButton.gameObject.SetActive(false);
            breakHostileButton.gameObject.SetActive(false);
            createCoopText.gameObject.SetActive(false);
            breakCoopText.gameObject.SetActive(false);
        }
     


    }




    // BUTTON FUNCTIONS

    public void ClickDestroyButton()
    {
        UIController.Instance.PlayExplosionAtObject(UIController.Instance.primarySelectedObject);
        DataController.Instance.changer.KillDataObject(UIController.Instance.primarySelectedObject);
    //    UpdateActionWindow();
    }

    public void ClickGiftButton()
    {
        DataController.Instance.changer.GiftMaterial(UIController.Instance.primarySelectedObject, giftDropdown.selectedText.text);
        UpdateActionWindow();
    }

    public void ClickClaimButton()
    {
        DataController.Instance.changer.ClaimDataObject(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
        UpdateActionWindow();
    }
    public void ClickCreateCoopButton()
    {
        DataController.Instance.changer.CreateCooperation(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
        UpdateActionWindow();
    }
    public void ClickBreakNeutralButton()
    {
        DataController.Instance.changer.BreakCooperationNeutral(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
        UpdateActionWindow();
    }

    public void ClickBreakHostileButton()
    {
        DataController.Instance.changer.BreakCooperationHostile(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
        UpdateActionWindow();
    }





}
