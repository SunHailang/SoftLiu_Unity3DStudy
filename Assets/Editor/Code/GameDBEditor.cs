using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameDBEditor
{
    [MenuItem("SoftLiu/Editor/GameDB/Build gameDB.gdb", false, 100)]
    public static void BuildGameDB()
    {
        GameDataBuilder.BuildGameData();
    }
}
