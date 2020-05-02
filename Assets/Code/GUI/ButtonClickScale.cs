using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClickScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject m_targetObject = null;

    [SerializeField]
    private Vector3 m_toScale = new Vector3(1.1f, 1.1f, 1.1f);

    private Vector3 m_previousScale = Vector3.one;
    private Button m_currentButton = null;

    private void Awake()
    {
        if (m_targetObject == null)
            m_targetObject = this.gameObject;

        m_previousScale = m_targetObject.transform.localScale;
        m_currentButton = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!m_currentButton.interactable) return;
        m_targetObject.transform.localScale = m_toScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_targetObject.transform.localScale = m_previousScale;
    }



}
