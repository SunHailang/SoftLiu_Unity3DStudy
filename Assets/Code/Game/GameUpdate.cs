using SoftLiu.Servers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu.SceneManagers;

public class GameUpdate : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        RequestsManager.Instance.OnUpdate();
        SceneManager.Instance.OnUpdate();
    }

    private void LateUpdate()
    {
        
    }
}
