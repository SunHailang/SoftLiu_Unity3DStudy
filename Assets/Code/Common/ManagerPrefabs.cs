using SoftLiu.Servers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntryPrefab
{
    public string type;
    public GameObject prefab;
}

public class ManagerPrefabs : MonoBehaviour
{
    private static ManagerPrefabs m_instance = null;
    private static object _lock = new object();

    public static ManagerPrefabs Instance
    {
        get
        {
            lock (_lock)
            {
                if (m_instance == null)
                {
                    Debug.LogError("ManagerPrefabs Instance is null.");
                }
                return m_instance;
            }
        }
    }

    [SerializeField]
    private EntryPrefab[] m_entryPrefabs;

    private void Awake()
    {
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
    }

    public GameObject GetEntryPrefab(string type)
    {
        GameObject obj = null;
        foreach (var item in m_entryPrefabs)
        {
            if (item.type.Equals(type))
            {
                obj = item.prefab;
            }
        }
        return obj;
    }


    private void Update()
    {
        RequestsManager.Instance.OnUpdate();
    }
}
