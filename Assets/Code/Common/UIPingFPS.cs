using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPingFPS : MonoBehaviour
{
    private const string m_IP = "baidu.com";

    private void Awake()
    {

    }

    private void Start()
    {
        SendPing();
        StartCoroutine(SendPing());
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.normal.textColor = Color.green;

        float ping = 0;
        int pingFinishCount = 0;
        int pingNotFinishCount = 0;
        for (int i = 0; i < m_pingArrary.Length; i++)
        {
            if (m_pingArrary[i] != null && m_pingArrary[i].isDone)
            {
                pingFinishCount++;
                ping += m_pingArrary[i].time;
            }
            else
            {
                pingNotFinishCount++;
            }
        }
        if (pingFinishCount > 0)
        {
            ping = ping / pingFinishCount;
            GUI.Label(new Rect(50, 20, 500, 30), "Ping: " + Mathf.CeilToInt(ping) + "ms");
        }
        else
        {
            GUI.Label(new Rect(50, 20, 500, 30), "Ping: " + -1 + "ms");
        }


        float fps = 0;//1.0f / deltaTime;
        int count = 0;
        for (int i = 0; i < m_deltaTimeArray.Length; i++)
        {
            if (m_deltaTimeArray[i] <= 0) continue;
            count++;
            fps += m_deltaTimeArray[i];
        }
        fps = fps / count;
        GUI.Label(new Rect(50, 50, 500, 30), "FPS: " + Mathf.CeilToInt(1.0f / fps));
    }
    private float deltaTime = 0f;
    private float[] m_deltaTimeArray = new float[20];
    private int index = 0;
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        m_deltaTimeArray[index] = deltaTime;
        index++;
        if (index >= m_deltaTimeArray.Length)
        {
            index = 0;
        }
    }
    private Ping[] m_pingArrary = new Ping[10];
    private IEnumerator SendPing()
    {
        int pingIndex = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Ping ping = new Ping(m_IP);
            if (m_pingArrary[pingIndex] != null)
            {
                m_pingArrary[pingIndex].DestroyPing();
                m_pingArrary[pingIndex] = null;
            }
            m_pingArrary[pingIndex] = ping;
            pingIndex++;
            if (pingIndex >= m_pingArrary.Length)
            {
                pingIndex = 0;
            }
        }
    }
}
