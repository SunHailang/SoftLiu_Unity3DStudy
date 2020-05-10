using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 贝塞尔曲线公式
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private Vector3 CalculateBezier(float t)
    {
        Vector3 ret = new Vector3(0, 0, 0);
        //阶数
        int n = 0;
        for (int i = 0; i <= n; i++)
        {
            Vector3 pi = Vector3.zero;
            ret = ret + Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i) * Cn_m(n, i) * pi;
        }
        return ret;
    }

    private int Cn_m(int n, int m)
    {
        int ret = 1;
        for (int i = 0; i < m; i++)
        {
            ret = ret * (n - i) / (i + 1);
        }
        return ret;
    }
}
