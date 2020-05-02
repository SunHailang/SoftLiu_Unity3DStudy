using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using SoftLiu.Utils;
using UnityEngine.UI;
using SoftLiu.Utilities;
using SoftLiu.Plugins.Native;
using SoftLiu.SceneManagers;
using SoftLiu.Authentication;

public class LoginLoader : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_loginTransform = null;
    [SerializeField]
    private TMP_InputField m_inputFieldUserName = null;
    [SerializeField]
    private TMP_InputField m_inputFieldUserPassword = null;
    [SerializeField]
    private GameObject m_btnLoginSuccess = null;
    [SerializeField]
    private Toggle m_toggleTourist = null;

    [SerializeField]
    private GameObject m_imageSpinner = null;

    public float m_loginRotate = 7f;

    private CanvasGroup m_loginCanvas = null;

    private bool m_logining = false;

    private void OnEnable()
    {

    }

    private void Start()
    {
        m_btnLoginSuccess.SetActive(false);
        m_loginCanvas = m_loginTransform.GetComponent<CanvasGroup>();
        m_inputFieldUserName.text = "softliu";
        m_inputFieldUserPassword.text = "123456";

        AudioController.Instance.PlayBGSound("bgm");

        m_toggleTourist.isOn = SaveFacade.Instance.userSaveSystem.userTouristMode;
    }

    private bool m_animationFinished = true;

    private IEnumerator StartLoginAnim(bool show, Vector3 start, Vector3 end, System.Action callback = null)
    {
        if (!m_animationFinished)
        {
            yield break;
        }
        m_animationFinished = false;
        bool rotateEnd = false;
        m_loginTransform.localEulerAngles = start;
        float dis = Vector3.Distance(start, end);
        m_loginCanvas.alpha = show ? 0 : 1;
        while (!rotateEnd)
        {
            yield return null;
            Vector3 loginAngle = m_loginTransform.localEulerAngles;
            m_loginTransform.localEulerAngles = Vector3.Lerp(loginAngle, end, Time.deltaTime * m_loginRotate);
            float curDis = Vector3.Distance(m_loginTransform.localEulerAngles, end);
            m_loginCanvas.alpha = show ? (1 - (curDis / dis)) : (curDis / dis);
            if (curDis <= 0.1f)
            {
                m_loginCanvas.alpha = show ? 1 : 0;
                m_loginTransform.localEulerAngles = end;
                rotateEnd = true;
            }
        }
        m_animationFinished = true;
        Debug.Log("Login Rotate Animation Finished.");
        if (callback != null) callback();
    }

    /// <summary>
    /// 请求登录、注销、刷新，状态
    /// </summary>
    /// <param name="name">用户名</param>
    /// <param name="psd">密码</param>
    /// <param name="state">状态：0：登录， -1：注销， 1：刷新(呼吸包)</param>
    /// <returns></returns>
    private IEnumerator RequestLogin(string name, string psd, int state, System.Action<string, string, long> callback)
    {
        yield return null;
        string m_serverURL = "http://localhost:8080/Login";
        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("Content-Type", "application/json;charset=utf-8");
        Dictionary<string, object> formDic = new Dictionary<string, object>();
        formDic.Add("username", name);
        formDic.Add("password", psd);
        formDic.Add("state", state);
        string json = MiniJSON.Serialize(formDic);
        Debug.Log("FormDic Json: " + json);
        byte[] postData = Encoding.UTF8.GetBytes(json);
        using (UnityWebRequest request = new UnityWebRequest(m_serverURL, UnityWebRequest.kHttpVerbPOST))
        {
            foreach (KeyValuePair<string, object> item in headers)
            {
                request.SetRequestHeader(item.Key, item.Value.ToString());
            }
            request.timeout = 15;
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SendWebRequest();
            while (!request.isDone)
            {
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.Log("Request Post Error: " + request.error);
                }
            }
            string down = request.downloadHandler.text;
            Debug.Log("PostRequest Download: " + down);
            callback(request.error, down, request.responseCode);
        }
    }

    private IEnumerator LoadBootLoader()
    {
        AsyncOperation asyncO = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BootLoader", UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return asyncO.isDone;

    }

    public void BtnSure_OnClick()
    {
        //Authenticator.Instance.User = new User();
        SaveFacade.Instance.userSaveSystem.userTouristMode = m_toggleTourist.isOn;
        SaveFacade.Instance.Save("UserSaveSystem");
        StartCoroutine(LoadBootLoader());
        return;

        m_imageSpinner.SetActive(false);
        StartCoroutine(StartLoginAnim(true, Vector3.zero, new Vector3(0, 90, 0), () =>
        {
            // server request login
#if !UNITY_EDITOR
            float x = 104;
            float widthX = Screen.width - x;
            float heigthY = Screen.height - ((x / Screen.width) * Screen.height);
            NativeBinding.Instance.ToggleSpinner(true, widthX, heigthY);
#else
            m_imageSpinner.SetActive(true);
#endif
            StartCoroutine(RequestLogin(m_inputFieldUserName.text, m_inputFieldUserPassword.text, 0, (error, response, code) =>
            {
                bool result = false;
                if (string.IsNullOrEmpty(error))
                {
                    Debug.Log("Json : " + response);
                    Dictionary<string, object> responseDic = MiniJSON.Deserialize(response) as Dictionary<string, object>;
                    if (responseDic != null)
                    {
                        try
                        {
                            int state = Convert.ToInt32(responseDic["state"]);

                            result = true;
                            switch (state)
                            {
                                case -1:
                                    // logout success
                                    result = false;
                                    break;
                                case 0:
                                    // login success , TODO
                                    m_btnLoginSuccess.SetActive(true);
                                    break;
                                case 1:
                                    // breath success
                                    break;
                                default:
                                    result = false;
                                    break;
                            }

                        }
                        catch (System.Exception msg)
                        {
                            Debug.LogError("Response Dictionary Error: " + msg.Message);
                        }
                    }
                }
                if (!result)
                {
                    // request error
                    StartCoroutine(StartLoginAnim(true, new Vector3(0, -90, 0), Vector3.zero, () =>
                    {
#if !UNITY_EDITOR
                        NativeBinding.Instance.ToggleSpinner(false);
#else
                        m_imageSpinner.SetActive(false);
#endif
                    }));
                }
            }));
        }));
    }

    public void BtnCancel_OnClick()
    {
        StartCoroutine(StartLoginAnim(true, new Vector3(0, -90, 0), Vector3.zero, () =>
        {
#if !UNITY_EDITOR
            NativeBinding.Instance.ToggleSpinner(false);
#else
            m_imageSpinner.SetActive(false);
#endif
        }));
    }
}
