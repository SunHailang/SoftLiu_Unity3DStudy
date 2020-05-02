using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIAdaptation : MonoBehaviour
{
    public enum UIAdaptationType
    {
        OnEnableType,
        UpdateType,
    }
    private RectTransform m_rectTransform = null;

    [Range(0, 200)]
    public float m_width = 0;
    [Range(-2, 100)]
    public float m_height = 0;

    public UIAdaptationType m_type = UIAdaptationType.UpdateType;

    private void Awake()
    {
        m_rectTransform = gameObject.GetComponent<RectTransform>();
        SoftLiu.Assert.Fatal(m_rectTransform != null, "ScreenAdaptation m_rectTransform is null.");
        //Screen.height
        //Screen.width
        //Debug.Log(string.Format("Screen width: {0}  height: {1}", Screen.width, Screen.height));
    }

    private void OnEnable()
    {
        m_rectTransform.sizeDelta = new Vector2(-m_width, -m_height);
    }

    private void Update()
    {
        if (m_type == UIAdaptationType.UpdateType)
            m_rectTransform.sizeDelta = new Vector2(-m_width, -m_height);
    }

}
