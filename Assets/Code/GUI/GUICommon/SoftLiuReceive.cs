using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftLiuReceive : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
