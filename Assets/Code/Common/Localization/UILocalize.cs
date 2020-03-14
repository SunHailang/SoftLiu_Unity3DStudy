using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SoftLiu;

namespace SoftLiu.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UILocalize : MonoBehaviour
    {
        [SerializeField]
        private string m_key = null;

        private string m_afterKey = null;

        private object[] m_params = null;

        private TextMeshProUGUI m_text = null;

        private void Awake()
        {
            m_text = gameObject.GetComponent<TextMeshProUGUI>();
            Assert.Fatal(m_text != null, string.Format("name: {0} UILocalize TextMeshProUGUI is null.", gameObject.name));
        }

        private void OnEnable()
        {
            m_afterKey = m_key;
            OnLoad();
        }

        private void Update()
        {
            if (m_afterKey != m_key)
            {
                m_key = m_afterKey;
                OnLoad(m_params);
            }
        }

        public void SetValue(string key, params object[] mParams)
        {
            m_afterKey = key;
            m_params = mParams;
        }

        private void OnLoad(params object[] mParams)
        {
            string value = Localization.Instance.Get(m_afterKey);
            if (mParams == null)
            {
                m_text.SetText(value);
            }
            else
            {
                m_text.SetText(string.Format(value, mParams));
            }
        }
    }
}