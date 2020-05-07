using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;

namespace SoftLiu.Servers
{
    public enum Method
    {
        POST,
        GET,
        PUT
    }
    public class Request
    {

        protected int m_connectionTimeout = 5;
        protected int m_timeout = 20;


        protected void Run(string url, Method method, Hashtable headers, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(url);
                sb.Append('?');
                int i = 0;
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    if (i > 0) sb.Append('&');
                    sb.Append(string.Format("{0}={1}", item.Key, item.Value));
                    i++;
                }
                url = sb.ToString();
            }

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.Method = method.ToString();
            req.ContinueTimeout = m_connectionTimeout;
            req.Timeout = m_timeout;

            if (headers != null)
            {
                foreach (DictionaryEntry item in headers)
                {
                    req.Headers.Add(item.Key as string, item.Value as string);
                }
            }

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
        }

    }
}
