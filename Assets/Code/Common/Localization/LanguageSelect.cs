using SoftLiu.Event;
using SoftLiu.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelect : MonoBehaviour
{
    [SerializeField]
    private string m_language = null;

    private GameObject m_currentCenterObj;

    public string language
    {
        get
        {
            return m_language;
        }
    }


    void Awake()
    {

    }

    void OnEnable()
    {
        //m_centerOnChild.onCenter += OnCenter;
        //m_centerOnChild.onFinished += OnCenteringFinished;
    }

    void OnDisable()
    {
        //m_centerOnChild.onCenter -= OnCenter;
        //m_centerOnChild.onFinished -= OnCenteringFinished;
    }

    public void SetLanguage()
    {
        //Localization.Instance.language = m_language;
    }

    void OnCenter(GameObject centeredObj)
    {
        m_currentCenterObj = centeredObj;
    }

    public void BtnSelectLanguage()
    {
        m_currentCenterObj = this.gameObject;
        OnCenteringFinished();
    }

    void OnCenteringFinished()
    {
        if (this.gameObject == m_currentCenterObj && m_language != Localization.Instance.language)
        {
            // we're the selected language
            Localization.Instance.LoadLanguageFromTextAsset(m_language);
            //LanguageInitializer.Instance.SetLanguageFromSpec(m_languageSpec);
            EventManager<Events>.Instance.TriggerEvent(Events.ChangedLanguage);
        }
    }
}
