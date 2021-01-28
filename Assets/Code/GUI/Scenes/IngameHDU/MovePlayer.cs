using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePlayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform m_moveForward = null;

    [HideInInspector]
    public Vector3 playerMoveDirection = Vector3.zero;


    private void OnEnable()
    {
        m_moveForward.anchoredPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPosition(eventData, m_moveForward);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_moveForward.localPosition = Vector3.zero;

        playerMoveDirection = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetPosition(eventData, m_moveForward);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(eventData, m_moveForward);

        playerMoveDirection = m_moveForward.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_moveForward.localPosition = Vector3.zero;

        playerMoveDirection = Vector3.zero;
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
