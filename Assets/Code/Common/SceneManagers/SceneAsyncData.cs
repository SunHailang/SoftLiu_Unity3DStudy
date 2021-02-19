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

        private List<AsyncOperation> m_sceneAsyncList = null;
        public List<AsyncOperation> SceneAsyncList
        {
            get
            {
                return m_sceneAsyncList;
            }
        }

        private float m_process = 0.0f;

        public SceneAsyncData(string name, List<AsyncOperation> sceneAsyncList, System.Action<SceneAsyncData> complete)
        {
            m_name = name;
            m_sceneAsyncList = sceneAsyncList;
            m_onFinish = complete;
            m_process = 0.0f;
        }

        public float ProcessIfFinished()
        {
            if (m_sceneAsyncList == null)
            {
                return 1.0f;
            }
            bool isDone = true;
            foreach (var sceneAsync in m_sceneAsyncList)
            {
                if (!sceneAsync.isDone) { isDone = false; }
                m_process = Mathf.Min(sceneAsync.progress, m_process);
                m_process = sceneAsync.progress;
            }
            if (isDone)
            {
                if (m_onFinish != null)
                {
                    m_onFinish(this);
                }
                return 1.0f;
            }
            else
            {
                return Mathf.Min(0.89f, m_process);
            }
        }
    }
}