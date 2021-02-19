using SoftLiu.SceneManagers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FrontEnd : MonoBehaviour
{
    [SerializeField]
    private GameObject m_content = null;
    [SerializeField]
    private Toggle[] m_toggleList = null;

    private Toggle m_currentToggle = null;


    private SceneStack m_currentScene = null;

    private void Awake()
    {
        foreach (Toggle item in m_toggleList)
        {
            item.onValueChanged.AddListener(delegate
            {
                if (item.isOn)
                {
                    m_currentToggle = item;
                }
            });
        }
    }

    private void Start()
    {
        m_toggleList[0].isOn = true;
        m_currentToggle = m_toggleList[0];
    }

    public void BtnPlay_OnClick()
    {
        Debug.Log("FrontEnd Play Button OnClick.");
        switch (m_currentToggle.gameObject.name)
        {
            case "SquaresGame":
                m_currentScene = new SceneStack("SquaresGame",
                        UnityEngine.SceneManagement.LoadSceneMode.Additive,
                        (data) =>
                        {
                            Debug.Log("SquaresGame Complete.");
                        });
                break;
            case "IngameHDU":
                m_currentScene = new SceneStack("IngameHDU",
                        UnityEngine.SceneManagement.LoadSceneMode.Additive,
                        (data) =>
                        {
                            Debug.Log("IngameHDU Complete.");
                        }, new string[] { "Common3D" });
                break;
            default:
                break;
        }


        StartCoroutine(LoadStartGame());
    }

    private IEnumerator LoadStartGame()
    {
        yield return null;
        if (m_currentScene == null) yield break;
        SceneManager.Instance.GoToInGame(m_currentScene);
    }

    public void BtnSetting_OnClick()
    {

    }
}
