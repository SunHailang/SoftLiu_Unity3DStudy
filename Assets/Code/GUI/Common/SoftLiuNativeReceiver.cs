using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftLiuNativeReceiver : MonoBehaviour
{
    #region SINGLETON

    private static SoftLiuNativeReceiver m_instance = null;

    public static SoftLiuNativeReceiver Instance
    {
        get { return m_instance; }
    }

    #endregion
    private void Awake()
    {
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MessageBoxClick(string msg)
    {

    }
}
