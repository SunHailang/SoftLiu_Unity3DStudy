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
    }
}
