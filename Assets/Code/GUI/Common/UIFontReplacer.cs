using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIFontReplacer : MonoBehaviour
{

    TextMeshProUGUI m_text = null;
    TMP_FontAsset m_font = null;

    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        m_font = Resources.Load<TMP_FontAsset>("Fonts/FNT_Normal_zh-cn");

        //m_text.font = m_font;
    }
    private void OnEnable()
    {
        if (m_text.font != m_font)
        {
            OnFontChanged(m_font);
        }
    }

    private void OnFontChanged(TMP_FontAsset newFont)
    {
        m_text.font = newFont;
    }

    private void OnDisable()
    {

    }
}
