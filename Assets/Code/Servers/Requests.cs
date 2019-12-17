using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Requests
{

    public void GetWebRequest(string url)
    {

        UnityWebRequest ww = UnityWebRequest.Get(url);
        if (ww.isDone)
        {

        }
    }

    public void PostWebRquest()
    {

    }

}
