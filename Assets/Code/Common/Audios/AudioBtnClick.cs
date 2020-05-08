using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioBtnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private string m_audioName = "click";
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayEffectsSound(m_audioName, false);
    }

}
