using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAdaptation : MonoBehaviour
{
    private RectTransform m_rectTransform = null;

    [Range(0, 200)]
    public float m_width = 10;

    [Range(0, 100)]
    public float m_height = 0;

    private void Awake()
    {
        m_rectTransform = gameObject.GetComponent<RectTransform>();
        SoftLiu.Assert.Fatal(m_rectTransform != null, "ScreenAdaptation m_rectTransform is null.");

        //Screen.height
        //Screen.width
        SoftLiu.Debug.Log(string.Format("Screen width: {0}  height: {1}", Screen.width, Screen.height));

        SoftLiu.Debug.LogError(3.54781.ToString("F"));
        SoftLiu.Debug.LogError(2.00781.ToString("F"));
    }


    private void Update()
    {
        //m_rectTransform.anchoredPosition = new Vector2(m_width, m_height);
        m_rectTransform.sizeDelta = new Vector2(-m_width, -m_height);
    }

}
