using Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInitData : ObjectData
{
    [SerializeField]
    private System.String m_key;
    public System.String Key { get { return m_key; } }

    [SerializeField]
    private System.Int32 m_gems;
    public System.Int32 Gems { get { return m_gems; } }

    [SerializeField]
    private System.Int32 m_coins;
    public System.Int32 Coins { get { return m_coins; } }

    [SerializeField]
    private System.Int32 m_pearls;
    public System.Int32 Pearls { get { return m_pearls; } }

    [SerializeField]
    private System.Int32 m_tickets;
    public System.Int32 Tickets { get { return m_tickets; } }

}
