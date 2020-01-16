using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SoftLiu.Servers
{
    public class RequestHandle
    {
        public Action<string, string> onFinish;
        public UnityWebRequest request;

        public RequestHandle(UnityWebRequest request, Action<string, string> onFinish)
        {
            this.request = request;
            this.onFinish = onFinish;
        }

        public bool ProcessIfFinished()
        {
            if (request.isDone)
            {
                if (onFinish != null)
                {
                    onFinish(request.error, request.downloadHandler.text);
                    onFinish = null;
                }
                return true;
            }
            return false;
        }
    }
}
