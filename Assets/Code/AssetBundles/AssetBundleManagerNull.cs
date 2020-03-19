using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoftLiu.AssetBundles
{
    public class AssetBundleManagerNull : IAssetBundleManager
    {
        public AssetBundleManagerNull()
        {
        }

        public string CurrentVersionDownloadLocation
        {
            get
            {
                return "";
            }
        }

        public NetworkReachability CurrentReachability
        {
            get { return NetworkReachability.NotReachable; }
        }

        public string BundleServerURL
        {
            get { return ""; }
        }

        public bool IsInGame { get { return false; } }

        public int SecondsBeforeBlockedBundleIsRetried
        {
            get { return 0; }
        }

        public bool ShowDetailedInfo { get { return false; } set {; } }

        public Dictionary<string, Bundle> BundleQueue { get { return null; } }

        public void EnableDebugger()
        { }

        public bool AreAllBundlesReadyToLoad(Bundle[] bundles)
        {
            return true;
        }

        public void Awake()
        {

        }

        public bool CheckAndResetCurrentPriorityChanged()
        {
            return false;
        }

        public bool CheckAndResetDownloadTimeout()
        {
            return false;
        }

        public void ForceRecheckOfAllAssetBundles()
        {

        }

        public float GetAssetBundleDownloadRemainingMB(Bundle assetBundleNames)
        {
            return 0.0f;
        }

        public float GetAssetBundlesDownloadProgressMB(Bundle[] assetBundleNames)
        {
            return 0.0f;
        }

        public float GetAssetBundlesDownloadSizeMB(Bundle[] assetBundleNames)
        {
            return 0.0f;
        }

        public double GetDownloadSpeedKilobytes()
        {
            return 0.0f;
        }

        public ulong GetFreeDiskSpace()
        {
            return 0;
        }

        public AssetBundleDownloadState GetOverallStateOfAssetBundles(Bundle[] assetBundles)
        {
            return AssetBundleDownloadState.Initialising;
        }

        public float GetPercentProgressOfDownload(Bundle[] assetBundles)
        {
            return 0.0f;
        }

        public void GivePermissionToDownloadBundles(Bundle[] bundles)
        {

        }

        public bool IsAnyBundleDownloading(Bundle[] assetBundleNames)
        {
            return false;
        }

        public void OnApplicationPause(bool pause)
        {

        }

        public void OnApplicationQuit()
        {

        }

        public void OnDestroy()
        {

        }

        public void ResetPollingTime()
        {

        }

        public void Start()
        {

        }

        public string[] TryGetAssetBundleNamesFromLevel(string levelName)
        {
            return null;
        }

        public string[] TryGetAssetBundleNamesToPlay(string levelName, string SharkKey)
        {
            return null;
        }

        public void UnloadAllBundles()
        {

        }

        public void UnloadAllExceptTheseBundles(Bundle[] bundles)
        {
            //throw new System.NotImplementedException();
        }

        public void Update()
        {

        }

        public bool GetAnyBundleRequirePermission()
        {
            return false;
        }

        public float GetTotalBundleAwaitingPermissionDownloadSizeMB()
        {
            return 0f;
        }

        public Bundle[] GetBundles<T>() where T : Bundle
        {
            return null;
        }

        public Bundle[] GetBundlesInState<T>(AssetBundleDownloadState downloadState) where T : Bundle
        {
            return null;
        }

        public void NoInternetAssBunsMessage()
        {
        }

        public void ShowAssetbundleDownloadMessage(Bundle[] bundles)
        {
        }

        public void OnBundlesRequestedByUser(Bundle[] bundles)
        {
        }

    }
}
