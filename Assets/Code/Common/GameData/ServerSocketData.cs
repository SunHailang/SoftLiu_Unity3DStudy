using Definitions;
using UnityEngine;

[System.Serializable]
public class ServerSocketData : ObjectData
{
    [SerializeField]
    private System.String m_key;
    public System.String Key { get { return m_key; } }

    [SerializeField]
    private System.String m_name;
    public System.String Name { get { return m_name; } }

    [SerializeField]
    private System.String m_type;
    public System.String Type { get { return m_type; } }

    [SerializeField]
    private System.String m_serverIP;
    public System.String ServerIP { get { return m_serverIP; } }

    [SerializeField]
    private System.Int32 m_serverPort;
    public System.Int32 ServerPort { get { return m_serverPort; } }
}
