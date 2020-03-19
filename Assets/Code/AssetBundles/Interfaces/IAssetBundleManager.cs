using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    public interface IAssetBundleManager
    {
        /// <summary>
		/// Monobehavours
		/// </summary>
		void Awake();
        void Start();
        void Update();
        void OnApplicationQuit();
        void OnApplicationPause(bool pause);
        void OnDestroy();

        string CurrentVersionDownloadLocation { get; }
        NetworkReachability CurrentReachability { get; }
        string BundleServerURL { get; }
        void ResetPollingTime();
        bool CheckAndResetCurrentPriorityChanged();
        bool CheckAndResetDownloadTimeout();
        bool IsInGame { get; }

        int SecondsBeforeBlockedBundleIsRetried { get; }

        ulong GetFreeDiskSpace();

        void ForceRecheckOfAllAssetBundles();

        bool ShowDetailedInfo { get; set; }

        double GetDownloadSpeedKilobytes();

        Dictionary<string, Bundle> BundleQueue { get; }

        AssetBundleDownloadState GetOverallStateOfAssetBundles(Bundle[] assetBundles);
        float GetPercentProgressOfDownload(Bundle[] AssetBundles);
        float GetAssetBundleDownloadRemainingMB(Bundle assetBundle);
        float GetAssetBundlesDownloadProgressMB(Bundle[] assetBundles);
        float GetAssetBundlesDownloadSizeMB(Bundle[] assetBundles);
        float GetTotalBundleAwaitingPermissionDownloadSizeMB();

        bool IsAnyBundleDownloading(Bundle[] assetBundles);
        bool AreAllBundlesReadyToLoad(Bundle[] bundles);
        bool GetAnyBundleRequirePermission();
        void GivePermissionToDownloadBundles(Bundle[] Bundles);
        void UnloadAllBundles();
        void UnloadAllExceptTheseBundles(Bundle[] bundles);
        void EnableDebugger();
        Bundle[] GetBundles<T>() where T : Bundle;

        Bundle[] GetBundlesInState<T>(AssetBundleDownloadState downloadState) where T : Bundle;

        void NoInternetAssBunsMessage();
        void ShowAssetbundleDownloadMessage(Bundle[] bundles);

        void OnBundlesRequestedByUser(Bundle[] bundles);

    }
}
