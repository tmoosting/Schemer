using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        str1 += ui.primarySelectedObject.ID;
        destroyText.text = str1;

        // GIFT
        if (primaryType != DataObject.DataType.Material)
        {
            giftButton.gameObject.SetActive(true);
            giftText.gameObject.SetActive(true);
            giftText.text = "GIFT TO " + ui.primarySelectedObject.ID;
            // TODO: gift elements and logic
        }
        else
        {
            giftButton.gameObject.SetActive(false);
            giftText.gameObject.SetActive(false);
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

    }

    public void ClickGiftButton()
    {

    }

    public void ClickClaimButton()
    {

    }

    public void ClickBreakNeutralButton()
    {

    }

    public void ClickBreakHostileButton()
    {

    }
    // SINGLE DEPENDENT 

    // KILL / DESTROY
    // CHA / SCH / MAT


    // DUO DEPENDENT

    // BREAK COOP
    // CHA-SCH      INS-INS

    // TRANSFER OWNERSHIP
    // CHA-CHA    CHA-SCH   SCH-SCH









}
