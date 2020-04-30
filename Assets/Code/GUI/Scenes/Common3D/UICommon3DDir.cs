using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICommon3DDir : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player = null;
    [SerializeField]
    private HorizontalLayoutGroup m_dir = null;
    [SerializeField]
    private RectTransform m_imageRect = null;

    private void OnEnable()
    {
        RectTransform obj = Instantiate(m_imageRect, m_dir.transform);
        obj.gameObject.SetActive(true);
        RectTransform dirRect = m_dir.GetComponent<RectTransform>();
        int count = dirRect.childCount;
        float totalWidth = m_imageRect.sizeDelta.x * count + m_dir.spacing * (count - 1);
        dirRect.sizeDelta = new Vector2(totalWidth, m_imageRect.sizeDelta.y);
    }

    private void Update()
    {
        
    }
}
