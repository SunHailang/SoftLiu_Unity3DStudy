using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [HideInInspector]
    public Vector3 cameraMoveDirection = Vector3.zero;

    private void OnEnable()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cameraMoveDirection = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cameraMoveDirection = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        cameraMoveDirection = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cameraMoveDirection = Vector3.zero;
    }

    private void SetPosition(PointerEventData eventData, RectTransform rect)
    {
        //存储当前鼠标所在位置
        Vector3 rectWorldVec;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out rectWorldVec))
        {
            float dis = 200f;
            rect.position = rectWorldVec;
            float curDis = Vector3.Distance(Vector3.zero, rect.localPosition);
            if (curDis > dis)
            {
                //指定原点和方向
                Vector3 direction = rect.localPosition - Vector3.zero;
                Ray ray = new Ray(Vector3.zero, direction.normalized);
                Vector3 targetVec = ray.GetPoint(dis);
                rect.localPosition = targetVec;
            }
        }
    }
}
