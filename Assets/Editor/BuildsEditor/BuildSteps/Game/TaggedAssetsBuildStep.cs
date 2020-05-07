using SoftLiu.AssetBundles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class TaggedAssetsBuildStep : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            AssetDatabase.StartAssetEditing();


            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Direct;
        }

        [MenuItem("SoftLiu/AssetBundles/ENABLE BUNDLES/All")]
        public static void EnableAssetBundles()
        {
            AssetDatabase.StartAssetEditing();
            AddAllBundleNames();
            AddAssetBundleDefines();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void AddAssetBundleDefines()
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            // 添加 AssetBundle 的 宏
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), defines);
        }

        private static void AddAllBundleNames()
        {
            Debug.Log("BundleNames: Start.");
            string resourcesPath = "Assets/Resources/AssetBundles/";
            string scripatblePath = resourcesPath + "AssetBundleData.asset";

            AssetBundleData assetBundleData = (AssetBundleData)AssetDatabase.LoadAssetAtPath(scripatblePath, typeof(AssetBundleData));

            foreach (Bundle bundle in assetBundleData.Bundles)
            {
                if (bundle == null) continue;
                foreach (string path in bundle.assetPaths)
                {

                }
                //AssetImporter importer = AssetImporter.GetAtPath(bundle.);
                //string bundleName = string.Format("{0}/{1}/{2}", dirInfo.Parent.Name, dirInfo.Name, Path.GetFileNameWithoutExtension(file.FullName));
                //Debug.Log("BundleNames: " + bundleName);

            }
        }

        [MenuItem("SoftLiu/AssetBundles/DISABLE BUNDLES/All")]
        public static void DisableAssetBundles()
        {
            AssetDatabase.StartAssetEditing();
            RemoveAllBundleNames();
            RemoveAssetBundleDefines();
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void RemoveAllBundleNames()
        {
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();

            for (int i = 0; i < assetBundleNames.Length; i++)
            {
                UnityEditor.AssetDatabase.RemoveAssetBundleName(assetBundleNames[i], true);
            }
        }

        public static void RemoveAssetBundleDefines()
        {
            // BuildTargetGroup.
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            // 取消 AssetBundle 的宏
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), defines);
        }

    }
}
