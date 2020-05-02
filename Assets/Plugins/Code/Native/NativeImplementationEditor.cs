using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Plugins.Native
{
    public class NativeImplementationEditor : INativeImplementation
    {
        private string m_generatedUserId = null;
        private string m_generatedAdvertId = null;

        public NativeImplementationEditor()
        {
            m_generatedUserId = "USER_UNITY_EDITOR_" + Guid.NewGuid().ToString();
            m_generatedAdvertId = Guid.NewGuid().ToString();
        }


        public float GetOSVersion()
        {
            float outVersion = 2017.1f;
            float.TryParse(Application.unityVersion, out outVersion);
            return outVersion;
        }

        public string GetBundleVersion()
        {
#if UNITY_EDITOR
            return PlayerSettings.bundleVersion;
#endif
        }

        public string GetUniqueUserID()
        {
            return m_generatedUserId;
        }


        public string GetGameLanguageISO()
        {
            return "zh-CN";
        }

        public string GetUserCountryISO()
        {
            return "CHINA";
        }

        public string GetConnectionType()
        {
            return "Wifi";
        }

        public string GetDeviceName()
        {
            return "EDITOR";
        }


        public string GetAdvertisingIdentifier()
        {
            return m_generatedAdvertId;
        }

        public void DontBackupDirectory(string directory)
        {
        }

        public string GetPersistentDataPath()
        {
            return Application.persistentDataPath;
        }

        public string GetExpansionFilePath()
        {
            return "";
        }

        public bool HasNotch()
        {
            return true;
        }
        public int[] GetNotchSize()
        {
            return new int[] { 70, 120 };
        }

        public void ShowMessageBox(string title, string message, int msg_id = -1)
        {
#if UNITY_EDITOR
            bool result = UnityEditor.EditorUtility.DisplayDialog(title, message, "OK");

            if (msg_id != -1)
            {
                //SoftNativeReceiver.Instance.MessageBoxClick(string.Format("{0}:{1}", msg_id, result ? "OK" : "CANCEL"));
            }
#endif
        }

        public void ShowMessageBoxWithButtons(string title, string message, string ok_button, string cancel_button, int msg_id = -1)
        {
#if UNITY_EDITOR
            bool result = UnityEditor.EditorUtility.DisplayDialog(title, message, ok_button, cancel_button);

            if (msg_id != -1)
            {
                //FGOLNativeReceiver.Instance.MessageBoxClick(string.Format("{0}:{1}", msg_id, result ? "OK" : "CANCEL"));
            }
#endif
        }

        public int GetMemoryUsage()
        {
            return 0;
        }

        public long GetMaxHeapMemory()
        {
            return -1;
        }

        public long GetUsedHeapMemory()
        {
            return -1;
        }

        public long GetMaxDeviceMemory()
        {
            return -1;
        }

        public long GetAvailableDeviceMemory()
        {
            return -1;
        }

        public long GetDeviceMemoryThreshold()
        {
            return -1;
        }

        public int GetMemoryPSS()
        {
            return -1;
        }

        public bool IsAppInstalled(string bundleID)
        {
            return false;
        }

        public bool IsAudioPlayingFromOtherApps()
        {
            return false;
        }

        public bool IsPictureInPictureVideoPlaying()
        {
            return false;
        }

        public void RequestExclusiveAudio(bool exclusiveAudio)
        {
        }

        public void TryShowPermissionExplanation(string[] permissions, string messageTitle, string messageInfo)
        {

        }

        public bool HasPermissions(string[] permissions)
        {
            return true;
        }

        public void ToggleSpinner(bool enable, float x = 0, float y = 0)
        {

        }

        public string HashedValueForAccountName(string userAccountName)
        {
            return userAccountName;
        }

        public string[] GetCertificateSignatureSHA()
        {
            //only used by Android
            return new string[0];
        }


        public string GetDocumentsDirectory()
        {
            return null;
        }

        public ulong GetFreeDiskSpace()
        {
            return 524288000; // hardcoded 500MB
        }

        public bool IsPushDisabledBySystemWideSettings()
        {
            return false;
        }

        public string GetInstalledAppVersion(string appID)
        {
            return null;
        }

        public bool IsGuidedAccessEnabled()
        {
            return false;
        }

        public bool IsTVDevice()
        {
#if TV
            return true;
#else
            return false;
#endif
        }

        public bool ShowNativeAppRatingDialogue()
        {
            return false;
        }

        public void LoadCredentialsFromDevice()
        {
            //do nada
        }

        public void BackUpSetAuthCredentials(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public string BackupGetAuthCredentials(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public string GetCloudUbiquityIdentityToken()
        {
            return PlayerPrefs.GetString("current_cloud_token");
        }

        public void ShowAppSettings()
        {
        }

        public string GetCellularNetworkType()
        {
            return "NOT_AVAILABLE_IN_EDITOR";
        }

        public string GetCellularProviderName()
        {
            return "NOT_AVAILABLE_IN_EDITOR";
        }

        public string GetCellularCountryCode()
        {
            return "NOT_AVAILABLE_IN_EDITOR";
        }

        public string GetExpansionFileCRC()
        {
            return null;
        }
    }
}
