using Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData : ScriptableObject
{

    [SerializeField]
    List<PlayerInitData> m_playerInitData;


    private void Import(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey("PlayerInitData"))
        {
            m_playerInitData = CreateInstances<PlayerInitData>(dict["PlayerInitData"] as List<object>);
        }
    }
}
