using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataChanger : MonoBehaviour
{

    bool transferringMaterial = false;

    bool limitReloadCardObject = false;



    // ACTION FUNCTIONS

    public void KillDataObject(DataObject dataObject)
    { 

        if (dataObject.dataType == DataObject.DataType.Character)
            KillCharacter((Character)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Material)
            RemoveDataObject((Material)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Institution)
            KillScheme((Institution)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Relation)
            RemoveDataObject((Relation)dataObject);
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
    }
   
   

    public void GiftMaterial(DataObject receiverObject, string materialSubtype, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Material.MaterialSubtype subType = (Material.MaterialSubtype)System.Enum.Parse(typeof(Material.MaterialSubtype), materialSubtype);  
            if (subType != Material.MaterialSubtype.Nugget)
            { 
                DataController.Instance.CreateStandardMaterial(subType, receiverObject);
            }
            else if (subType == Material.MaterialSubtype.Nugget)
            {
                if (DataController.Instance.DoesObjectOwnMaterialSubtypeAlready(receiverObject, subType) == true)
                {
                    DataController.Instance.GetAnyOwnedMaterialOfSubtype(receiverObject, subType).baseAmount++;
                }
                else
                { 
                    DataController.Instance.CreateStandardMaterial(subType, receiverObject);
                }
            }
            
        }
        UIController.Instance.PlayGiftSparksAtObject(receiverObject);
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
    }
    public void GiftMaterialToInstitutionNamedCharacters(DataObject receiverInstitution, string materialSubtype, int amount)
    {
        Material.MaterialSubtype subType = (Material.MaterialSubtype)System.Enum.Parse(typeof(Material.MaterialSubtype), materialSubtype);
        UIController.Instance.PlayGiftSparksAtObject(receiverInstitution);
        foreach (Character character in DataController.Instance.GetSchemeCharacters((Institution)receiverInstitution))        
            DataController.Instance.CreateStandardMaterial(subType, character);
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
    }
    public void GiftMaterialToInstitutionUnnamedCharacters(DataObject receiverInstitution, string materialSubtype, int amount)
    {
        Material.MaterialSubtype subType = (Material.MaterialSubtype)System.Enum.Parse(typeof(Material.MaterialSubtype), materialSubtype);
        UIController.Instance.PlayGiftSparksAtObject(receiverInstitution);

        Institution ins = (Institution)receiverInstitution;
        Material createdMaterial =DataController.Instance.CreateStandardMaterial(subType, receiverInstitution);
        createdMaterial.baseAmount = (ins.genericOwnerCount + ins.genericCooperativeCount + ins.genericOwneeCount);

        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
    }

    public void ClaimDataObject(DataObject receiverObject,  DataObject claimedObject)
    {

        UIController.Instance.PlayGiftSparksAtObject(receiverObject);
        UIController.Instance.PlayGiftSparksAtObject(claimedObject);
        // what if scheme claims its only owner? 
        if (claimedObject.dataType == DataObject.DataType.Character && receiverObject.dataType == DataObject.DataType.Institution)
        {
            if (DataController.Instance.GetCharactersOwningScheme((Institution)receiverObject).Count ==1)
            {
                if (DataController.Instance.GetCharactersOwningScheme((Institution)receiverObject)[0] == (Character)claimedObject)
                {                  
                    DataObject nextOwner = DataController.Instance.GetNextSchemeOwnerCharacter((Institution)receiverObject);

                    // destroy prev relation
                    RemoveDataObject(DataController.Instance.GetRelationWithTheseTwoDataObjects(nextOwner, receiverObject));

                    // create new one
                    DataController.Instance.CreateRelation(Relation.RelationType.Ownership, nextOwner, receiverObject);              
                    
                } 
                DataController.Instance.CreateRelation(Relation.RelationType.Ownership, receiverObject, claimedObject);
            }
            else            
                DataController.Instance.CreateRelation(Relation.RelationType.Ownership, receiverObject, claimedObject); 
        }
        else if (claimedObject.dataType == DataObject.DataType.Character)
        { 
            DataController.Instance.CreateRelation(Relation.RelationType.Ownership, receiverObject, claimedObject); 
        }
        else if (claimedObject.dataType == DataObject.DataType.Material)
        {
            Material claimedMaterial = (Material)claimedObject;
            DataObject oldOwner = DataController.Instance.GetOwnerOfMaterial(claimedMaterial);
            TransferMaterialOwnership(oldOwner, receiverObject, claimedMaterial);
        }
        else if (claimedObject.dataType == DataObject.DataType.Institution)
        {
            Institution claimedScheme = (Institution)claimedObject;
            if (receiverObject.dataType == DataObject.DataType.Character)
            {
                DataObject oldOwner = DataController.Instance.GetCharacterOwnerOfScheme(claimedScheme);
                TransferSchemeOwnership(oldOwner, receiverObject, claimedScheme, true);
            }
            else if (receiverObject.dataType == DataObject.DataType.Institution)
            {
                DataObject oldOwner = DataController.Instance.GetSchemeOwnerOfScheme(claimedScheme);
                if (oldOwner != null)
                     TransferSchemeOwnership(oldOwner, receiverObject, claimedScheme, true);
                else
                {
                    DataController.Instance.CreateRelation(Relation.RelationType.Ownership, receiverObject, claimedScheme); 
                }
            }
        }
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();

    }

    public void CreateCooperation(DataObject primaryObject, DataObject secondaryObject)
    {
        DataController data = DataController.Instance;
        // if primary owns secondary, create new ownership IF primary is character
        if (data.IsFirstObjectOwnerOfSecondObject(primaryObject, secondaryObject) == true)
        {
            if  (primaryObject.dataType == DataObject.DataType.Character)
            {
                // secobj is sch because cha-cha not enabled in action window
                DataObject nextOwner = DataController.Instance.GetNextSchemeOwnerCharacter((Institution)secondaryObject);
                // destroy new owner's prev relation
                RemoveDataObject(DataController.Instance.GetRelationWithTheseTwoDataObjects(nextOwner, secondaryObject));
                // create new owner relation
                DataController.Instance.CreateRelation(Relation.RelationType.Ownership, nextOwner, secondaryObject);
            }            
        }

        // else if secondary owns primary
        else if (data.IsFirstObjectOwnerOfSecondObject(secondaryObject, primaryObject) == true)
        {
            if (secondaryObject.dataType == DataObject.DataType.Character)
            {
                // secobj is sch because cha-cha not enabled in action window
                DataObject nextOwner = DataController.Instance.GetNextSchemeOwnerCharacter((Institution)primaryObject);
                // destroy new owner's prev relation
                RemoveDataObject(DataController.Instance.GetRelationWithTheseTwoDataObjects(nextOwner, primaryObject));
                // create new owner relation
                DataController.Instance.CreateRelation(Relation.RelationType.Ownership, nextOwner, primaryObject);
            }
        }
        // destroy prev relation if one already exists
        if (data.DoesRelationExistBetweenObjects(primaryObject, secondaryObject) == true)
            RemoveDataObject(data.GetRelationWithTheseTwoDataObjects(primaryObject, secondaryObject));

        // create new one
        DataController.Instance.CreateRelation(Relation.RelationType.Cooperative, primaryObject, secondaryObject);
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
     
    }

    public void BreakCooperationNeutral(DataObject primaryObject, DataObject secondaryObject)
    {
        // destroy relevant relation
        Relation relation = DataController.Instance.GetRelationWithTheseTwoDataObjects(primaryObject, secondaryObject);
        if (relation.relationType != Relation.RelationType.Cooperative)
            Debug.LogWarning("Why do " + primaryObject.ID + " and  " + secondaryObject.ID + " have a non-coop REL?: " + relation.ID);

        RemoveDataObject(relation);
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
    
    }


    // DEPRECATED FOR NOW - USE MANUAL CLAIM-MATERIAL
    public void BreakCooperationHostile(DataObject primaryObject, DataObject secondaryObject)
    {
        // destroy relevant relation
        // some way to grab power / material for primaryObject

        Relation relation = DataController.Instance.GetRelationWithTheseTwoDataObjects(primaryObject, secondaryObject);
        if (relation.relationType != Relation.RelationType.Cooperative)
            Debug.LogWarning("Why do " + primaryObject.ID + " and  " + secondaryObject.ID + " have a non-coop REL?: " + relation.ID);

        StealCooperativePower(primaryObject, secondaryObject);

        RemoveDataObject(relation);
        if (limitReloadCardObject == false)
            DataController.Instance.powerCalculator.CalculatePowers();
       
    }

    void StealCooperativePower (DataObject stealer, DataObject victim)
    {
        Debug.Log(stealer.ID + " steals from " + victim.ID);
    } 
 


    // PURPOSE FUNCTIONS 

    public void TransferMaterialOwnership(DataObject oldOwner, DataObject newOwner, Material transferredMaterial)
    {
        transferringMaterial = true;
        // Destroy previous ownership relation
        RemoveDataObject(DataController.Instance.GetRelationWithTheseTwoDataObjects(oldOwner, transferredMaterial));

        // Create new ownership relation
        DataController.Instance.CreateRelation(Relation.RelationType.Ownership, newOwner, transferredMaterial);

        transferringMaterial = false;
        if (limitReloadCardObject == false)
            UIController.Instance.ReloadObjectCards();
    }
  

    public void TransferSchemeOwnership(DataObject oldOwner, DataObject newOwner, Institution transferredScheme, bool demoteOldOwner)
    {
        // Destroy previous ownership relation
        RemoveDataObject(DataController.Instance.GetRelationWithTheseTwoDataObjects(oldOwner, transferredScheme));

        // Destroy previous relation between new owner and scheme, if any
        if (DataController.Instance.GetRelationWithTheseTwoDataObjects(newOwner, transferredScheme) != null)
            RemoveDataObject(DataController.Instance.GetRelationWithTheseTwoDataObjects(newOwner, transferredScheme));

        // Create new ownership relation
        DataController.Instance.CreateRelation(Relation.RelationType.Ownership, newOwner, transferredScheme);

        if (demoteOldOwner == true)        
            DataController.Instance.CreateRelation(Relation.RelationType.Cooperative, oldOwner, transferredScheme);

        if (limitReloadCardObject == false)
            UIController.Instance.ReloadObjectCards();
    }


    // NEXT TIER FUNCTIONS

    public void KillCharacter(Character character)
    {
        DataController data = DataController.Instance;

        // Material: if CHA is owner or coop with any schemes, choose the most powerful, transfers to that Institution; 
        // otherwise, if owner is ownee of a scheme, go to that scheme; 
        // otherwise, material is destroyed! 

        // Things to pass on
        List<Material> distributeMaterials = data.GetMaterialsOwnedByCharacter(character);
        List<Institution> distributeSchemes = data.GetSchemesOwnedByCharacter(character);

        // Is CHA owner, coop or owned vs one or multiple SCH?
        List<Institution> ownedSchemeList = data.GetSchemesOwnedByCharacter(character);
        List<Institution> coopSchemeList = data.GetSchemesCoopedByCharacter(character);
        List<Institution> ownerSchemeList = data.GetSchemesOwningCharacter(character);

        List<Institution> schemesToDestroy = new List<Institution>();
         
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
            foreach (Institution distSch in distributeSchemes)
            {
                List<Character> otherOwners = data.GetSchemeOwnerCharacters(distSch);
                List<Character> otherCoops = data.GetSchemeCoopCharacters(distSch);
                List<Character> otherOwnees = data.GetSchemeOwneeCharacters(distSch);
                otherOwners.Remove(character);
                if (otherOwners.Count > 1)
                {
                    TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherOwners), distSch, false);
                }
                else if (otherOwners.Count == 1)
                {
                    TransferSchemeOwnership(character, otherOwners[0], distSch, false);
                }
                else if (otherOwners.Count == 0)
                {
                    if (otherCoops.Count > 1)
                    {
                        TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherCoops), distSch, false);
                    }
                    else if (otherCoops.Count == 1)
                    { 
                        TransferSchemeOwnership(character, otherCoops[0], distSch, false);
                    }
                    else if (otherCoops.Count == 0)
                    {
                        if (otherOwnees.Count > 1)
                        {
                            TransferSchemeOwnership(character, data.GetMostPowerfulDataObjectFromCharacterList(otherOwnees), distSch, false);
                        }
                        else if (otherOwnees.Count == 1)
                        {
                            TransferSchemeOwnership(character, otherOwnees[0], distSch, false);
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
        foreach (Institution scheme in schemesToDestroy)
        {
            KillDataObject(scheme);
        }
    }



    public void KillScheme(Institution scheme)
    {
        DataController data = DataController.Instance;

        // Characters owned by scheme: nothing happens
        // Materials owned by scheme: passed to CHA owner; otherwise to SCH owner; to CHA coop; to SCH coop; to CHA ownee; to SCH ownee
        // Schemes owned by schenme: nothing happens
        // Destroy all RELs associated with scheme

        List<Material> distributeMaterials = data.GetMaterialsOwnedByScheme(scheme);
        List<Character> ownerCharactersList = data.GetCharactersOwningScheme(scheme);
        List<Institution> ownerSchemeList = data.GetSchemesOwningScheme(scheme);
        List<Character> coopCharacterList = data.GetCharactersCoopedByScheme(scheme);
        List<Institution> coopSchemeList = data.GetSchemesCoopedByScheme(scheme);
        List<Character> ownedCharacterList = data.GetCharactersOwnedByScheme(scheme);
        List<Institution> ownedSchemeList = data.GetSchemesOwnedByScheme(scheme);
         
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
        //if (dataObject.dataType == DataObject.DataType.Institution)
        //    destroyedMaterials = DataController.Instance.GetMaterialsOwnedByScheme((Institution)dataObject);



        if (dataObject.dataType == DataObject.DataType.Character)
            DataController.Instance.characterList.Remove((Character)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Material)
            DataController.Instance.materialList.Remove((Material)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Institution)
            DataController.Instance.institutionList.Remove((Institution)dataObject);
        else if (dataObject.dataType == DataObject.DataType.Relation) 
            DataController.Instance.relationList.Remove((Relation)dataObject);
         

        foreach (Relation relation in destroyedRelations)
            RemoveDataObject(relation);

        if (limitReloadCardObject == false)
            UIController.Instance.ReloadObjectCards();

        //if (dataObject.dataType == DataObject.DataType.Institution)
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
        List<Institution> sweepedSchemes = new List<Institution> ();
        foreach (Institution sch in DataController.Instance.institutionList)        
            if (DataController.Instance.GetRelationsThatIncludeObject(sch).Count == 0)
                sweepedSchemes.Add(sch);
        foreach (Institution sch in sweepedSchemes)
            RemoveDataObject(sch);
    }
   

}
