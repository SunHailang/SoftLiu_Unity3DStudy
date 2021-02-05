using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FrontEnd : MonoBehaviour
{
    [SerializeField]
    private GameObject m_content = null;
    [SerializeField]
    private Toggle[] m_toggleList = null;

    private Toggle m_currentToggle = null;


    private void Start()
    {
        foreach (Toggle item in m_toggleList)
        {
            item.onValueChanged.AddListener(delegate
            {
                if (item.isOn)
                    m_currentToggle = item;
            });
        }
    }

    public void BtnPlay_OnClick()
    {
        Debug.Log("FrontEnd Play Button OnClick.");
        StartCoroutine(LoadStartGame());
    }

    private IEnumerator LoadStartGame()
    {
        yield return null;

        AsyncOperation async = SceneManager.LoadSceneAsync("MenuToIngame", LoadSceneMode.Single);

    }

    public void ToggleContent_OnChanged(bool isOn)
    {
        if (isOn)
        {
            m_currentToggle = m_toggleList.Where((toggle) => { return toggle.isOn; }).FirstOrDefault();
        }
    }

    public void BtnSetting_OnClick()
    {

    }
}
