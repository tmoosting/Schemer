using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPrefab : MonoBehaviour
{
    [HideInInspector]
    public DataObject linkedObject;

    private void OnMouseUp()
    {
        UIController.Instance.ClickLinkPrefab(this);
    }
}
