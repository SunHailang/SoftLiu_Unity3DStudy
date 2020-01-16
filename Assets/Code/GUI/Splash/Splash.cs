using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SoftLiu.Utilities;
using SoftLiu.Servers;
using System;

public class TimeResponse
{
    public DataResponse data;
    public int errcode;
}

public class DataResponse
{
    public int time;
}

public class Splash : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        string json = "{\"data\":{\"time\":1579083760},\"errcode\":-1}";
        TimeResponse time = JsonUtility.FromJson<TimeResponse>(json);
        Debug.Log(time);

        TimeResponse t1 = MiniJSON.Deserialize(json) as TimeResponse;
        if (t1 == null)
        {
            Debug.Log("t1 is null.");
        }
        else
        {
            Debug.Log(t1);
        }

        TimeResponse t2 = new TimeResponse() { data = new DataResponse() { time = 1234567 }, errcode = -1 };
        string t2Str = JsonUtility.ToJson(t2);
        Debug.Log(t2Str);

        //string t3 = "Thu, 16 Jan 2020 05:45:15 GMT";
        //DateTime date = Convert.ToDateTime(t3);
        //date = date.ToLocalTime();
        //Debug.Log(date.ToString("yyyy/MM/dd HH:mm:ss"));
    }

    public void OnClick()
    {
        RequestsManager.Instance.GetServerTime((response) =>
        {
            if (!response.error)
            {
                Debug.Log(response.response.unixTimestamp);
            }
        });

        SaveFacade.Instance.Init();
    }

    IEnumerator get(string url)
    {
        UnityWebRequest ww = UnityWebRequest.Get("www.baidu.com");
        yield return ww.SendWebRequest();

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
        RequestsManager.Instance.Update();
    }
}
