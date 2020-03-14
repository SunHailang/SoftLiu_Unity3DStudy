using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoftLiu.SceneManagers
{
    public class SceneAsyncData
    {
        private System.Action<SceneAsyncData> m_onFinish = null;

        private string m_name = string.Empty;
        public string Name { get { return m_name; } }

        private AsyncOperation m_sceneAsync = null;
        public AsyncOperation SceneAsync
        {
            get
            {
                return m_sceneAsync;
            }
        }

        public SceneAsyncData(string name, AsyncOperation sceneAsync, System.Action<SceneAsyncData> complete)
        {
            m_name = name;
            m_sceneAsync = sceneAsync;
            m_onFinish = complete;
        }

        public bool ProcessIfFinished()
        {
            if (m_sceneAsync == null)
            {
                return true;
            }
            if (m_sceneAsync.isDone)
            {
                if (m_onFinish != null)
                {
                    m_onFinish(this);
                }
                return true;
            }
            return false;
        }
    }
}