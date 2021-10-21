using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataChanger : MonoBehaviour
{



    // GENERAL FUNCTIONS

    public void RemoveDataObject(DataObject dataObject)
    {

        List<Relation> destroyedRelations = DataController.Instance.GetRelationsThatIncludeObject(dataObject);
        List<ObjectViewCard> destroyedCards = new List<ObjectViewCard>();

        foreach (ObjectViewCard card in UIController.Instance.cardListRegular)
        {
            if (card.containedObject == dataObject)
                destroyedCards.Add(card);
            else if (card.containedObject.dataType == DataObject.DataType.Relation)
            {
                Relation rel = (Relation)card.containedObject;
                if (destroyedRelations.Contains(rel))
                    destroyedCards.Add(card);
            }
        }
        foreach (ObjectViewCard card in UIController.Instance.cardListFocusView)
        {
            if (card.containedObject == dataObject)
                destroyedCards.Add(card);
            else if (card.containedObject.dataType == DataObject.DataType.Relation)
            {
                if (destroyedRelations.Contains((Relation)card.containedObject))
                    destroyedCards.Add(card);
            }
        }
        foreach (ObjectViewCard card in destroyedCards)
        {
            card.containedObject = null;
            UIController.Instance.cardListFocusView.Remove(card);
            Destroy(card.gameObject);
        }
        if (dataObject == UIController.Instance.primarySelectedObject)
            UIController.Instance.DeselectPrimarySelectionObject();

        if (dataObject.dataType == DataObject.DataType.Character)
            DataController.Instance.characterList.Remove((Character)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Material)
            DataController.Instance.materialList.Remove((Material)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Scheme)
            DataController.Instance.schemeList.Remove((Scheme)dataObject);

        foreach (Relation relation in destroyedRelations)
            DestroyRelation(relation);

        UIController.Instance.ReloadObjectCards();
    }
    public void DestroyRelation(Relation relation)
    {
        DataController.Instance.relationList.Remove(relation);
    }


    public void TransferMaterialOwnership(DataObject oldOwner, DataObject newOwner, Material transferredMaterial)
    {
        // Destroy previous ownership relation
        DestroyRelation(DataController.Instance.GetRelationWithTheseTwoDataObjects(oldOwner, transferredMaterial));

        // Create new ownership relation
        DataController.Instance.CreateRelation(Relation.RelationType.Ownership, newOwner, transferredMaterial);
        UIController.Instance.ReloadObjectCards();
    }




    // ACTION FUNCTIONS

    public void KillDataObject(DataObject dataObject)
    {
        // 1. Character gets destroyed
        if (dataObject.dataType == DataObject.DataType.Character)
            KillCharacter((Character)dataObject);


    }

    public void KillCharacter(Character character)
    {
        DataController data = DataController.Instance;

        // Material: if CHA is owner or coop with any schemes, choose the most powerful, transfers to that Scheme; 
        // otherwise, if owner is ownee of a scheme, go to that scheme; 
        // otherwise, material is destroyed! 

        List<Material> ownedMaterials = data.GetCharacterOwnedMaterials(character);
        List<Scheme> ownedSchemes = data.GetCharacterOwnedSchemes(character);

        // Does CHA own any MAT? 
        if (ownedMaterials.Count != 0)
        {
            foreach (Material mat in ownedMaterials)
            {

                // Is CHA owner of one or multiple SCH?
                List<Scheme> ownedSchemeList = data.GetCharacterOwnedSchemes(character);
                List<Scheme> coopSchemeList = data.GetCharacterCoopSchemes(character);
                List<Scheme> ownerSchemeList = data.GetCharacterOwningSchemes(character);

                // Zero Owned SCH
                if (ownedSchemeList.Count == 0)
                {
                    // Zero Coop SCH
                    if (coopSchemeList.Count == 0)
                    {
                        // Zero Owning SCH
                        if (ownerSchemeList.Count == 0)
                        {
                            RemoveDataObject(mat);
                        }
                        else if (ownerSchemeList.Count == 1)
                        {
                            TransferMaterialOwnership(character, ownerSchemeList[0], mat);
                        }
                        else if (ownerSchemeList.Count > 1)
                        {
                            // GIVE TO STRONGEST
                            TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(ownerSchemeList), mat);
                        }
                    }
                    else if (coopSchemeList.Count == 1)
                    {
                        TransferMaterialOwnership(character, coopSchemeList[0], mat);
                    }
                    if (coopSchemeList.Count > 1)
                    {
                        TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(coopSchemeList), mat);
                    }
                }
                // One Owned SCH
                else if (ownedSchemeList.Count == 1)
                {
                    TransferMaterialOwnership(character, ownedSchemeList[0], mat);
                }
                // Multiple Owned SCH
                else if (ownedSchemeList.Count > 1)
                {
                    TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(ownedSchemeList), mat);
                }
            }
        }


        // Scheme: ownership goes to next-most-powerful cooper. 
        // If no coopers, to most pwoerful scheme owner. if no scheme owners, to most powerful ownee
        if (ownedSchemes.Count != 0)
        {
            foreach (Scheme sch in ownedSchemes)
            {

            }
        }

        RemoveDataObject(character);
    }

    public void GiftMaterial(DataObject receiverObject, string materialSubtype)
    {
        Material.MaterialSubtype subType = (Material.MaterialSubtype)System.Enum.Parse(typeof(Material.MaterialSubtype), materialSubtype);
            
        // create standard material object of that subtype (determine type from it)
        // create ownership relation
    }

    public void ChangeMaterialOwnership(DataObject receiverObject,  DataObject material)
    {
        // destroy relation that indicated ownership of material;
        // create relation for new ownership



    }

    public void BreakCooperationNeutral(DataObject primaryObject, DataObject secondaryObject)
    {
        // destroy relevant relation

    }

    public void BreakCooperationHostile(DataObject primaryObject, DataObject secondaryObject)
    {
        // destroy relevant relation
        // some way to grab power / material for primaryObject

    }


}
