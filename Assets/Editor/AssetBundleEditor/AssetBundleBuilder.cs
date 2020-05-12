using SoftLiu.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{



    public static void GenerateData()
    {
        EditorUtility.DisplayProgressBar("Creating Asset Bundle Data", "Creating Scriptable Objects", 0f);

        string[] assetGUIDs = AssetDatabase.FindAssets("t:Bundle");

        List<Bundle> existingBundles = new List<Bundle>();

        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            Bundle loadedBundle = AssetDatabase.LoadAssetAtPath<Bundle>(AssetDatabase.GUIDToAssetPath(assetGUIDs[i]));
            existingBundles.Add(loadedBundle);
        }

        string resourcesPath = "Assets/Resources/AssetBundles/";
        string scripatblePath = resourcesPath + "AssetBundleData.asset";

        AssetBundleData assetBundleData = (AssetBundleData)AssetDatabase.LoadAssetAtPath(scripatblePath, typeof(AssetBundleData));

        if (assetBundleData == null)
        {
            assetBundleData = ScriptableObject.CreateInstance<AssetBundleData>();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(scripatblePath);
            AssetDatabase.CreateAsset(assetBundleData, assetPathAndName);
        }

        EditorUtility.DisplayProgressBar("Creating Asset Bundle Data", "Creating SingletonMono Data", 0f);
        List<Bundle> allbundles = new List<Bundle>();

        List<Bundle> singletonMonoBundles = GenerateSingletonMonoBundleData(existingBundles, resourcesPath);
        for (int i = 0; i < singletonMonoBundles.Count; i++)
        {
            allbundles.Add(singletonMonoBundles[i]);
        }



        assetBundleData.Bundles = allbundles.ToArray();
        EditorUtility.SetDirty(assetBundleData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static List<Bundle> GenerateSingletonMonoBundleData(List<Bundle> existingBundles, string resourcesPath)
    {
        //clear existing data
        for (int i = 0; i < existingBundles.Count; i++)
        {
            if (existingBundles[i] is SingletonMonoBundle)
            {
                existingBundles[i].ClearAssetPaths();
            }
        }
        List<Bundle> singletonMonoBundles = new List<Bundle>();

        return singletonMonoBundles;
    }


}
