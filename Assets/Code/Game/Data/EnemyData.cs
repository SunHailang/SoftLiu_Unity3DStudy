using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    [SerializeField]
    private System.String m_key;
    public System.String Key { get { return m_key; } }

    [SerializeField]
    private System.Int32 m_blood;
    public System.Int32 Blood { get { return m_blood; } }

    [SerializeField]
    private System.Single m_attack;
    public System.Single Attack { get { return m_attack; } }

    [SerializeField]
    private System.Single m_move;
    public System.Single Move { get { return m_move; } }
}
