﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace SoftLiu.Utilities
{
    public class AutoGeneratedSingletonMonobehaviour<T> : MonoBehaviour where T : AutoGeneratedSingletonMonobehaviour<T>
    {
        private static bool m_isCreate = false;
        private static T m_instance = null;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (m_instance == null && !m_isCreate)
                    {
                        T tInScene = GameObject.FindObjectOfType<T>();
                        if (tInScene == null)
                        {
                            string className = typeof(T).Name;
                            GameObject tPrefab = ManagerPrefabs.Instance.GetEntryPrefab(className);
                            if (tPrefab != null)
                            {
                                GameObject tObj = GameObject.Instantiate(tPrefab) as GameObject;
                                GameObject.DontDestroyOnLoad(tObj);
                                tObj.name = className;
                                tInScene = tObj.GetComponent<T>();
                                m_isCreate = true;
                            }
                            else
                            {
                                Debug.LogError("SingletonMonobehaviour GetEntryPrefab is null.");
                            }
                        }
                        m_instance = tInScene;
                    }
                    return m_instance;
                }
            }
        }
    }
}
