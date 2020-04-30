using SoftLiu.Event;
using SoftLiu.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIFontReplacer : MonoBehaviour
{
    TextMeshProUGUI m_text = null;
    TMP_FontAsset m_font = null;

    private string m_language = "null";

    private void Awake()
    {
        //Register
        EventManager<Events>.Instance.RegisterEvent(Events.ChangedLanguage, OnChangedLanguage);

        m_text = GetComponent<TextMeshProUGUI>();
        m_font = Resources.Load<TMP_FontAsset>("Fonts/FNT_Normal_zh-cn");

        //m_text.font = m_font;
    }

    private void OnEnable()
    {
        //Register
        EventManager<Events>.Instance.RegisterEvent(Events.ChangedLanguage, OnChangedLanguage);
        //m_language = Localization.Instance.language;

        OnFontChanged();
    }

    private void OnChangedLanguage(Events eventType, object[] mParams)
    {
        OnFontChanged();
    }

    private void OnFontChanged()
    {
        if (m_language != Localization.Instance.language)
        {
            m_language = Localization.Instance.language;
            if (m_language.ToLower().Equals("chinese"))
            {
                m_font = Resources.Load<TMP_FontAsset>("Fonts/FNT_Normal_zh-cn");
            }
            else if (m_language.ToLower().Equals("english"))
            {
                m_font = Resources.Load<TMP_FontAsset>("Fonts/FNT_Normal_en");
            }
            m_text.font = m_font;
        }
    }

    private void OnDisable()
    {
        //Deregister
        EventManager<Events>.Instance.DeregisterEvent(Events.ChangedLanguage, OnChangedLanguage);
    }
}
