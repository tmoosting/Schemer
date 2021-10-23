using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataChanger : MonoBehaviour
{

    bool transferringMaterial = false;


    // GENERAL FUNCTIONS 
     


    public void TransferMaterialOwnership(DataObject oldOwner, DataObject newOwner, Material transferredMaterial)
    {
        transferringMaterial = true;
        // Destroy previous ownership relation
        DestroyRelation(DataController.Instance.GetRelationWithTheseTwoDataObjects(oldOwner, transferredMaterial));

        // Create new ownership relation
        DataController.Instance.CreateRelation(Relation.RelationType.Ownership, newOwner, transferredMaterial);

        transferringMaterial = false;

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
        if (dataObject.dataType == DataObject.DataType.Character)
            KillCharacter((Character)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Material)
            RemoveDataObject((Material)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Scheme)
            KillScheme((Scheme)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Relation)
            RemoveDataObject((Relation)dataObject);
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


    // NEXT TIER FUNCTIONS

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
        List<Scheme> ownerSchemeList = data.GetSchemesOwningCharacter(character);

        List<Scheme> schemesToDestroy = new List<Scheme>();
         
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
                        if (ownerSchemeList.Count == 0)
                        {
                            RemoveDataObject(mat);
                        }
                        // Character is owned by one SCH
                        else if (ownerSchemeList.Count == 1)
                        { 
                            TransferMaterialOwnership(character, ownerSchemeList[0], mat);
                        }
                        else if (ownerSchemeList.Count > 1)
                        {
                            // Character is owned by multiple SCH
                            // GIVE TO STRONGEST
                            TransferMaterialOwnership(character, data.GetMostPowerfulDataObjectFromSchemeList(ownerSchemeList), mat);
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
                if (otherOwners.Count > 1)
                {
                    TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherOwners), distSch);
                }
                else if (otherOwners.Count == 1)
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
                            schemesToDestroy.Add(distSch);
                        }
                    }
                }
            }
        }
        RemoveDataObject(character);
        foreach (Scheme scheme in schemesToDestroy)
        {
            KillDataObject(scheme);
        }
    }



    public void KillScheme(Scheme scheme)
    {
        DataController data = DataController.Instance;

        // Characters owned by scheme: nothing happens
        // Materials owned by scheme: passed to CHA owner; otherwise to SCH owner; to CHA coop; to SCH coop; to CHA ownee; to SCH ownee
        // Schemes owned by schenme: nothing happens
        // Destroy all RELs associated with scheme

        List<Material> distributeMaterials = data.GetMaterialsOwnedByScheme(scheme);
        List<Character> ownerCharactersList = data.GetCharactersOwningScheme(scheme);
        List<Scheme> ownerSchemeList = data.GetSchemesOwningScheme(scheme);
        List<Character> coopCharacterList = data.GetCharactersCoopedByScheme(scheme);
        List<Scheme> coopSchemeList = data.GetSchemesCoopedByScheme(scheme);
        List<Character> ownedCharacterList = data.GetCharactersOwnedByScheme(scheme);
        List<Scheme> ownedSchemeList = data.GetSchemesOwnedByScheme(scheme);
         
        if (distributeMaterials.Count != 0)
        {
            foreach (Material mat in distributeMaterials)
            {
                if (ownerCharactersList.Count > 1)
                {
                    TransferMaterialOwnership(scheme, data.GetMostPowerfulDataObjectFromCharacterList(ownerCharactersList), mat);
                }
                else if (ownerCharactersList.Count == 1)
                {
                    TransferMaterialOwnership(scheme, ownerCharactersList[0], mat);
                }
                else if (ownerCharactersList.Count == 0)
                {
                    if (ownerSchemeList.Count > 1)
                    {
                        TransferMaterialOwnership(scheme, data.GetMostPowerfulDataObjectFromSchemeList(ownerSchemeList), mat);
                    }
                    else if (ownerSchemeList.Count == 1)
                    {
                        TransferMaterialOwnership(scheme, ownerSchemeList[0], mat);
                    }
                    else if (ownerSchemeList.Count == 0)
                    {
                        if (coopCharacterList.Count > 1)
                        {
                            TransferMaterialOwnership(scheme, data.GetMostPowerfulDataObjectFromCharacterList(coopCharacterList), mat);
                        }
                        else if (coopCharacterList.Count == 1)
                        {
                            TransferMaterialOwnership(scheme, coopCharacterList[0], mat);
                        }
                        else if (coopCharacterList.Count == 0)
                        {
                            if (coopSchemeList.Count > 1)
                            {
                                TransferMaterialOwnership(scheme, data.GetMostPowerfulDataObjectFromSchemeList(coopSchemeList), mat);
                            }
                            else if (coopSchemeList.Count == 1)
                            {
                                TransferMaterialOwnership(scheme, coopSchemeList[0], mat);
                            }
                            else if (coopSchemeList.Count == 0)
                            {
                                if (ownedCharacterList.Count > 1)
                                {
                                    TransferMaterialOwnership(scheme, data.GetMostPowerfulDataObjectFromCharacterList(ownedCharacterList), mat);
                                }
                                else if (ownedCharacterList.Count == 1)
                                {
                                    TransferMaterialOwnership(scheme, ownedCharacterList[0], mat);
                                }
                                else if (ownedCharacterList.Count == 0)
                                {
                                    if (ownedSchemeList.Count > 1)
                                    {
                                        TransferMaterialOwnership(scheme, data.GetMostPowerfulDataObjectFromSchemeList(ownedSchemeList), mat);
                                    }
                                    else if (ownedSchemeList.Count == 1)
                                    {
                                        TransferMaterialOwnership(scheme, ownedSchemeList[0], mat);
                                    }
                                    else if (ownedSchemeList.Count == 0)
                                    {
                                        RemoveDataObject(mat);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        RemoveDataObject(scheme);
    }




    // THIRD TIER REMOVAL


    public void RemoveDataObject(DataObject dataObject)
    { 
        List<Relation> destroyedRelations = DataController.Instance.GetRelationsThatIncludeObject(dataObject);
        List<ObjectViewCard> destroyedCards = new List<ObjectViewCard>();

        // Remove Viewcards
        foreach (ObjectViewCard card in UIController.Instance.cardListRegular)
        {
            if (card != null)
            {
                if (card.containedObject != null)
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
            } 
        }
        foreach (ObjectViewCard card in UIController.Instance.cardListFocusView)
        {
            if (card != null)
            {
                if (card.containedObject != null)
                {
                    if (card.containedObject == dataObject)
                        destroyedCards.Add(card);
                    else if (card.containedObject.dataType == DataObject.DataType.Relation)
                    {
                        if (destroyedRelations.Contains((Relation)card.containedObject))
                            destroyedCards.Add(card);
                    }
                }
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

        //// check for relationless material leftover
        //List<Material> destroyedMaterials = new List<Material>();
        //if (dataObject.dataType == DataObject.DataType.Scheme)
        //    destroyedMaterials = DataController.Instance.GetMaterialsOwnedByScheme((Scheme)dataObject);



        if (dataObject.dataType == DataObject.DataType.Character)
            DataController.Instance.characterList.Remove((Character)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Material)
            DataController.Instance.materialList.Remove((Material)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Scheme)
            DataController.Instance.schemeList.Remove((Scheme)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Relation) 
            DataController.Instance.relationList.Remove((Relation)dataObject);
         

        foreach (Relation relation in destroyedRelations)
            DestroyRelation(relation);

        UIController.Instance.ReloadObjectCards();

        //if (dataObject.dataType == DataObject.DataType.Scheme)
        //    if (destroyedMaterials.Count > 0)
        //        foreach (Material material in destroyedMaterials)
        //            RemoveDataObject(material);
       

        //check for when transferring ownership before the new relation is made
        if (transferringMaterial == false)
        {
            if (DataController.Instance.sweepUnownedMaterials == true)
                SweepUnownedMaterial();
            if (DataController.Instance.sweepUnownedSchemes == true)
                SweepUnownedSchemes();
        }
      

    }

    void SweepUnownedMaterial()
    {
        List<Material> sweepedMaterial = new List<Material>();
        foreach (Material mat in DataController.Instance.materialList)        
            if (DataController.Instance.GetRelationsThatIncludeObject(mat).Count == 0)
                sweepedMaterial.Add(mat);
        foreach (Material mat in sweepedMaterial)
            RemoveDataObject(mat);
    }  
    void SweepUnownedSchemes()
    {
        List<Scheme> sweepedSchemes = new List<Scheme> ();
        foreach (Scheme sch in DataController.Instance.schemeList)        
            if (DataController.Instance.GetRelationsThatIncludeObject(sch).Count == 0)
                sweepedSchemes.Add(sch);
        foreach (Scheme sch in sweepedSchemes)
            RemoveDataObject(sch);
    }
    public void DestroyRelation(Relation relation)
    {
         RemoveDataObject(relation);
      //   DataController.Instance.relationList.Remove(relation);
    }

}
