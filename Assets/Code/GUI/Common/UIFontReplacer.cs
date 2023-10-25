using SoftLiu.Event;
using SoftLiu.Localization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIFontReplacer : MonoBehaviour
{
    private Text m_text = null;

    private string m_language = "null";

    private void Awake()
    {
        //Register
        EventManager<Events>.Instance.RegisterEvent(Events.ChangedLanguage, OnChangedLanguage);

        m_text = GetComponent<Text>();

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
                
            }
            else if (m_language.ToLower().Equals("english"))
            {
                
            }
            // m_text.font = m_font;
        }
    }

    private void OnDisable()
    {
        //Deregister
        EventManager<Events>.Instance.DeregisterEvent(Events.ChangedLanguage, OnChangedLanguage);
    }
}
