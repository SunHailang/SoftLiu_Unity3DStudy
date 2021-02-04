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
using SoftLiu.Save;
using SoftLiu.Authentication;
using UnityEngine.EventSystems;

public class Splash : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI m_text = null;
    [SerializeField]
    private Material m_bgMaterial = null;

    private void Awake()
    {

    }

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(InitGameData());
    }

    private IEnumerator InitGameData()
    {
        // load game data
        GameDataManager.Instance.LoadGameData();

        yield return null;

        SaveFacade.Instance.Init();

        SaveGameManager.Instance.Load(Authenticator.Instance.User);

        yield return null;
        App.Instance.Init();

        
    }
   
    private void Update()
    {
        // 获取鼠标位置
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Location Pos:" + eventData.position);
        Debug.Log("World Pos:" + eventData.pointerCurrentRaycast.worldPosition);
        Debug.Log("Screen Pos:" + eventData.pointerCurrentRaycast.screenPosition);
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

    public void BtnStart_OnClick()
    {
        StartCoroutine(StartLogin());
    }

    private IEnumerator StartLogin()
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LoginLoader", UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return async;
    }
}
