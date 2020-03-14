using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu;

public class UIPhoneAdaptation : MonoBehaviour
{
    private RectTransform m_rectTransform = null;

    [Range(0, 200)]
    public float m_width = 10;

    [Range(-2, 100)]
    public float m_height = -2;

    private void Awake()
    {
        m_rectTransform = gameObject.GetComponent<RectTransform>();
        Assert.Fatal(m_rectTransform != null, "ScreenAdaptation m_rectTransform is null.");

        //Screen.height
        //Screen.width
        Debug.Log(string.Format("Screen width: {0}  height: {1}", Screen.width, Screen.height));
    }


    private void Update()
    {
        //m_rectTransform.anchoredPosition = new Vector2(m_width, m_height);
        m_rectTransform.sizeDelta = new Vector2(-m_width, -m_height);
    }

}
