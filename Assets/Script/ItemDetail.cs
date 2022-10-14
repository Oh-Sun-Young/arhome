using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDetail : MonoBehaviour
{
    public void GoDetail()
    {
        int id = transform.parent.GetComponent<ItemInfo>().itemId;
        GameManager.instance.idDetailView = id;
        UIManager.instance.PopupContentActive("detail");
        UIManager.instance.DetailActive(id);
    }
}
