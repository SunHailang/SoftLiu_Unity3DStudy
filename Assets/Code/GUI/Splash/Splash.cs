using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Splash : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        
    }

    IEnumerator get(string url)
    {
        UnityWebRequest ww = UnityWebRequest.Get("www.baidu.com");
        yield return ww.Send();

        if (ww.isNetworkError || ww.isHttpError)
        {

        }
        else
        {
            Debug.Log(ww.downloadedBytes);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
