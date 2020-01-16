using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu.Utilities;

[Serializable]
public class GetServerTimeResponseInternal
{
    public long unixTimestamp;

    public GetServerTimeResponseInternal(long unixTimestamp)
    {
        this.unixTimestamp = unixTimestamp;
    }
}

public class GetServerTimeResponse
{
    [SerializeField]
    public GetServerTimeResponseInternal response;

    public bool error;

    public GetServerTimeResponse(string response, bool errorCode)
    {
        //解析 回调
        this.error = errorCode;
        if (!this.error)
        {
            try
            {
                // 解析 response
                DateTime dateTime = Convert.ToDateTime(response).ToLocalTime();
                long stamps = TimeUtilities.Instance.GetTimeStamp(dateTime);
                this.response = new GetServerTimeResponseInternal(stamps);
            }
            catch (Exception ex)
            {
                Debug.LogError("GetServerTimeResponse Response Error : " + ex.Message);
            }
        }
    }
}
