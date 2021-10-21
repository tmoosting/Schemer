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
    public Button breakNeutralButton;
    public Button breakHostileButton;
    public TextMeshProUGUI destroyText;
    public TextMeshProUGUI giftText;
    public TextMeshProUGUI claimText;
    public TextMeshProUGUI breakNeutralText;
    public TextMeshProUGUI breakHostileText;
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
        if (primaryType != DataObject.DataType.Material && primaryType != DataObject.DataType.Relation)
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
                //disable
                claimButton.gameObject.SetActive(false);
                claimText.gameObject.SetActive(false);
            }
            else if (primaryType == DataObject.DataType.Character || primaryType == DataObject.DataType.Scheme)
            {
                claimButton.gameObject.SetActive(true);
                claimText.gameObject.SetActive(true);
                claimText.text = ui.primarySelectedObject.ID + " CLAIMS " + ui.secondarySelectedObject.ID;
            }
            else
            {
                //disable
                claimButton.gameObject.SetActive(false);
                claimText.gameObject.SetActive(false);
            }

            // BREAKNEUTRAL 
            // BREAKHOSTILE

            if ((primaryType == DataObject.DataType.Character && secondaryType == DataObject.DataType.Character) ||
                primaryType == DataObject.DataType.Material || secondaryType == DataObject.DataType.Material)
            {
                //disable
                breakNeutralButton.gameObject.SetActive(false);
                breakHostileButton.gameObject.SetActive(false);
                breakNeutralText.gameObject.SetActive(false);
                breakHostileText.gameObject.SetActive(false);

            }
            else
            {
                // all other cases viable
                breakNeutralButton.gameObject.SetActive(true);
                breakHostileButton.gameObject.SetActive(true);
                breakNeutralText.gameObject.SetActive(true);
                breakHostileText.gameObject.SetActive(true);
                breakNeutralText.text = ui.primarySelectedObject.ID + " BREAK NEUTRAL WITH " + ui.secondarySelectedObject.ID;
                breakHostileText.text = ui.primarySelectedObject.ID + " BREAK HOSTILE WITH " + ui.secondarySelectedObject.ID;
            }
        }
        else
        {
            claimButton.gameObject.SetActive(false);
            claimText.gameObject.SetActive(false);
            breakNeutralButton.gameObject.SetActive(false);
            breakHostileButton.gameObject.SetActive(false);
            breakNeutralText.gameObject.SetActive(false);
            breakHostileText.gameObject.SetActive(false);
        }
     


    }




    // BUTTON FUNCTIONS

    public void ClickDestroyButton()
    {
        DataController.Instance.changer.KillDataObject(UIController.Instance.primarySelectedObject);
    }

    public void ClickGiftButton()
    {
        DataController.Instance.changer.GiftMaterial(UIController.Instance.primarySelectedObject, giftDropdown.selectedText.text);
    }

    public void ClickClaimButton()
    {
        DataController.Instance.changer.ChangeMaterialOwnership(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
    }

    public void ClickBreakNeutralButton()
    {
        DataController.Instance.changer.BreakCooperationNeutral(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
    }

    public void ClickBreakHostileButton()
    {
        DataController.Instance.changer.BreakCooperationHostile(UIController.Instance.primarySelectedObject, UIController.Instance.secondarySelectedObject);
    }
   




}
