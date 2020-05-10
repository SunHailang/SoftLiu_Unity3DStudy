using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMapCamera : MonoBehaviour
{
    private void Start()
    {
        // 获取分辨率比例
        float ratio = (float)Screen.width / (float)Screen.height;
        // 使摄像头永远是正方形
        //GetComponent<Camera>().rect = new Rect((1 - 0.2f), (1 - 0.2f * ratio), 0.2f, 0.2f * ratio);
        GetComponent<Camera>().rect = new Rect((1 - 0.2f), 0, 0.2f, 0.2f * ratio);
    }
}
