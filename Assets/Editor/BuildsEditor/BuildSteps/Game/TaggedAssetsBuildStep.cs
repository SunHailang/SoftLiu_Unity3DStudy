using SoftLiu.AssetBundles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    [CreateAssetMenu]
    public class CreateBundleFile : ScriptableObject
    {
        [Serializable]
        public class DirectoryBundleInfo
        {
            /// <summary>
            /// 当前文件夹内的 bundle 文件夹名字(小写) , 后面会 是连接文件的名字
            /// </summary>
            [SerializeField]
            private string m_bundleNameWithoutFile;
            public string bundleNameWithoutFile { get { return m_bundleNameWithoutFile; } }
            /// <summary>
            /// 文件夹路径， 文件夹内所有的文件 (.meta 除外) 都会打成 AssetBundle
            /// </summary>
            [SerializeField]
            private string m_filePath;
            public string filePath { get { return m_filePath; } }
            /// <summary>
            /// 生成的 Asset 文件存放的位置
            /// </summary>
            [SerializeField]
            private string m_saveAssetPath;
            public string saveAssetPath { get { return m_saveAssetPath; } }
        }

        [SerializeField]
        private DirectoryBundleInfo[] m_directoryInfos;
        public DirectoryBundleInfo[] directoryInfos { get { return m_directoryInfos; } }
    }

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
#if UNITY_EDITOR
            Debug.Log("BundleNames: Start.");



            //EditorUtility.ClearProgressBar();
            //return;
            try
            {
                string resourcesPath = "Assets/Resources/AssetBundles/";
                string scripatblePath = resourcesPath + "AssetBundleData.asset";

                AssetBundleData assetBundleData = (AssetBundleData)AssetDatabase.LoadAssetAtPath(scripatblePath, typeof(AssetBundleData));

                string createBundlePath = "Assets/Misc/AssetBundle/CreateBundleFile.asset";
                CreateBundleFile createBundleData = (CreateBundleFile)AssetDatabase.LoadAssetAtPath(createBundlePath, typeof(CreateBundleFile));

                List<Bundle> bundles = new List<Bundle>();
                for (int i = 0; i < createBundleData.directoryInfos.Length; i++)
                {
                    CreateBundleFile.DirectoryBundleInfo bundleInfo = createBundleData.directoryInfos[i];
                    bundles.AddRange(CreateBundle(bundleInfo.bundleNameWithoutFile, bundleInfo.filePath, bundleInfo.saveAssetPath, (name) =>
                    {
                        UnityEditor.EditorUtility.DisplayProgressBar("CreateSingletonMonoBundle", name, i + 1 / createBundleData.directoryInfos.Length);
                    }));
                }

                assetBundleData.Bundles = bundles.ToArray();
            }
            catch (Exception error)
            {
                Debug.LogError("AddAllBundleNames Error: " + error.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
#endif
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

        public static List<Bundle> CreateBundle(string bundleName, string filePath, string assetPath, System.Action<string> callback)
        {
            List<Bundle> bundles = new List<Bundle>();

            DirectoryInfo dir = new DirectoryInfo(filePath);

            foreach (var item in dir.GetFiles())
            {
                //FileInfo item = new FileInfo(path2 + "/App.prefab");
                if (item.Extension == ".meta") continue;
                string withoutEx = Path.GetFileNameWithoutExtension(item.FullName);
                if (callback != null) callback(withoutEx);
                string name = bundleName + "/" + withoutEx.ToLower();
                AssetImporter importer = AssetImporter.GetAtPath(filePath + "/" + item.Name);
                importer.assetBundleName = bundleName;
                string assetFilePath = assetPath + "/" + withoutEx + "BundleData.asset";
                if (File.Exists(assetFilePath)) File.Delete(assetFilePath);
                SingletonMonoBundle asset = ScriptableObject.CreateInstance<SingletonMonoBundle>();
                AssetDatabase.CreateAsset(asset, assetFilePath);
                AssetDatabase.SaveAssets();
                asset.InitData(bundleName, new string[] { importer.assetPath }, withoutEx);
                bundles.Add(asset);
            }
            return bundles;
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


        public static void SetupBundles()
        {
            string resourcesPath = "Assets/Resources/AssetBundles/";
            string scripatblePath = resourcesPath + "AssetBundleData.asset";

            AssetBundleData assetBundleData = (AssetBundleData)AssetDatabase.LoadAssetAtPath(scripatblePath, typeof(AssetBundleData));

            if (assetBundleData != null)
            {
                for (int i = 0; i < assetBundleData.Bundles.Length; i++)
                {
                    assetBundleData.Bundles[i].EnableBundle();
                }
            }
            else
            {
                Debug.LogError("assetBundleData could not be loaded");
            }
        }
    }
}
