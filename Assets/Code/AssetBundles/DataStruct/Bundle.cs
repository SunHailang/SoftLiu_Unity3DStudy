using SoftLiu.AssetBundles.Downloader;
using SoftLiu.Event;
using SoftLiu.Plugins.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoftLiu.AssetBundles
{
    public class Bundle : ScriptableObject, IBundle
    {
        public delegate void PriorityDelegate(DownloadPriority downloadPriority);
        public event PriorityDelegate OnPriorityChanged;

        public delegate void StateDelegate(AssetBundleDownloadState oldState, AssetBundleDownloadState newState);
        public event StateDelegate OnStateChanged;

        public delegate void PermissionDelegate(bool hasPermission);
        public event PermissionDelegate OnPermissionChanged;

        private AssetBundle m_loadedBundle;
        [SerializeField]
        protected string m_assetBundleName;
        public string assetBundleName { get { return m_assetBundleName; } private set { m_assetBundleName = value; } }

        [SerializeField]
        protected string[] m_assetPaths;
        public string[] assetPaths { get { return m_assetPaths; } private set {; } }

        protected DownloadPriority m_downloadPriority = DownloadPriority.Medium;
        protected DownloadPriority m_overrideDownloadPriority = DownloadPriority.Medium;

        [SerializeField]
        protected AssetBundleDownloadState m_state;
        public AssetBundleDownloadState state { get { return m_state; } private set { m_state = value; } }


        public static long downloadPermissionThreshold = 3145728;
        public long downloadSizeBytes { get; private set; }
        public long downloadProgressBytes { get; private set; }
        public string error { get; private set; }
        public bool isLoadable { get; private set; }
        public int downloadRetries { get; private set; }
        public float realTimeForNextUnblock { get; private set; }
        public long downloadSizeRemainingBytes { get { return downloadSizeBytes - downloadProgressBytes; } }

        private bool m_isLoaded;

        public float downloadSizeMB
        {
            get
            {
                float mb = (downloadSizeBytes / 1024f) / 1024f;
                float mbInt = (float)Math.Round(mb * 100f) / 100f; //round to 2sf
                return mbInt;
            }
        }
        public float downloadSizeRemainingMB
        {
            get
            {
                float mb = (downloadSizeRemainingBytes / 1024f) / 1024f;
                float mbInt = (float)Math.Round(mb * 100f) / 100f; //round to 2sf
                return mbInt;
            }
        }

        protected void InitData(string assetBundleName, string[] assets)
        {
            m_assetBundleName = assetBundleName;
            m_assetPaths = assets;
        }

        public void Initalise()
        {
            state = AssetBundleDownloadState.Initialising;
            m_downloadPriority = DownloadPriority.Medium;
            m_overrideDownloadPriority = DownloadPriority.Medium;
        }

        public bool RequiresPermission()
        {
            return false;
        }

        public void SetPermission(bool hasPermission)
        {
            PlayerPrefs.SetInt("ABPermission_" + m_assetBundleName, hasPermission ? 1 : 0);
            //EventManager<Events>.Instance.TriggerEvent(Events.BundlePermissionsChanged, this);
            if (OnPermissionChanged != null)
            {
                OnPermissionChanged(hasPermission);
            }
        }
        public bool GetPermission()
        {
            return PlayerPrefs.GetInt("ABPermission_" + m_assetBundleName, 0) == 1;
        }

        public double GetProgressPercent()
        {
            if (downloadSizeBytes <= 0)
            {
                //assume it's still paused/waiting/loading
                return 0;
            }
            double progress = (downloadProgressBytes * 100 / downloadSizeBytes);
            return Math.Round(progress);
        }

        public override string ToString()
        {
            return string.Format("\"{0}\" ({1}), DL {3:N0} of {2:N0} bytes, E: \"{4}\", IsLoadable: {5}", assetBundleName, state, downloadSizeBytes, downloadProgressBytes, error, isLoadable);
        }

        public void OnAssetBundleStateChanged(AssetBundleStateChangedEventArgs eventArgs)
        {
            if (state != eventArgs.State && !IsBundleLoaded())
            {

                AssetBundleDownloadState oldState = state;
                if (state == AssetBundleDownloadState.NotBundled)
                {
                    Debug.LogError(String.Format("AssetBundle: {0} is not bundled but it's state is changing to {1}", m_assetBundleName, eventArgs.State));
                }
                //isretry is determined from errors in the state
                Debug.Log("State change: " + this + " > " + eventArgs.State + ", " + eventArgs.Error);
                if (eventArgs.IsRetry)
                {
                    downloadRetries++;
                    Debug.Log("OnAssetBundleStateChanged :: " + assetBundleName + " encountered an error. " + downloadRetries + "/3 errors encountered.");
                }
                if (downloadRetries >= 3)
                {
                    Debug.Log("OnAssetBundleStateChanged :: " + assetBundleName + " is assumed to be blocked after 3 retries.");
                    state = AssetBundleDownloadState.Blocked;
                    error = "Attempted download 3 times, failed each time. Stopping";
                    return;
                }
                state = eventArgs.State;
                error = eventArgs.Error;

                if (OnStateChanged != null)
                {
                    OnStateChanged(oldState, state);
                }
                //EventManager<Events>.Instance.TriggerEvent(Events.BundleStateChanged, this);
                if (state == AssetBundleDownloadState.Loadable)
                {
                    m_overrideDownloadPriority = DownloadPriority.Medium;
                    m_downloadPriority = DownloadPriority.Medium;
                }
            }
        }

        public void CheckBlockedState()
        {
            //if blocked
            if (realTimeForNextUnblock > 0 && downloadRetries >= 1)
            {
                //if it's time to unblock
                if (Time.realtimeSinceStartup > realTimeForNextUnblock)
                {
                    //reduce the number of retries by one
                    downloadRetries--;
                    //put it back into checking state
                    state = AssetBundleDownloadState.CRCCheck;

                    //if still blocked
                    if (downloadRetries >= 1)
                    {
                        //set timer again, bear in mind that this won't update correctly while the app is backgrounded
                        realTimeForNextUnblock = Time.realtimeSinceStartup + AssetBundleManager.Instance.SecondsBeforeBlockedBundleIsRetried;
                    }
                    else
                    {
                        realTimeForNextUnblock = 0;
                    }
                }
            }
            else if (downloadRetries >= 3 && realTimeForNextUnblock == 0)
            {
                realTimeForNextUnblock = Time.realtimeSinceStartup + AssetBundleManager.Instance.SecondsBeforeBlockedBundleIsRetried;
            }
        }

        public virtual DownloadPriority GetDownloadPriority()
        {
            if (RequiresPermission() && !GetPermission())
            {
                return DownloadPriority.DoNotDownload;
            }
            if (m_overrideDownloadPriority != DownloadPriority.Medium)
            {
                return m_overrideDownloadPriority;
            }
            return DownloadPriority.Medium;

        }

        public void OnBundleRequestedByUser()
        {
            if (state != AssetBundleDownloadState.Loadable)
            {
                if (m_overrideDownloadPriority != DownloadPriority.Immediate)
                {
                    m_overrideDownloadPriority = DownloadPriority.Immediate;
                    if (OnPriorityChanged != null)
                    {
                        OnPriorityChanged(m_overrideDownloadPriority);
                    }
                }
            }
        }

        public void ResetDownloadPriority()
        {
            if (state != AssetBundleDownloadState.Loadable)
            {
                if (m_overrideDownloadPriority != DownloadPriority.Medium)
                {
                    m_overrideDownloadPriority = DownloadPriority.Medium;
                    if (OnPriorityChanged != null)
                    {
                        OnPriorityChanged(m_overrideDownloadPriority);
                    }
                }
            }
        }

        public void OnDownloadProgressChanged(DownloadProgressChangedEventArgs eventArgs)
        {
            downloadProgressBytes = eventArgs.BytesReceived;
        }

        public void OnLocalFileChecked(AssetBundleLocalFileCheckedEventArgs eventArgs)
        {
            isLoadable = eventArgs.IsLoadable;
            downloadProgressBytes = eventArgs.BytesDownloaded;
            downloadSizeBytes = eventArgs.FileSizeBytes;
        }


        public Material GetMaterialFromBundle(string assetName)
        {
            Material mat = LoadMaterialFromBundle(assetName.ToLower());
            if (mat == null)
            {
                //if material is null it either isnt bundled or we dont have the bundle yet
                mat = (Material)Resources.Load("Materials/" + assetName);
            }
            return mat;
        }

        private Material LoadMaterialFromBundle(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            if (state == AssetBundleDownloadState.Loadable)
            {
                AssetBundle bundle = GetOrLoadAssetBundle();
                if (bundle == null)
                {
                    return null;
                }
                foreach (string asset in assetPaths)
                {
                    if (Path.GetFileNameWithoutExtension(asset).ToLower().Equals(assetName.ToLower()))
                    {
#if !PRODUCTION
                        Debug.Log(assetBundleName + " Contains: ");
                        string[] assetNames = bundle.GetAllAssetNames();
                        for (int i = 0; i < assetNames.Length; i++)
                        {
                            Debug.Log(assetNames[i]);
                        }
#endif
                        return bundle.LoadAsset<Material>(asset.ToLower());
                    }
                }
                return null;
            }

            return null;

        }

        public Texture GetTextureFromBundle(string assetName)
        {
            Texture tex = LoadTextureFromBundle(assetName.ToLower());
            if (tex == null)
            {
                //if material is null it either isnt bundled or we dont have the bundle yet
                tex = (Texture)Resources.Load("Textures/" + assetName);
            }
            return tex;
        }

        private Texture LoadTextureFromBundle(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            if (state == AssetBundleDownloadState.Loadable)
            {
                AssetBundle bundle = GetOrLoadAssetBundle();
                if (bundle == null)
                {
                    return null;
                }
                foreach (string asset in assetPaths)
                {
                    if (Path.GetFileNameWithoutExtension(asset).ToLower().Equals(assetName.ToLower()))
                    {
#if !PRODUCTION
                        Debug.Log(assetBundleName + " Contains: ");
                        string[] assetNames = bundle.GetAllAssetNames();
                        for (int i = 0; i < assetNames.Length; i++)
                        {
                            Debug.Log(assetNames[i]);
                        }
#endif
                        return bundle.LoadAsset<Texture>(asset.ToLower());
                    }
                }
                return null;
            }

            return null;
        }
        public void AddAsset(string assetPath)
        {
            if (m_assetPaths == null)
            {
                m_assetPaths = new string[] { assetPath };
                return;
            }
            if (!m_assetPaths.Contains(assetPath))
            {
                m_assetPaths = m_assetPaths.Concat(new string[] { assetPath }).ToArray();
            }
        }

        public void ClearAssetPaths()
        {
            m_assetPaths = null;
        }

        public bool HasAsset(string asset)
        {
            for (int i = 0; i < m_assetPaths.Length; i++)
            {
                if (Path.GetFileNameWithoutExtension(m_assetPaths[i]).Equals(asset))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void EnableBundle() { }

        public virtual void DisableBundle() { }


        public AssetBundle GetOrLoadAssetBundle()
        {
            Debug.Log("GetOrLoadAssetBundle(" + assetBundleName + ")");

            if (state == AssetBundleDownloadState.Loadable)
            {

                if (IsBundleLoaded())
                {
                    return m_loadedBundle;
                }

#if !PRODUCTION
                Stopwatch sw = Stopwatch.StartNew();
#endif
                string fullPath = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/" + assetBundleName;
                Debug.Log("AssetBundleLoader | Loading asset bundles async: " + fullPath);
                try
                {
                    if (!File.Exists(fullPath))
                    {
                        Debug.LogError("AssetBundleLoader | File does not exist: " + fullPath);
                    }
                    else
                    {
                        m_loadedBundle = AssetBundle.LoadFromFile(fullPath);
                        m_isLoaded = true;
                        Debug.Log("AssetBundleLoader | Loaded asset bundle! " + assetBundleName);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("AssetBundleLoader | File could not be loaded " + fullPath + ". Exception : " + e.ToString());
                    m_isLoaded = false;
                }

                if (!IsBundleLoaded())
                {
                    Debug.LogError("AssetBundleLoader | Load Fail: " + assetBundleName);
                }

#if !PRODUCTION
                sw.Stop();
                Debug.LogWarning("AssetBundleLoad " + assetBundleName + ":" + sw.ElapsedMilliseconds + "ms");
#endif
                return m_loadedBundle;
            }
            return null;

        }

        public void UnloadAssetBundle()
        {
            Debug.Log("UnloadAssetBundle(" + assetBundleName + ")");
            if (IsBundleLoaded())
            {
                m_loadedBundle.Unload(true);
                m_loadedBundle = null;
                m_isLoaded = false;
            }
        }

        public bool IsBundleLoaded()
        {
            return m_isLoaded;
        }

        public bool LoadSceneFromBundle(string sceneName, LoadSceneMode mode)
        {
            Debug.Log("AssetBundleLoader | LoadSceneAdditiveFromSceneBundle");
            AssetBundle bundle = GetOrLoadAssetBundle();
            if (bundle == null)
            {
                return false;
            }
            string[] scenes = bundle.GetAllScenePaths();
            foreach (string scene in scenes)
            {
                if (scene.Contains(sceneName))
                {
                    SceneManager.LoadScene(scene, mode);
                    return true;
                }
            }
            Debug.LogError("AssetBundleLoader | Assetbundle scene \"" + sceneName + "\" was not found in bundle \"" + assetBundleName + "\"");
            return false;

        }

        //ScriptableObject Functions

        private void OnEnable()
        {
            state = AssetBundleDownloadState.NotBundled;
        }

        public void DeleteBundle()
        {
            UnloadAssetBundle();
            string path = NativeBinding.Instance.GetPersistentDataPath() + "/ABDL/" + Application.version + "/" + assetBundleName;
            try
            {
                File.Delete(path);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
        }

        public virtual string GetContentString()
        {
            return "";
        }
    }
}
