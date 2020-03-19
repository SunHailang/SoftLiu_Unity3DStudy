using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
    public static T CreateAsset<T>(string assetPathAndName = null) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        if (string.IsNullOrEmpty(assetPathAndName))
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        }
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        string assetName = typeof(T).ToString();
        int index = assetName.LastIndexOf('.');
        asset.name = assetName.Substring(index + 1, assetName.Length - index - 1);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

    public static T CreateAsset<T>(Object parentAsset) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        asset.name = AssetDatabase.GenerateUniqueAssetPath("New " + typeof(T).ToString());

        AssetDatabase.AddObjectToAsset(asset, parentAsset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }
}
