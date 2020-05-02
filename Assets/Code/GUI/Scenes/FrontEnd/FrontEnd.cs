using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FrontEnd : MonoBehaviour
{


    

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
}
