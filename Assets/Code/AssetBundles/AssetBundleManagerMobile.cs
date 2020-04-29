using SoftLiu.AssetBundles.Downloader;
using SoftLiu.Event;
using SoftLiu.Plugins.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    public class AssetBundleManagerMobile : IAssetBundleManager
    {
        private AssetBundleDownloaderManager m_assetBundleDownloaderManager = null;
        private AssetBundleInformationManager m_assetBundleInformationManager = null;
        private Dictionary<string, Bundle> m_bundleQueue = null;
        public Dictionary<string, Bundle> BundleQueue { get { return m_bundleQueue; } }
        //Game specific IAssetBundleAnalytics
        private float m_realtimeToPollAgain = 0;
        private bool m_isInGame = false;
        private bool m_sendAssetBundleLoadableEventOnMainThread;
        private List<AssetBundleCRCInfo> m_includedInBuildCrcList;
        //static values used by the downloader
        private static bool m_hasCurrentPriorityChanged;
        private static bool m_hasDownloadTimedOut;
        private long m_downloadTimeoutLastBytes = 0;
        private float m_realtimeTimeoutStarted = 0;
        public const int PollIntervalSeconds = 3;
        public const int DownloadNoProgressTimeoutSeconds = 15;
        public int m_secondsBeforeBlockedBundleIsRetried = (60 * 15); //15 mins
        public int SecondsBeforeBlockedBundleIsRetried { get { return m_secondsBeforeBlockedBundleIsRetried; } }
        public const string ErrorDiskSpaceFull = "DISK_SPACE_FULL";
        private const string RevokePermissionOnUpdatePref = "AssBunRevokePermissions";
        private AssetBundleData m_assetBundleData;
        public bool IsInGame { get { return m_isInGame; } }
        public bool ShowDetailedInfo { get; set; }
        public string CurrentVersionDownloadLocation { get; private set; }
        public string RootDownloadLocation { get; private set; }
        public long CurrentDownloadSpeed { get; private set; }
        private string m_bundleServerURL = "";
        public string BundleServerURL { get { return m_bundleServerURL; } private set { m_bundleServerURL = value; } }
        private NetworkReachability m_currentReachability = NetworkReachability.NotReachable;
        public NetworkReachability CurrentReachability { get { return m_currentReachability; } private set { m_currentReachability = value; } }

        private List<Bundle> m_permisionToRevokeQueue = new List<Bundle>();
        public void Awake()
        {
            ShowDetailedInfo = false;
            m_hasCurrentPriorityChanged = false;
            m_bundleQueue = new Dictionary<string, Bundle>();
            //EventManager<Events>.Instance.RegisterEvent(Events.OnGameDBLoaded, OnGameDBLoaded);
            //EventManager<Events>.Instance.RegisterEvent(Events.GameFullyLoaded, OnGamePlayStart);
            //EventManager<Events>.Instance.RegisterEvent(Events.CloseResultScreen, OnGamePlayEnd);
        }
        public void OnDestroy()
        {
            //EventManager<Events>.Instance.DeregisterEvent(Events.GameFullyLoaded, OnGamePlayStart);
            //EventManager<Events>.Instance.DeregisterEvent(Events.CloseResultScreen, OnGamePlayEnd);
        }
        public void Start()
        {

            // NW: Yaro gave us this fix. Refer to email chain for details (or chat to Pete, myself or Yaro).


            // Paths.
            RootDownloadLocation = NativeBinding.Instance.GetPersistentDataPath() + "/ABDL/";
            CurrentVersionDownloadLocation = RootDownloadLocation + Application.version;

            // Make sure our current version has a directory.
            if (!Directory.Exists(CurrentVersionDownloadLocation))
            {
                Directory.CreateDirectory(CurrentVersionDownloadLocation);
            }

            CurrentDownloadSpeed = 0;
        }
        public void OnApplicationQuit()
        {
            if (m_assetBundleDownloaderManager != null)
            {
                Debug.Log("Application is quiting, killing Asset bundle threads");
                m_assetBundleDownloaderManager.ApplicationQuitKillDownloadThreads();
                m_assetBundleInformationManager.ApplicationQuitKillInformationThreads();
            }
        }
        public void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                CurrentReachability = Util.IsInternetReachable();

                if (CurrentReachability != NetworkReachability.NotReachable)
                {
                    Debug.Log("Application resumed from pause, ");
                    RecheckAllAssetBundles();
                }
            }
        }
        public void UnloadAllBundles()
        {
            for (int i = 0; i < m_assetBundleData.Bundles.Length; i++)
            {
                m_assetBundleData.Bundles[i].UnloadAssetBundle();
            }
        }
        public void UnloadAllExceptTheseBundles(Bundle[] bundles)
        {
            for (int i = 0; i < m_assetBundleData.Bundles.Length; i++)
            {
                bool found = false;
                for (int j = 0; j < bundles.Length; j++)
                {
                    if (m_assetBundleData.Bundles[i] == bundles[j])
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    m_assetBundleData.Bundles[i].UnloadAssetBundle();
                }

            }
        }
        private void AttemptUpdateFromPreviousVersion()
        {
            // Check old directories:
            string[] oldDirectoryPaths = Directory.GetDirectories(RootDownloadLocation);
            foreach (string directoryPath in oldDirectoryPaths)
            {
                // We obviously do not need to migrate the CURRENT directory (or any of its parents).
                if (CurrentVersionDownloadLocation.StartsWith(directoryPath))
                {
                    continue;
                }

                DirectoryInfo oldDirectory = new DirectoryInfo(directoryPath);

                // CRC check each file against the included-in-build CRC list.
                // This special list is used to determine which files can be safely migrated to the current version directory.
                // I.e. They have not changed between releases, and will not break the build if used.

                // DEVELOPMENT builds are in yet another subfolder (revision number).
                FileInfo[] oldAssetBundleFiles = oldDirectory.GetFiles("*", SearchOption.AllDirectories);

                Debug.Log("AssetBundleManager::Found old directory, attempting to migrate " + oldAssetBundleFiles.Length + " files now from: " + directoryPath);

                foreach (FileInfo oldAssetBundleFile in oldAssetBundleFiles)
                {
                    foreach (AssetBundleCRCInfo incCrc in m_includedInBuildCrcList)
                    {
                        if (incCrc.m_name == oldAssetBundleFile.Name)
                        {
                            // IF length (quick) matches, move to new directory, else delete.
                            if (oldAssetBundleFile.Length != incCrc.m_fileSizeBytes)
                            {
                                // NW: We assume the CRC is good because file length is good and it's not an incomplete file.
                                // So it must have been working at some point, you'd assume.
                                // We also CRC it later, so all good.
                                DeleteFile(oldAssetBundleFile);
                                break;
                            }

                            // Move the file.
                            Debug.Log("AssetBundleManager::Old file " + oldAssetBundleFile.Name + " MATCHES the name and length of an includedInBuild CRC. Moving the file.");

                            try
                            {
                                File.Move(oldAssetBundleFile.FullName, CurrentVersionDownloadLocation + "/" + oldAssetBundleFile.Name);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("AssetBundleManager::Exception when migrating old file. Most likely a destination file already exists. Harmless to delete it then. " + e);
                            }
                            break;
                        }
                    }
                }

                // Now delete the directory so versioning is complete. All other files and folders are ignored.
                oldDirectory.Delete(true);
            }
        }

        private void DeleteFile(FileInfo oldAssetBundleFile)
        {
            Debug.Log("AssetBundleManager::Old file " + oldAssetBundleFile.Name + " is incompatible or not required. Deleting.");
            oldAssetBundleFile.Delete();
        }

        public void OnGameDBLoaded(Events eventType, object[] args)
        {
            //this can happen more than once? WTF.

            //get platform name and server url
            GetPlatformNameAndBundleURL();

            // This file is generated by PeteP using the magic of Jenkins, hence resources.
            // @Note: Due to the brilliance of TextAsset not being compatible with arbitrary file extensions, we've called it *.crc.txt.
            TextAsset includedInBuildAssetBundlesTextAsset = Resources.Load<TextAsset>("IncludedInBuildAssetBundles.crc");
            m_includedInBuildCrcList = new List<AssetBundleCRCInfo>(30);
            AssetBundleInformationManager.ParseCRCInformation(includedInBuildAssetBundlesTextAsset.text, m_includedInBuildCrcList);

            AttemptUpdateFromPreviousVersion();


            m_assetBundleData = Resources.Load<AssetBundleData>("AssetBundleData");

            m_bundleQueue.Clear();

            foreach (AssetBundleCRCInfo crcInfo in m_includedInBuildCrcList)
            {
                //int index = m_sharkAssetData.BundleMap.FindIndex(x => x.Name == crcInfo.m_name);
                Bundle bundle = Array.Find(m_assetBundleData.Bundles, x => x.assetBundleName == crcInfo.m_name);
                if (bundle != null)
                {
                    m_bundleQueue.Add(crcInfo.m_name, bundle);
                    bundle.Initalise();
                    bundle.OnPriorityChanged += OnBundlePriorityChanged;
                }
                else
                {
                    Debug.LogWarning("Bundle key " + crcInfo.m_name + " not found in bundleMap. This warning can be ignored if the game isn't supposed to be loading this bundle.");
                }
            }

            RevokeOldPermissions();

            // Initialize managers.
            m_assetBundleDownloaderManager = new AssetBundleDownloaderManager();
            m_assetBundleDownloaderManager.DownloadProgressChanged += DownloadProgressChangedHandler;
            m_assetBundleDownloaderManager.AssetBundleStateChanged += AssetBundleStateChangedHandler;
            m_assetBundleDownloaderManager.AssetBundleDownloadCorruptedOrBroken += AssetBundleDownloadCorruptedOrBrokenHandler;

            m_assetBundleInformationManager = new AssetBundleInformationManager(m_includedInBuildCrcList);
            m_assetBundleInformationManager.AssetBundleStateChanged += AssetBundleStateChangedHandler;
            m_assetBundleInformationManager.AssetBundleLocalFileChecked += AssetBundleLocalFileCheckedHandler;
            m_assetBundleInformationManager.AssetBundleFinishedAndRequiresMoving += AssetBundleFinishedAndRequiresMovingHandler;
            m_assetBundleInformationManager.AssetBundleDownloadCorruptedOrBroken += AssetBundleDownloadCorruptedOrBrokenHandler;
            m_assetBundleInformationManager.AssetBundleRevokePermision += AssetBundleRevokePermision;

            EnableDebugger();

            //EventManager<Events>.Instance.DeregisterEvent(Events.OnGameDBLoaded, OnGameDBLoaded);
        }

        private void RevokeOldPermissions()
        {
            //can be removed after 3.6.0
            if (PlayerPrefs.GetString(RevokePermissionOnUpdatePref, "") != Globals.GetApplicationVersion())
            {
                RevokeAllPermissions();
                PlayerPrefs.SetString(RevokePermissionOnUpdatePref, Globals.GetApplicationVersion());
            }
        }

        private void AssetBundleRevokePermision(object sender, AssetBundleEventArgs e)
        {
            if (m_bundleQueue.ContainsKey(e.AssetBundleName))
            {
                Bundle bundle = m_bundleQueue[e.AssetBundleName];
                if (bundle != null)
                {
                    m_permisionToRevokeQueue.Add(bundle);
                }
            }
        }

        private void OnBundlePriorityChanged(DownloadPriority downloadPriority)
        {
            if (downloadPriority == DownloadPriority.Immediate)
            {
                m_hasCurrentPriorityChanged = true;
            }
        }

        public void OnGamePlayStart(Events eventType, object[] arg)
        {
            m_isInGame = true;
        }
        public void OnGamePlayEnd(Events eventType, object[] arg)
        {
            m_isInGame = false;
        }

        public bool CheckAndResetDownloadTimeout()
        {
            bool tmp = m_hasDownloadTimedOut;
            m_hasDownloadTimedOut = false;
            return tmp;
        }

        public bool CheckAndResetCurrentPriorityChanged()
        {
            bool tmp = m_hasCurrentPriorityChanged;
            m_hasCurrentPriorityChanged = false;
            return tmp;
        }

        public void ForceRecheckOfAllAssetBundles()
        {
            foreach (KeyValuePair<string, Bundle> absi in m_bundleQueue)
            {
                m_assetBundleDownloaderManager.ForceRecheckOfBundle(absi.Value);
            }
        }

        public void GetPlatformNameAndBundleURL()
        {

        }

        public void Update()
        {

            UnityEngine.Profiling.Profiler.BeginSample("AssetBundleUpdate");
            if (m_assetBundleInformationManager == null)
            {
                return;
            }
            if (m_sendAssetBundleLoadableEventOnMainThread && !m_isInGame)
            {
                m_sendAssetBundleLoadableEventOnMainThread = false;
                //EventManager<Events>.Instance.TriggerEvent(Events.AssetBundleLoadable);
            }

            // Poll every x seconds.
            m_realtimeToPollAgain -= Time.deltaTime;
            //float time = Time.realtimeSinceStartup;
            if (m_realtimeToPollAgain < 0.0f)
            {
                m_realtimeToPollAgain = PollIntervalSeconds;

                if (m_permisionToRevokeQueue.Count > 0)
                {
                    for (int i = m_permisionToRevokeQueue.Count - 1; i > -1; i--)
                    {
                        m_permisionToRevokeQueue[i].SetPermission(false);
                        Debug.Log("Assetbundle Permissions revoked for " + m_permisionToRevokeQueue[i].assetBundleName);
                        m_permisionToRevokeQueue.RemoveAt(i);
                    }
                }

                //cached for threads
                CurrentReachability = Util.IsInternetReachable();

                if (CurrentReachability != NetworkReachability.NotReachable)
                {

                    if (!m_assetBundleInformationManager.HasLoadedServerCRCInfo)
                    {
                        //check crc info queue
                        m_assetBundleInformationManager.PollUpdate();
                    }
                    else
                    {
                        //poll other bundle states
                        UpdateStateOfBundlesInQueue();


                        foreach (KeyValuePair<string, Bundle> kv in m_bundleQueue)
                        {
                            if (kv.Value.state == AssetBundleDownloadState.CRCCheck)
                            {
                                m_assetBundleInformationManager.CheckLocalAssetBundle(kv.Value.assetBundleName);
                            }

                            //record the loaded state in the player prefs for the UI
                            if (kv.Value.state == AssetBundleDownloadState.Loadable)
                            {
                                if (HasBundleBeenLoadedPreviously(kv.Value.assetBundleName) == false)
                                {
                                    string bundleNameLoadedKey = GetBundleNameLoadedKey(kv.Value.assetBundleName);
                                    PlayerPrefs.SetInt(bundleNameLoadedKey, 1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    m_assetBundleInformationManager.PollUpdate(true);
                    if (m_assetBundleInformationManager.HasLoadedLocalCRCInfo)
                    {
                        UpdateStateOfBundlesInQueue();
                    }


                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public ulong GetFreeDiskSpace()
        {
            ulong freeDiskSpace = NativeBinding.Instance.GetFreeDiskSpace();
#if !PRODUCTION && !PREPRODUCTION
            if (PlayerPrefs.GetInt("DEBUG_SpoofLowSpace", 0) == 1)
            {
                freeDiskSpace = 1024 * 1024 * 5;//mb
            }
#endif
            return freeDiskSpace;
        }

        private void UpdateStateOfBundlesInQueue()
        {
            ulong freeDiskSpace = GetFreeDiskSpace();

            List<Bundle> bundlesSortedByPriority = m_bundleQueue.Values.ToList(); //put to list
            for (int i = bundlesSortedByPriority.Count - 1; i >= 0; i--)
            {
                //get the download priority, cache it and then remove all that cannot be downloaded
                DownloadPriority downloadPriority = bundlesSortedByPriority[i].GetDownloadPriority();
                //any with the minInt value aren't downloadable
                if (downloadPriority == DownloadPriority.DoNotDownload)
                {
                    if (bundlesSortedByPriority[i].state == AssetBundleDownloadState.Queued)
                    {
                        bundlesSortedByPriority[i].OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(bundlesSortedByPriority[i].assetBundleName, AssetBundleDownloadState.WaitingManualPermission, null));
                    }
                    bundlesSortedByPriority.RemoveAt(i);
                    continue;
                }
                //these aren't relevant to state checks
                if (bundlesSortedByPriority[i].state == AssetBundleDownloadState.Loadable
                    || bundlesSortedByPriority[i].state == AssetBundleDownloadState.Initialising
                    || bundlesSortedByPriority[i].state == AssetBundleDownloadState.CRCCheck)
                {
                    bundlesSortedByPriority.RemoveAt(i);
                }
            }

            //sort by priority then progress
            bundlesSortedByPriority.Sort(delegate (Bundle x, Bundle y)
            {
                int value1 = (int)x.GetDownloadPriority();
                int value2 = (int)y.GetDownloadPriority();

                int enumCompare = value1.CompareTo(value2);
                if (enumCompare == 0)
                {
                    //if bundles are of same priority compare progress in reverse so highest is first
                    enumCompare = y.GetProgressPercent().CompareTo(x.GetProgressPercent());
                }
                if (enumCompare == 0)
                {
                    if (x is SceneBundle && y is SceneBundle)
                    {
                        enumCompare = (x as SceneBundle).stateName.CompareTo((y as SceneBundle).stateName);
                    }
                }
                return enumCompare;
            });

            for (int i = 0; i < bundlesSortedByPriority.Count; i++)
            {
                Bundle absi = bundlesSortedByPriority[i];

                // Do we have any queued? Manual is higher priority than auto.
                switch (absi.state)
                {
                    case AssetBundleDownloadState.Downloading:
                        //keep checking download timeout.
                        if (m_downloadTimeoutLastBytes == absi.downloadProgressBytes)
                        {
                            if (m_realtimeTimeoutStarted == 0)
                            {
                                m_realtimeTimeoutStarted = Time.realtimeSinceStartup;
                            }
                            else
                            {
                                if (Time.realtimeSinceStartup > m_realtimeTimeoutStarted + DownloadNoProgressTimeoutSeconds)
                                {
                                    m_hasDownloadTimedOut = true;
                                    m_realtimeTimeoutStarted = 0;
                                }
                            }
                        }
                        else
                        {
                            //update download progress
                            m_downloadTimeoutLastBytes = absi.downloadProgressBytes;
                        }
                        continue;

                    case AssetBundleDownloadState.Blocked:
                        absi.CheckBlockedState();
                        continue;

                    case AssetBundleDownloadState.WaitingManualPermission:
                    case AssetBundleDownloadState.Queued:
                        //continue to checking code
                        break;

                    case AssetBundleDownloadState.NotBundled:
                    default:
                        //unknown or uncared about state, step over
                        continue;
                }


                // Can we? Check disk space.
                bool hasInsufficientDiskSpace = freeDiskSpace <= (ulong)absi.downloadSizeRemainingBytes;
                if (hasInsufficientDiskSpace)
                {
                    Debug.Log("AssetBundleManager::TryKickOffPendingDownload - HasSpaceForAssetBundleDownload - " + absi.assetBundleName + " = false. Blocked. Trying again with another.");
                    absi.OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(absi.assetBundleName, AssetBundleDownloadState.Blocked, ErrorDiskSpaceFull));

                    // Try again with a non-blocked download.
                    // This absi will only unblock when the app restarts.
                    continue;
                }

                // Can we? Other download in progress?
                if (m_assetBundleDownloaderManager.IsDownloading)
                {
                    if (absi.state != AssetBundleDownloadState.Queued)
                    {
                        absi.OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(absi.assetBundleName, AssetBundleDownloadState.Queued));
                    }
                    continue;
                }

                Debug.Log("AssetBundleManager::TryKickOffPendingDownload - DoGameplayRulesAllowAssetBundleDownload - " + absi.assetBundleName + " = yes! Downloading this one and returning.");
                m_assetBundleDownloaderManager.StartDownloadThread(absi);
            }
        }

        public float GetAssetBundlesDownloadSizeMB(Bundle[] assetBundles)
        {
            long bytes = 0;
            for (int i = 0; i < assetBundles.Length; i++)
            {
                bytes += assetBundles[i].downloadSizeBytes;
            }
            float mb = (bytes / 1024f) / 1024f;
            float mbInt = (float)Math.Round(mb * 100f) / 100f; //round to 2sf
            return mbInt;
        }

        public float GetAssetBundlesDownloadProgressMB(Bundle[] assetBundles)
        {
            long bytes = 0;
            for (int i = 0; i < assetBundles.Length; i++)
            {
                bytes += assetBundles[i].downloadProgressBytes;
            }

            float mb = (bytes / 1024f) / 1024f;
            float mbInt = (float)Math.Round(mb * 100f) / 100f; //round to 2sf
            return mbInt;
        }

        public float GetAssetBundlesDownloadRemainingMB(Bundle[] assetBundles)
        {
            long remaining = 0;
            for (int i = 0; i < assetBundles.Length; i++)
            {
                remaining += (assetBundles[i].downloadSizeBytes - assetBundles[i].downloadProgressBytes);
            }
            float mb = (remaining / 1024f) / 1024f;
            float mbInt = (float)Math.Round(mb * 100f) / 100f; //round to 2sf
            return mbInt;
        }

        public float GetAssetBundleDownloadRemainingMB(Bundle assetBundleName)
        {

            long remaining = (assetBundleName.downloadSizeBytes - assetBundleName.downloadProgressBytes);

            float mb = (remaining / 1024f) / 1024f;
            float mbInt = (float)Math.Round(mb * 100f) / 100f; //round to 2sf
            return mbInt;
        }

        public float GetPercentProgressOfDownload(Bundle[] assetBundles)
        {
            long totalSize = 0;
            long totalDownloaded = 0;
            for (int i = 0; i < assetBundles.Length; i++)
            {
                totalDownloaded += assetBundles[i].downloadProgressBytes;
                totalSize += assetBundles[i].downloadSizeBytes;
            }

            if (totalSize == 0)
            {
                return 0.0f;
            }
            else
            {
                float percent = (float)totalDownloaded / (float)totalSize;
                return percent;
            }
        }

        public double GetDownloadSpeedKilobytes()
        {
            double speedkb = CurrentDownloadSpeed / (double)1024;
            return Math.Round(speedkb);
        }

        public AssetBundleDownloadState GetOverallStateOfAssetBundles(Bundle[] assetBundles)
        {
            if (assetBundles != null)
            {
                AssetBundleDownloadState state = AssetBundleDownloadState.Loadable;

                for (int i = 0; i < assetBundles.Length; i++)
                {
                    if (assetBundles[i].state < state)
                    {
                        state = assetBundles[i].state;
                    }
                    else if (assetBundles[i].state == AssetBundleDownloadState.Blocked)
                    {
                        return AssetBundleDownloadState.Blocked;
                    }
                }
                return state;
            }
            return AssetBundleDownloadState.Initialising;
        }

        public bool IsAnyBundleDownloading(Bundle[] assetBundles)
        {
            for (int i = 0; i < assetBundles.Length; i++)
            {
                if (assetBundles[i].state == AssetBundleDownloadState.Downloading)
                {
                    return true;
                }
            }
            return false;
        }

        public string GetAssetBundleError(string assetBundleName)
        {
            if (!m_bundleQueue.ContainsKey(assetBundleName))
            {
                Debug.LogError("AssetBundleManager::GetAssetBundleError. No asset bundle of name \"" + assetBundleName + "\" known.");
                return null;
            }
            return m_bundleQueue[assetBundleName].error;
        }

        //called from onapplicationresume
        //we assume the user could have tampered and deleted all the assetbundles while the app was in the background
        public void RecheckAllAssetBundles()
        {
            Debug.Log("AssetBundleManager::RecheckAllAssetBundles triggered by app resume");
            foreach (KeyValuePair<string, Bundle> kv in m_bundleQueue)
            {
                //user could have tampered with finished bundles.
                if (kv.Value.state == AssetBundleDownloadState.Loadable)
                {
                    m_bundleQueue[kv.Value.assetBundleName].OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(kv.Value.assetBundleName, AssetBundleDownloadState.CRCCheck));
                }
            }
        }

        public bool AreAllBundlesReadyToLoad(Bundle[] bundles)
        {
            for (int i = 0; i < bundles.Length; i++)
            {
                if (bundles[i].state != AssetBundleDownloadState.Loadable)
                {
                    return false;
                }
            }
            return true;
        }

        public void GivePermissionToDownloadBundles(Bundle[] bundles)
        {
            for (int i = 0; i < bundles.Length; i++)
            {
                bundles[i].SetPermission(true);
            }
        }

        //does not happen on main thread
        private void DownloadProgressChangedHandler(object x, Downloader.DownloadProgressChangedEventArgs eventarg)
        {
            //Debug.Log("DownloadProgressHandler - " + eventarg.AssetBundleName + " = " + eventarg.BytesReceived.ToString("N0") + " (speed " + eventarg.CurrentSpeed.ToString("N0") + ")");
            Bundle absi;
            if (!m_bundleQueue.TryGetValue(eventarg.AssetBundleName, out absi))
            {
                Debug.LogWarning("Asset bundle name " + eventarg.AssetBundleName + " not recognised, ignoring");
                return;
            }
            absi.OnDownloadProgressChanged(eventarg);
            CurrentDownloadSpeed = eventarg.CurrentSpeed;
        }
        private void AssetBundleStateChangedHandler(object x, AssetBundleStateChangedEventArgs eventarg)
        {
            Debug.Log("AssetBundleStateChangedHandler - " + eventarg.AssetBundleName + " - " + eventarg.State.ToString());
            if (!m_bundleQueue.ContainsKey(eventarg.AssetBundleName))
            {
                Debug.LogWarning("Asset bundle name " + eventarg.AssetBundleName + " not recognised, ignoring");
                return;
            }
            m_bundleQueue[eventarg.AssetBundleName].OnAssetBundleStateChanged(eventarg);

            m_sendAssetBundleLoadableEventOnMainThread |= eventarg.State == AssetBundleDownloadState.Loadable;
        }
        private void AssetBundleLocalFileCheckedHandler(object x, AssetBundleLocalFileCheckedEventArgs eventarg)
        {
            try
            {

                Debug.Log("AssetBundleLocalFileCheckedHandler - " + eventarg.AssetBundleName);
                if (!m_bundleQueue.ContainsKey(eventarg.AssetBundleName))
                {
                    Debug.LogWarning("Asset bundle name " + eventarg.AssetBundleName + " not in queue, ignoring");
                    return;
                }
                m_bundleQueue[eventarg.AssetBundleName].OnLocalFileChecked(eventarg);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        private void AssetBundleDownloadCorruptedOrBrokenHandler(object x, AssetBundleDeleteAssetBundleFileEventArgs eventarg)
        {
            Debug.Log("AssetBundleDownloadCorruptedOrBrokenHandler - " + eventarg.AssetBundleName);
            if (!m_bundleQueue.ContainsKey(eventarg.AssetBundleName))
            {
                Debug.LogWarning("Asset bundle name " + eventarg.AssetBundleName + " not recognised, ignoring");
                return;
            }
            DeleteAndRestartAssetBundleDownload(eventarg.AssetBundleName, eventarg.Error);
        }
        private void AssetBundleFinishedAndRequiresMovingHandler(object x, AssetBundleEventArgs eventarg)
        {
            Debug.Log("AssetBundleFinishedAndRequiresMovingHandler - " + eventarg.AssetBundleName);
            if (!m_bundleQueue.ContainsKey(eventarg.AssetBundleName))
            {
                Debug.LogWarning("Asset bundle name " + eventarg.AssetBundleName + " not recognised, ignoring");
                return;
            }
            MoveTempDownloadToFinalLocation(eventarg.AssetBundleName);
        }

        //this is a critical method to remove assetbundles in incorrect or malformed states and to reschedule them.
        //if for ANY reason this fails, we can assume the game is unable to get this download during this session. We need to block the download entirely.
        private void DeleteAndRestartAssetBundleDownload(string assetBundleName, string error)
        {
            FileInfo fileInfo = new FileInfo(CurrentVersionDownloadLocation + "/" + assetBundleName);
            FileInfo incompleteFileInfo = new FileInfo(CurrentVersionDownloadLocation + "/" + assetBundleName + ".incomplete");
            try
            {
                if (incompleteFileInfo.Exists)
                {
                    incompleteFileInfo.Delete();
                    incompleteFileInfo.Refresh();
                }
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                    fileInfo.Refresh();
                }


                FileStream fs = incompleteFileInfo.Create(); //create a blank file
                fs.Close();

                //reset progress
                //m_bundleQueue[assetBundleName].OnLocalFileChecked(new AssetBundleLocalFileCheckedEventArgs(assetBundleName, false, 0, m_bundleQueue[assetBundleName].downloadSizeBytes));
                //m_bundleQueue[assetBundleName].OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(assetBundleName, AssetBundleDownloadState.WaitingManualPermission, error));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("AssetBundlerManager DeleteAndRestartAssetBundleDownload: Critical failure in removing download, or writing new file: " + e.ToString());
                //m_bundleQueue[assetBundleName].OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(assetBundleName, AssetBundleDownloadState.Blocked, "FAILED_TO_WRITE_TO_DISK"));
            }
        }
        private void MoveTempDownloadToFinalLocation(string assetBundleName)
        {
            FileInfo assetBundleNameFinalFileData = new FileInfo(CurrentVersionDownloadLocation + "/" + assetBundleName);
            FileInfo assetBundleNameTempFileData = new FileInfo(CurrentVersionDownloadLocation + "/" + assetBundleName + ".incomplete");
            try
            {
                //wipe current file.
                //This CAN FAIL IF THE ASSET BUNDLE IS LOADED.
                //WTF DO WE DO? AAAARGH
                if (assetBundleNameFinalFileData.Exists)
                {
                    assetBundleNameFinalFileData.Delete();
                }
                assetBundleNameTempFileData.MoveTo(assetBundleNameFinalFileData.FullName);
                //m_bundleQueue[assetBundleName].OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(assetBundleName, AssetBundleDownloadState.CRCCheck)); //restart checking logic
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("AssetBundler MoveTempDownloadToFinalLocation: Moving the incomplete download to the final location failed: " + e.ToString());
                //m_bundleQueue[assetBundleName].OnAssetBundleStateChanged(new AssetBundleStateChangedEventArgs(assetBundleName, AssetBundleDownloadState.CRCCheck, "FAILED_TO_WRITE_TO_DISK"));
            }
        }
        private string GetBundleNameLoadedKey(string assetBundleName)
        {
            return "bundleNameLoaded" + assetBundleName;
        }
        public bool HasBundleBeenLoadedPreviously(string assetBundleName)
        {
            string bundleNameLoadedKey = GetBundleNameLoadedKey(assetBundleName);
            return PlayerPrefs.HasKey(bundleNameLoadedKey);
        }
        public void NoInternetAssBunsMessage()
        {
            //MessageBoxPopup.MessageBoxConfig config = new MessageBoxPopup.MessageBoxConfig();

            //config.titleText = "STRING_SHARKTANK_CONNECTION_ERROR_TITLE";
            //config.messageText = "STRING_ASSBUN_ERROR_OFFLINE_DESC";

            //config.onConfirm = null;
            //config.backButtonMode = MessageBoxPopup.MessageBoxConfig.BackButtonMode.confirm;
            //config.cancelEnabled = false;
            //config.modal = true;

            //MessageBoxPopup.OpenMessageBox(config);
        }
        private void InitialisingDownloadMessage()
        {
            //MessageBoxPopup.MessageBoxConfig config = new MessageBoxPopup.MessageBoxConfig();

            //config.titleText = "STRING_SHARKTANK_CONNECTION_ERROR_TITLE";
            //config.messageText = "STRING_ASSBUN_ERROR_SLOWNET_DESC";

            //config.onConfirm = null;
            //config.backButtonMode = MessageBoxPopup.MessageBoxConfig.BackButtonMode.confirm;
            //config.cancelEnabled = false;
            //config.modal = true;

            //MessageBoxPopup.OpenMessageBox(config);
        }


        private Bundle[] GetNonLoadableBundles(Bundle[] assetBundles)
        {

            List<Bundle> results = new List<Bundle>();

            for (int n = 0; n < assetBundles.Length; n++)
            {
                if (assetBundles[n].state != AssetBundleDownloadState.Loadable)
                {
                    results.Add(assetBundles[n]);
                }
            }
            return results.ToArray();
        }


        private void ShowDiskFullMessageBox(Bundle[] bundles)
        {
            //float remainingMB = GetAssetBundlesDownloadRemainingMB(bundles);

            //MessageBoxPopup.MessageBoxConfig diskFullPopup = new MessageBoxPopup.MessageBoxConfig();
            //diskFullPopup.titleText = "STRING_ASSBUN_ERROR_DISKFULL_TITLE";
            //diskFullPopup.messageText = "STRING_ASSBUN_ERROR_DISKFULL_DESC";
            //diskFullPopup.messageArgs = new[] { Mathf.Max(1, remainingMB).ToString("N0") };
            //diskFullPopup.backButtonMode = MessageBoxPopup.MessageBoxConfig.BackButtonMode.confirm;
            //diskFullPopup.onConfirm = () => { };
            //diskFullPopup.cancelEnabled = false;

            //MessageBoxPopup.OpenMessageBox(diskFullPopup);
        }

        private void ShowFileBlockedMessageBox()
        {
            //MessageBoxPopup.MessageBoxConfig blockedPopup = new MessageBoxPopup.MessageBoxConfig();
            //blockedPopup.titleText = "STRING_ASSBUN_ERROR_FAILED_TITLE";
            //blockedPopup.messageText = "STRING_ASSBUN_ERROR_FAILED_DESC";
            //blockedPopup.backButtonMode = MessageBoxPopup.MessageBoxConfig.BackButtonMode.confirm;
            //blockedPopup.onConfirm = () => { };
            //blockedPopup.cancelEnabled = false;

            //MessageBoxPopup.OpenMessageBox(blockedPopup);
        }

        public void ResetPollingTime()
        {
            //forces the next poll
            m_realtimeToPollAgain = 0.1f;
        }

        public void EnableDebugger()
        {
#if !(PRODUCTION || PRE_PRODUCTION)
            if (PlayerPrefs.GetInt("AssetBundleDebug", 0) == 1)
            {
                string m_originPath = "GUI/Debug/PF_AssetBundleDebugInfo";

                GameObject overlay = (GameObject)Resources.Load(m_originPath);
                SoftLiu.Assert.Fatal(overlay != null, "Couldnt load " + m_originPath);
                if (overlay != null)
                {
                    GameObject go = UnityEngine.Object.Instantiate(overlay, Vector3.zero, Quaternion.identity);
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }
            }
#endif
        }

        public void RevokeAllPermissions()
        {
            foreach (KeyValuePair<string, Bundle> item in m_bundleQueue)
            {
                item.Value.SetPermission(false);
            }
        }

        public bool GetAnyBundleRequirePermission()
        {
            foreach (KeyValuePair<string, Bundle> item in m_bundleQueue)
            {
                if (!item.Value.isLoadable && item.Value.GetPermission() == false)
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetAnyBundleRequirePermission(Bundle[] bundles)
        {
            for (int i = 0; i < bundles.Length; i++)
            {
                if (bundles[i].GetPermission() == false)
                {
                    return true;
                }
            }
            return false;
        }

        public float GetTotalBundleAwaitingPermissionDownloadSizeMB()
        {
            float totalSize = 0f;
            foreach (KeyValuePair<string, Bundle> item in m_bundleQueue)
            {
                if (item.Value.state == AssetBundleDownloadState.WaitingManualPermission)
                {
                    totalSize += item.Value.downloadSizeMB;
                }
            }
            return totalSize;
        }

        public Bundle[] GetBundles<T>() where T : Bundle
        {
            List<Bundle> bundles = new List<Bundle>();

            foreach (KeyValuePair<string, Bundle> item in m_bundleQueue)
            {
                if (item.Value is T)
                {
                    bundles.Add(item.Value);
                }
            }

            return bundles.ToArray();
        }

        public Bundle[] GetBundlesInState<T>(AssetBundleDownloadState downloadState) where T : Bundle
        {
            List<Bundle> bundles = new List<Bundle>();

            foreach (KeyValuePair<string, Bundle> item in m_bundleQueue)
            {
                if (item.Value is T && item.Value.state == downloadState)
                {
                    bundles.Add(item.Value);
                }
            }

            return bundles.ToArray();
        }

        public void ShowAssetbundleDownloadMessage(Bundle[] bundles)
        {
            if (CurrentReachability == NetworkReachability.NotReachable)
            {
                AssetBundleManager.Instance.NoInternetAssBunsMessage();
            }
            else
            {
                if (GetAnyBundleRequirePermission(bundles))
                {
                    
                }
                AssetBundlesDownloadRequested(bundles);
            }
        }

        private void AssetBundlesDownloadRequested(Bundle[] bundles)
        {
            AssetBundleDownloadState state = GetOverallStateOfAssetBundles(bundles);
            switch (state)
            {
                case AssetBundleDownloadState.Blocked:
                    ShowBlockedStateMessage(bundles);
                    break;
                case AssetBundleDownloadState.Queued:
                case AssetBundleDownloadState.Downloading:
                case AssetBundleDownloadState.CRCCheck:
                    ShowDownloadingStateMessages(bundles);
                    OnBundlesRequestedByUser(bundles);
                    break;
                default:
                    Debug.LogWarning("User hit play but they are in unhandled asset bundle state. Assuming dodgy internet: bundles are in state: " + state.ToString());
                    InitialisingDownloadMessage();
                    break;
            }
        }

        private void ShowBlockedStateMessage(Bundle[] bundles)
        {
            if (DoesAnyBundleHaveError(bundles, ErrorDiskSpaceFull))
            {
                ShowDiskFullMessageBox(bundles);
            }
            else
            {
                ShowFileBlockedMessageBox();
            }
        }

        private void ShowDownloadingStateMessages(Bundle[] bundles)
        {
            ShowDownloadingMessageBox(bundles);
        }

        private void ShowDownloadingMessageBox(Bundle[] bundles)
        {
            float remainingMB = GetAssetBundlesDownloadRemainingMB(bundles);
            //MessageBoxPopup.MessageBoxConfig downloadingPopup = new MessageBoxPopup.MessageBoxConfig();
            //downloadingPopup.titleText = "STRING_ASSBUN_PROMPT_TITLE";

            //downloadingPopup.messageText = "STRING_ASSBUN_PROMPT_DESC";
            //downloadingPopup.messageArgs = new[] { Mathf.Max(1, remainingMB).ToString("F0") };
            //downloadingPopup.backButtonMode = MessageBoxPopup.MessageBoxConfig.BackButtonMode.confirm;
            //downloadingPopup.onConfirm = () =>
            //{
            //    HSXAnalyticsManager.Instance.AssetBundlesPromptResult(bundles, "continue", "LevelGate");
            //};
            //downloadingPopup.cancelEnabled = false;

            //MessageBoxPopup.OpenMessageBox(downloadingPopup);
        }

        private bool DoesAnyBundleHaveError(Bundle[] Bundles, string error)
        {
            if (Bundles != null)
            {
                for (int i = 0; i < Bundles.Length; i++)
                {
                    if (Bundles[i] != null && !string.IsNullOrEmpty(Bundles[i].error) && Bundles[i].error.Equals(error))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void OnBundlesRequestedByUser(Bundle[] bundles)
        {
            foreach (KeyValuePair<string, Bundle> bundle in m_bundleQueue)
            {
                bundle.Value.ResetDownloadPriority();
            }

            for (int i = 0; i < bundles.Length; i++)
            {
                bundles[i].OnBundleRequestedByUser();
            }
        }

    }
}
