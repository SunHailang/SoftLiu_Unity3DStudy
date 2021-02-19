using SoftLiu.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SoftLiu.SceneManagers;

public class MenuToIngame : MonoBehaviour
{
    [SerializeField]
    private GameObject m_uiRoot = null;
    [SerializeField]
    private RectTransform m_sliderBGRect = null;
    [SerializeField]
    private Image m_sliderForward = null;

    private void Awake()
    {

    }

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

        SceneStack scene = SceneManager.Instance.CurrentSceneStack;
        SceneAsyncData scenes = SceneManager.Instance.LoadSceneAsync(scene);
        float process = 0.0f;
        bool isDone = false;
        while (!isDone)
        {
            process = Mathf.Min(0.9f, scenes.ProcessIfFinished());
            m_sliderForward.fillAmount = process;
            if (process == 0.9f) isDone = true;
            yield return null;
        }
        while (m_sliderForward.fillAmount < 1.0f)
        {
            yield return new WaitForSeconds(0.02f);
            m_sliderForward.fillAmount += 0.0005f;
        }
        m_sliderForward.fillAmount = 1.0f;
        if (scene.LoadMode == UnityEngine.SceneManagement.LoadSceneMode.Additive)
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("MenuToIngame");
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
