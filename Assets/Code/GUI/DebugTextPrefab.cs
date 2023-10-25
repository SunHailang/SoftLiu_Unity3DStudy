using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextPrefab : MonoBehaviour
{
    private Text m_text = null;

    private float m_moveTime = 3f;

    private float m_scaleSpeed = 5f;

    private Vector3 m_scale = Vector3.one;

    private Vector3 m_point = Vector3.zero;

    private float m_moveDis = 10f;

    private void Awake()
    {
        m_text = GetComponent<Text>();
        SoftLiu.Assert.Fatal(m_text != null, "DebugTextPrefab m_text is null.");
        m_point = new Vector3(transform.localPosition.x, transform.localPosition.y + m_moveDis, transform.localPosition.z);
    }

    public void Configuer(string text, Vector3 scale, Color color)
    {
        m_scale = scale;
        m_text.color = color;
        //m_text.SetText(text);
    }

    private void Update()
    {
        m_scale = Vector3.Lerp(m_scale, Vector3.zero, Time.unscaledDeltaTime);
        transform.localScale = m_scale;
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_point, Time.unscaledDeltaTime * m_scaleSpeed);
        if (Vector3.Distance(transform.localPosition, m_point) <= 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
