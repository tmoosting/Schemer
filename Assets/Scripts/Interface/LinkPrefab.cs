using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPrefab : MonoBehaviour
{
    [HideInInspector]
    public DataObject linkedObject;
    [HideInInspector]
    public ObjectViewCard parentCard;

    public GameObject highlight;

    private void Start()
    {
        highlight.SetActive(false); 
    }
    private void OnMouseUp()
    {
        UIController.Instance.ClickLinkPrefab(this);
    }

    private void OnMouseOver()
    {
        highlight.SetActive(true);
        parentCard.clickable = false;
    }
    private void OnMouseExit()
    {
        highlight.SetActive(false); 
        parentCard.clickable = true;
    }
}
