using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameDBEditor
{
    [MenuItem("SoftLiu/Editor/GameDB/Build gameDB.gdb")]
    public static void BuildGameDB()
    {
        GameDataBuilder.BuildGameData();
    }
}
