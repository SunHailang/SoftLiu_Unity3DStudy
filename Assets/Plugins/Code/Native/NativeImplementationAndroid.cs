using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Plugins.Native
{
    public class NativeImplementationAndroid : INativeImplementation
    {
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
            throw new NotImplementedException();
        }

        public long GetAvailableDeviceMemory()
        {
            throw new NotImplementedException();
        }

        public string GetBundleVersion()
        {
            throw new NotImplementedException();
        }

        public string GetCellularCountryCode()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public long GetDeviceMemoryThreshold()
        {
            throw new NotImplementedException();
        }

        public string GetDeviceName()
        {
            throw new NotImplementedException();
        }

        public string GetDocumentsDirectory()
        {
            throw new NotImplementedException();
        }

        public string GetExpansionFileCRC()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string GetPersistentDataPath()
        {
            throw new NotImplementedException();
        }

        public string GetUniqueUserID()
        {
            throw new NotImplementedException();
        }

        public long GetUsedHeapMemory()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void TryShowPermissionExplanation(string[] permissions, string messageTitle, string messageInfo)
        {
            throw new NotImplementedException();
        }
    }
}
