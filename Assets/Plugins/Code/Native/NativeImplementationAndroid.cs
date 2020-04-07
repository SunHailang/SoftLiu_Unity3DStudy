using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Plugins.Native
{
    public class NativeImplementationAndroid : INativeImplementation
    {
        private AndroidJavaObject m_javaActivity;
        private AndroidJavaObject m_javaNative;

        public NativeImplementationAndroid()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (jc == null)
            {
                Debug.LogError("Could not find class com.unity3d.player.UnityPlayer!");
            }

            // find the plugin instance
            m_javaActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            if (m_javaActivity == null)
            {
                Debug.LogError("Could not find currentActivity!");
            }

            using (var nativeClass = new AndroidJavaClass("com.softliu.hlsun.SoftLiuNative"))
            {
                m_javaNative = nativeClass.CallStatic<AndroidJavaObject>("Init", m_javaActivity);
            }
        }

        public string BackupGetAuthCredentials(string key)
        {
            throw new NotImplementedException();
        }

        public void BackUpSetAuthCredentials(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void DontBackupDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public string GetAdvertisingIdentifier()
        {
            return m_javaNative.Call<string>("GetAdvertisingIdentifier");
        }

        public long GetAvailableDeviceMemory()
        {
            return m_javaNative.Call<long>("GetAvailableDeviceMemory");
        }

        public string GetBundleVersion()
        {
            return m_javaNative.Call<string>("GetBundleVersion");
        }

        public string GetCellularCountryCode()
        {
            return m_javaNative.Call<string>("GetCellularCountryCode");
        }

        public string GetCellularNetworkType()
        {
            throw new NotImplementedException();
        }

        public string GetCellularProviderName()
        {
            throw new NotImplementedException();
        }

        public string[] GetCertificateSignatureSHA()
        {
            throw new NotImplementedException();
        }

        public string GetCloudUbiquityIdentityToken()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionType()
        {
            return m_javaNative.CallStatic<string>("GetConnectionType");
        }

        public long GetDeviceMemoryThreshold()
        {
            return m_javaNative.Call<long>("GetDeviceMemoryThreshold");
        }

        public string GetDeviceName()
        {
            throw new NotImplementedException();
        }

        public string GetDocumentsDirectory()
        {
            return m_javaNative.Call<string>("GetDocumentsDirectory");
        }

        public string GetExpansionFileCRC()
        {
            return m_javaNative.CallStatic<string>("GetExtensionFileCRC");
        }

        public string GetExpansionFilePath()
        {
            throw new NotImplementedException();
        }

        public ulong GetFreeDiskSpace()
        {
            throw new NotImplementedException();
        }

        public string GetGameLanguageISO()
        {
            throw new NotImplementedException();
        }

        public string GetInstalledAppVersion(string appID)
        {
            throw new NotImplementedException();
        }

        public long GetMaxDeviceMemory()
        {
            throw new NotImplementedException();
        }

        public long GetMaxHeapMemory()
        {
            throw new NotImplementedException();
        }

        public int GetMemoryPSS()
        {
            throw new NotImplementedException();
        }

        public int GetMemoryUsage()
        {
            throw new NotImplementedException();
        }

        public float GetOSVersion()
        {
            try
            {
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    return version.GetStatic<int>("SDK_INT");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Could not get android GetOSVersion: " + e.ToString());
            }
            return 0;
        }
        private string m_cachedDataPath;
        public string GetPersistentDataPath()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return Application.persistentDataPath;
            }
            else
            {
                if (string.IsNullOrEmpty(m_cachedDataPath))
                {
                    m_cachedDataPath = m_javaNative.CallStatic<string>("GetExternalStorageLocation");

                    // In case the path cannot be retrieved from Android, use Unity's one
                    if (string.IsNullOrEmpty(m_cachedDataPath))
                    {
                        m_cachedDataPath = Application.persistentDataPath;
                    }
                }

                return m_cachedDataPath;
            }
        }

        public string GetUniqueUserID()
        {
            return m_javaNative.Call<string>("GetAndroidID");
        }

        public long GetUsedHeapMemory()
        {
            return m_javaNative.Call<long>("GetUsedHeapMemory");
        }

        public string GetUserCountryISO()
        {
            throw new NotImplementedException();
        }

        public string HashedValueForAccountName(string userAccountName)
        {
            throw new NotImplementedException();
        }

        public bool HasPermissions(string[] permissions)
        {
            throw new NotImplementedException();
        }

        public bool IsAppInstalled(string identifier)
        {
            throw new NotImplementedException();
        }

        public bool IsAudioPlayingFromOtherApps()
        {
            throw new NotImplementedException();
        }

        public bool IsGuidedAccessEnabled()
        {
            throw new NotImplementedException();
        }

        public bool IsPictureInPictureVideoPlaying()
        {
            throw new NotImplementedException();
        }

        public bool IsPushDisabledBySystemWideSettings()
        {
            throw new NotImplementedException();
        }

        public bool IsTVDevice()
        {
            return m_javaNative.CallStatic<bool>("IsAndroidTVDevice");
        }

        public void LoadCredentialsFromDevice()
        {
            throw new NotImplementedException();
        }

        public void RequestExclusiveAudio(bool exclusiveAudio)
        {
            throw new NotImplementedException();
        }

        public void ShowAppSettings()
        {
            throw new NotImplementedException();
        }

        public void ShowMessageBox(string title, string message, int msg_id = -1)
        {
            throw new NotImplementedException();
        }

        public void ShowMessageBoxWithButtons(string title, string message, string ok_button, string cancel_button, int msg_id = -1)
        {
            throw new NotImplementedException();
        }

        public bool ShowNativeAppRatingDialogue()
        {
            throw new NotImplementedException();
        }

        public void ToggleSpinner(bool enable, float x = 1, float y = 0.99F)
        {
            m_javaNative.Call("ToggleSpinner", enable, x, y);
        }

        public void TryShowPermissionExplanation(string[] permissions, string messageTitle, string messageInfo)
        {
            throw new NotImplementedException();
        }
    }
}
