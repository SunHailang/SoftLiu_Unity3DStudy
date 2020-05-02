using SoftLiu.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MenuToIngame : MonoBehaviour
{
    [SerializeField]
    private GameObject m_uiRoot = null;
    [SerializeField]
    private RectTransform m_sliderBGRect = null;
    [SerializeField]
    private Image m_sliderForward = null;


    private void OnEnable()
    {
        // Register Event
        EventManager<Events>.Instance.RegisterEvent(Events.HasNotchAndSize, OnHasNotchAndSize);
        m_sliderForward.fillAmount = 0;

    }

    private void Start()
    {
        StartCoroutine(LoadStartGame());
    }

    private IEnumerator LoadStartGame()
    {
        yield return null;
        AsyncOperation async = SceneManager.LoadSceneAsync("Common3D", LoadSceneMode.Additive);
        while (!async.isDone)
        {
            m_sliderForward.fillAmount = async.progress;
            if (async.progress == 0.9f && async.allowSceneActivation)
            {
                async.allowSceneActivation = false;
                break;
            }
            yield return null;
        }
        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }
        m_sliderForward.fillAmount = 1f;
        SceneManager.LoadScene("IngameHDU", LoadSceneMode.Additive);

        SceneManager.UnloadSceneAsync("MenuToIngame");
    }

    private void Update()
    {

    }

    private void OnDisable()
    {
        // Deregister Event
        EventManager<Events>.Instance.DeregisterEvent(Events.HasNotchAndSize, OnHasNotchAndSize);
    }

    private void OnHasNotchAndSize(Events eventType, object[] mParams)
    {
        if (mParams != null && mParams.Length > 0)
        {
            int[] notchs = mParams[0] as int[];
            if (notchs != null)
            {
                m_sliderBGRect.sizeDelta += new Vector2(notchs[0], 0);
            }
        }
    }
}
