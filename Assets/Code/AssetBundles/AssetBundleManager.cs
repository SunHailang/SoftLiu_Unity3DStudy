using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    public enum AssetBundleDownloadState
    {
        //ORDERED FROM START TO FINISH. This is used as an int comparison. :D
        Initialising = 0,
        WaitingManualPermission = 1, // IF auto downloads are OFF, we require permission from the user PER BUNDLE. This state is waiting for that permission.
        Queued = 2, // AFTER manual permission, now we're queued for download (waiting for prev to finish)
                    // IF auto downloads are ON, WaitingManualPermission jumps to this.
        Downloading = 3,
        CRCCheck = 4,
        Loadable = 100,
        Blocked = 998, //file is damaged and unwritable, or disk space low. Stuff we can't deal with.
        NotBundled = 999//file is not bundled in this build
    };

    public class AssetBundleManager : MonoBehaviour
    {
        private static IAssetBundleManager m_instance;
        public static IAssetBundleManager Instance
        {
            get
            {
                if (m_instance == null)
                {
#if (ASSETBUNDLES_LEVELS) && (UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR)
                    m_instance = new AssetBundleManagerMobile();
#else
                    m_instance = new AssetBundleManagerNull();
#endif
                }
                return m_instance;
            }
        }

        private void Awake()
        {
            Instance.Awake();
        }
        private void Start()
        {
            Instance.Start();
        }
        private void Update()
        {
            Instance.Update();
        }
        private void OnApplicationQuit()
        {
            Instance.OnApplicationQuit();
        }
        private void OnApplicationPause(bool pause)
        {
            Instance.OnApplicationPause(pause);
        }
        private void OnDestroy()
        {
            Instance.OnDestroy();
        }
    }
}
