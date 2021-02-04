using Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData : ScriptableObject
{

    [SerializeField]
    List<PlayerInitData> m_playerInitData;

    [SerializeField]
    List<EnemyData> m_enemyData;

    [SerializeField]
    List<ServerSocketData> m_serverSocketData;

    private void Import(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey("PlayerInitData"))
        {
            m_playerInitData = CreateInstances<PlayerInitData>(dict["PlayerInitData"] as List<object>);
        }
        if(dict.ContainsKey("EnemyData"))
        {
            m_enemyData = CreateInstances<EnemyData>(dict["EnemyData"] as List<object>);
        }
        if(dict.ContainsKey("ServerSocketData"))
        {
            m_serverSocketData = CreateInstances<ServerSocketData>(dict["ServerSocketData"] as List<object>);
        }
    }
}
