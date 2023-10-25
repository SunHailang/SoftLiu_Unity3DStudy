using UnityEditor;
using SoftLiu.AssetBundles;

public class ScriptableObjectMenu
{
    [MenuItem("SoftLiu/Misc/Editor/Create AssetBundle Data")]
    public static void CreateAssetBundleData()
    {
        ScriptableObjectUtility.CreateAsset<AssetBundleData>();
    }

    [MenuItem("SoftLiu/Misc/Editor/Create SceneBundle Data")]
    public static void CreateSceneBundles()
    {
        ScriptableObjectUtility.CreateAsset<SceneBundle>();
    }
}
