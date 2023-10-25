using UnityEditor;

public class GameDBEditor
{
    [MenuItem("SoftLiu/Editor/GameDB/Build gameDB.gdb", false, 100)]
    public static void BuildGameDB()
    {
        GameDataBuilder.BuildGameData();
    }
}
