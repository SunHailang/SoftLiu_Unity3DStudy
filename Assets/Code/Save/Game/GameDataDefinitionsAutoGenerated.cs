using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Definitions
{
    [System.Serializable]
    public partial class ObjectData
    {
        [SerializeField]
        protected System.String m_key;
        public System.String key { get { return m_key; } }
    }


}
