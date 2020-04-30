using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SoftLiu.Utilities;
using SoftLiu.Servers;
using System;
using TMPro;
using SoftLiu.Localization;
using SoftLiu.SceneManagers;


public class Splash : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_text = null;

    private void Awake()
    {

    }

    // Use this for initialization
    private void Start()
    {
        SaveFacade.Instance.Init();
    }
    float m_frame = 0;
    private void Update()
    {
        RequestsManager.Instance.OnUpdate();
    }

    public void OnClick()
    {
        RequestsManager.Instance.GetServerTime((response) =>
        {
            if (string.IsNullOrEmpty(response.error))
            {
                Debug.Log(response.response.unixTimestamp);
                Debug.Log(Time.unscaledTime);

                //SceneManager.LoadScene("Common3D", LoadSceneMode.Additive);
                bool result = SceneManager.Instance.LoadSceneAsync("Common3D", (data) =>
                {
                    Debug.Log(data.Name);
                    if (data.SceneAsync != null)
                    {
                        //data.SceneAsync.allowSceneActivation = true;
                    }
                    else
                    {
                        Debug.LogError("LoadSceneAsync Error: " + data.Name);
                    }

                }, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogError("GetServerTime Error: " + response.error);
            }
        });
    }
}
