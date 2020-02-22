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


    [System.Serializable]
    public partial class PlayerInitData : ObjectData
    {
        public const string KeyPlayer = "player";

        [SerializeField]
        private System.Int32 m_gems;
        public System.Int32 gems { get { return m_gems; } }

        [SerializeField]
        private System.Int32 m_coins;
        public System.Int32 coins { get { return m_coins; } }

        [SerializeField]
        private System.Int32 m_tickets;
        public System.Int32 tickets { get { return m_tickets; } }

        [SerializeField]
        private System.Int32 m_pearls;
        public System.Int32 pearls { get { return m_pearls; } }
    }

}
