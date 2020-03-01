using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{


    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop");
        // Drag GameObject
        GameObject pointerObj = eventData.pointerDrag;
        if (pointerObj != null)
        {
            pointerObj.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        }
    }


}
