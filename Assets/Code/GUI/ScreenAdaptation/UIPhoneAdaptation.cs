using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu;
using SoftLiu.Plugins.Native;
using SoftLiu.Event;

public class UIPhoneAdaptation : MonoBehaviour
{
    private RectTransform m_rectTransform = null;

    [Range(0, 200)]
    public float m_width = 10;

    [Range(-2, 100)]
    public float m_height = -2;

    private bool m_init = false;

    private void Awake()
    {
        m_rectTransform = gameObject.GetComponent<RectTransform>();
        Assert.Fatal(m_rectTransform != null, "ScreenAdaptation m_rectTransform is null.");

        //Screen.height
        //Screen.width
        //Debug.Log(string.Format("Screen width: {0}  height: {1}", Screen.width, Screen.height));
    }


    private void Update()
    {
        if (m_init) return;
        m_init = true;
        if (NativeBinding.Instance.HasNotch())
        {
            int[] notchs = NativeBinding.Instance.GetNotchSize();
            m_width = (float)notchs[0];
            //m_rectTransform.anchoredPosition = new Vector2(m_width, m_height);
            m_rectTransform.sizeDelta = new Vector2(-m_width, -m_height);
            EventManager<Events>.Instance.TriggerEvent(Events.HasNotchAndSize, notchs);
        }
    }


}
