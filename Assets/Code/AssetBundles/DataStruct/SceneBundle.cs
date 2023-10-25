using SoftLiu.AssetBundles.Downloader;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    [CreateAssetMenu]
    public class SceneBundle : Bundle
    {
        [SerializeField]
        private string m_stateName;
        public string stateName { get { return m_stateName; } }

        public void InitData(string assetBundleName, string[] assets, string stateName)
        {
            base.InitData(assetBundleName, assets);
            m_stateName = stateName;
        }

        public override string GetContentString()
        {
            string content = "";

            for (int i = 0; i < m_assetPaths.Length; i++)
            {
                //                string key = Path.GetFileNameWithoutExtension(m_assetPaths[i]);
                //                AssBunsServerKeys masterKey = GameDataManager.Instance.gameDB.GetItem<AssBunsServerKeys>(key.ToLower());
                //                if (masterKey != null)
                //                {
                //                    content += masterKey.serverKey;
                //                }
                //                else
                //                {
                //                    content += key;
                //                    Debug.LogWarning("No ass buns server key for asset " + key)
                //;
                //                }
                //                if (i < m_assetPaths.Length - 1)
                //                {
                //                    content += ";";
                //                }
            }

            return content;
        }

        public void UpdateBundleData(string[] assets)
        {
            m_assetPaths = assets;
        }

        public override DownloadPriority GetDownloadPriority()
        {
            //if (state == AssetBundleDownloadState.Loadable)
            //{
            //    return DownloadPriority.Medium;
            //}
            //if (m_overrideDownloadPriority != DownloadPriority.Medium)
            //{
            //    return m_overrideDownloadPriority;
            //}
            //if (GetPermission())
            //{
            //    return DownloadPriority.Medium;
            //}
            //if (!RequiresPermission() && AssetBundleManager.Instance.CurrentReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            //{
                //LevelData levelData = GameDataManager.Instance.gameDB.GetItem<LevelData>(m_stateName);

                //if (App.Instance.PlayerProgress.GetLevelTemporaryUnlocked(m_stateName) == true)
                //{
                //    return DownloadPriority.Medium;
                //}
                //if (levelData.type == LevelData.LevelTypes.Normal)
                //{
                //    if (App.Instance.PlayerProgress.GetIsLevelUnlocked(m_stateName))
                //    {
                //        return DownloadPriority.Medium;
                //    }

                //    if (ItemDataManager.Instance != null && ItemDataManager.Instance.sharkDataManager != null)
                //    {
                //        LevelUnlockData unlockData = GameDataManager.Instance.gameDB.GetItem<LevelUnlockData>(m_stateName);

                //        int numSharks = ItemDataManager.Instance.sharkDataManager.GetPurchasedSharks().Count;
                //        if (numSharks + 1 >= unlockData.numSharksToUnlock)
                //        {
                //            return DownloadPriority.Medium;
                //        }
                //    }
                //return DownloadPriority.Medium;
            //}
            //else if (levelData.type == LevelData.LevelTypes.Boss || levelData.type == LevelData.LevelTypes.Mini)
            //{
            //    if ((ItemDataManager.Instance != null && ItemDataManager.Instance.sharkDataManager != null) && ItemDataManager.Instance.sharkDataManager.GetHighestUnlockSharkTierWithOwnedSharks() >= 2)
            //    {
            //        EventTracker highestPriorityEventForLevel = DailyEventsManager.Instance.GetHighestPriorityEventForLevel(m_stateName);

            //        if (highestPriorityEventForLevel != null)
            //        {
            //            return DownloadPriority.Low;
            //        }
            //        return DownloadPriority.DoNotDownload;
            //    }
            //}

            //}
            return DownloadPriority.DoNotDownload;
        }

        public override void EnableBundle()
        {
#if UNITY_EDITOR

            Debug.Log("Enabling " + assetBundleName);
            //first, wipe any assets in that bundle
            // AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
            // List<UnityEditor.EditorBuildSettingsScene> SceneList = new List<UnityEditor.EditorBuildSettingsScene>(UnityEditor.EditorBuildSettings.scenes);
            // for (int i = 0; i < assetPaths.Length; i++)
            // {
            //     //assign to bundle
            //     UnityEditor.AssetImporter.GetAtPath(assetPaths[i]).assetBundleName = assetBundleName;
            //
            //     //go through scene build list, remove the relevant scene
            //     foreach (UnityEditor.EditorBuildSettingsScene scene in SceneList)
            //     {
            //         if (scene.path == assetPaths[i])
            //         {
            //             scene.enabled = false;
            //             break;
            //         }
            //     }
            // }
            // UnityEditor.EditorBuildSettings.scenes = SceneList.ToArray();
#endif
        }

        public override void DisableBundle()
        {
#if UNITY_EDITOR
            // UnityEditor.AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
            // List<UnityEditor.EditorBuildSettingsScene> SceneList = new List<UnityEditor.EditorBuildSettingsScene>(UnityEditor.EditorBuildSettings.scenes);
            // for (int i = 0; i < assetPaths.Length; i++)
            // {
            //     //go through scene build list, remove the relevant scene
            //     foreach (UnityEditor.EditorBuildSettingsScene scene in SceneList)
            //     {
            //         if (scene.path == assetPaths[i])
            //         {
            //             scene.enabled = true;
            //         }
            //     }
            // }
            // UnityEditor.EditorBuildSettings.scenes = SceneList.ToArray();
#endif
        }
    }
}
