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

        // check for relationless material leftover
        List<Material> destroyedMaterials = new List<Material>();
        if (dataObject.dataType == DataObject.DataType.Scheme)               
           destroyedMaterials = DataController.Instance.GetMaterialsOwnedByScheme((Scheme)dataObject);        
        


        if (dataObject.dataType == DataObject.DataType.Character)
            DataController.Instance.characterList.Remove((Character)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Material)
            DataController.Instance.materialList.Remove((Material)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Scheme)
            DataController.Instance.schemeList.Remove((Scheme)dataObject);

        foreach (Relation relation in destroyedRelations)
            DestroyRelation(relation);

        UIController.Instance.ReloadObjectCards();

        if (dataObject.dataType == DataObject.DataType.Scheme)
            if (destroyedMaterials.Count > 0)
                   foreach (Material material in destroyedMaterials)
                    RemoveDataObject(material);

       
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

    public void TransferSchemeOwnership(DataObject oldOwner, DataObject newOwner, Scheme transferredScheme)
    {
        // Destroy previous ownership relation
        DestroyRelation(DataController.Instance.GetRelationWithTheseTwoDataObjects(oldOwner, transferredScheme));

        // Destroy previous relation between new owner and scheme, if any
        if (DataController.Instance.GetRelationWithTheseTwoDataObjects(newOwner, transferredScheme) != null)
            DestroyRelation(DataController.Instance.GetRelationWithTheseTwoDataObjects(newOwner, transferredScheme));

        // Create new ownership relation
        DataController.Instance.CreateRelation(Relation.RelationType.Ownership, newOwner, transferredScheme);
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

        // Things to pass on
        List<Material> distributeMaterials = data.GetMaterialsOwnedByCharacter(character);
        List<Scheme> distributeSchemes = data.GetSchemesOwnedByCharacter(character);

        // Is CHA owner, coop or owned vs one or multiple SCH?
        List<Scheme> ownedSchemeList = data.GetSchemesOwnedByCharacter(character);
        List<Scheme> coopSchemeList = data.GetSchemesCoopedByCharacter(character);
        List<Scheme> owneeSchemeList = data.GetSchemesOwningCharacter(character);

        // Does CHA own any MAT? 
        if (distributeMaterials.Count != 0)
        {
            foreach (Material mat in distributeMaterials)
            {
                // Character owns zero SCH
                if (ownedSchemeList.Count == 0)
                {
                    // Character Coops zero SCH
                    if (coopSchemeList.Count == 0)
                    {
                        // Character is owned by zero SCH
                        if (owneeSchemeList.Count == 0)
                        {
                            RemoveDataObject(mat);
                        }
                        // Character is owned by one SCH
                        else if (owneeSchemeList.Count == 1)
                        {
                            TransferMaterialOwnership(character, owneeSchemeList[0], mat);
                        }
                        else if (owneeSchemeList.Count > 1)
                        {
                            // Character is owned by multiple SCH
                            // GIVE TO STRONGEST
                            TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(owneeSchemeList), mat);
                        }
                    }
                    // Character Coops one SCH
                    else if (coopSchemeList.Count == 1)
                    {
                        TransferMaterialOwnership(character, coopSchemeList[0], mat);
                    }
                    // Character Coops multiple SCH
                    if (coopSchemeList.Count > 1)
                    {
                        TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(coopSchemeList), mat);
                    }
                }
                // Character owns one SCH
                else if (ownedSchemeList.Count == 1)
                {
                    // give material to SCH
                    TransferMaterialOwnership(character, ownedSchemeList[0], mat);
                }
                // Character owns multiple SCH
                else if (ownedSchemeList.Count > 1)
                {
                    // give material to strongest of those SCH
                    TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(ownedSchemeList), mat);
                }
            }
        }


        // Owned Schemes: ownership goes to next-most-powerful owner, othwerise cooper, otherwise ownee
        // if none of those exist, then the scheme becomes anyway empty, so disband
        // ALTOPTION: pass control, before passing to ownees, to either other SCHs owned by CHA or SCHs that coop/own CHA
        if (distributeSchemes.Count != 0)
        {
            foreach (Scheme distSch in distributeSchemes)
            {
                List<Character> otherOwners = data.GetSchemeOwnerCharacters(distSch);
                List<Character> otherCoops = data.GetSchemeCoopCharacters(distSch);
                List<Character> otherOwnees = data.GetSchemeOwneeCharacters(distSch);
                otherOwners.Remove(character);
                if (otherOwners.Count> 1 )
                {
                    TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherOwners), distSch);
                } 
                else if  (otherOwners.Count == 1)
                {
                    TransferSchemeOwnership(character, otherOwners[0], distSch);
                }
                else if (otherOwners.Count == 0)
                {
                    if (otherCoops.Count > 1)
                    {
                        TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherCoops), distSch);
                    }
                    else if (otherCoops.Count == 1)
                    {
                        TransferSchemeOwnership(character, otherCoops[0], distSch);
                    }
                    else if (otherCoops.Count == 0)
                    {
                        if (otherOwnees.Count > 1)
                        {
                            TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherOwnees), distSch);
                        }
                        else if (otherOwnees.Count == 1)
                        {
                            TransferSchemeOwnership(character, otherOwnees[0], distSch);
                        }
                        else if (otherOwnees.Count == 0)
                        {
                            RemoveDataObject (distSch);
                        }
                    }
                }
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
