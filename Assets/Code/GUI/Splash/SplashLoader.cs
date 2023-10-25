using System.Collections;
using UnityEngine;
using SoftLiu.Save;
using SoftLiu.Authentication;
using UnityEngine.EventSystems;

public class SplashLoader : MonoBehaviour, IPointerClickHandler
{
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
