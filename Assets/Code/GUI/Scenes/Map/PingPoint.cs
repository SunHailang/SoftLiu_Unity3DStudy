using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPoint : MonoBehaviour
{
    public float m_scale = 5f;

    private SpriteRenderer m_spriteRender = null;

    private bool m_init = false;

    private float m_runningTime = 0;
    private float m_existTime = 3f;

    private Color m_color = Color.white;

    private void OnEnable()
    {
        m_spriteRender = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * m_scale;
    }

    public void SetColor(Color col)
    {
        m_color = col;
        m_spriteRender.color = col;
        m_init = true;
    }

    private void Update()
    {
        if (m_init)
        {
            m_runningTime += Time.deltaTime;
            m_spriteRender.color = Color.Lerp(m_spriteRender.color, new Color(m_color.r, m_color.g, m_color.b, 0), Time.deltaTime);
            if (m_runningTime >= m_existTime)
            {
                m_spriteRender.color = new Color(m_color.r, m_color.g, m_color.b, 0);
                m_init = false;
                Destroy(gameObject);
            }
        }
    }
}
