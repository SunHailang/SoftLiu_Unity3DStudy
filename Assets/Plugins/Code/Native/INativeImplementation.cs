using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoftLiu.Plugins.Native
{
    public interface INativeImplementation
    {
        //  App identity
        string GetBundleVersion();
        string GetUniqueUserID();
        string GetAdvertisingIdentifier();
        string GetGameLanguageISO(); //following ISO 639-1 norm
        string GetUserCountryISO(); ////following ISO 3166-1 alpha-2 norm
        string GetConnectionType(); //[3G | 4G | Edge | Wifi ]
        string GetDeviceName();
        float GetOSVersion();
        void DontBackupDirectory(string directory);

        string GetPersistentDataPath();
        string GetExpansionFilePath();
        string[] GetCertificateSignatureSHA(); //used by Android

        //	Audio & multitasking
        bool IsAudioPlayingFromOtherApps();
        bool IsPictureInPictureVideoPlaying();
        void RequestExclusiveAudio(bool exclusiveAudio);

        // General info
        int GetMemoryUsage();
        long GetMaxHeapMemory();
        long GetUsedHeapMemory();
        long GetMaxDeviceMemory();
        long GetAvailableDeviceMemory();
        long GetDeviceMemoryThreshold();
        int GetMemoryPSS();						// Alternative method to GetMemoryUsage
        ulong GetFreeDiskSpace();

        bool IsTVDevice();

        bool IsAppInstalled(string identifier);

        //  Alerts
        void ShowMessageBox(string title, string message, int msg_id = -1);
        void ShowMessageBoxWithButtons(string title, string message, string ok_button, string cancel_button, int msg_id = -1);

        // Permissions
        void TryShowPermissionExplanation(string[] permissions, string messageTitle, string messageInfo);
        bool HasPermissions(string[] permissions);

        // Native visuals
        void ToggleSpinner(bool enable, float x = 1.0f, float y = 0.99f);

        //	Used only on iOS
        //	https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/StoreKitGuide/Chapters/RequestPayment.html#//apple_ref/doc/uid/TP40008267-CH4-SW6
        string HashedValueForAccountName(string userAccountName);

        //  Used only on Android
        string GetDocumentsDirectory();

        //	Returns true if push notifications have been disabled by system settings
        bool IsPushDisabledBySystemWideSettings();

        //  Android only (used for FB version checking), returns NULL if app version could not be detected
        string GetInstalledAppVersion(string appID);

        //	iOS Only, returns true if iOS has guided access enabled. Returns false on every other platform.
        bool IsGuidedAccessEnabled();

        bool ShowNativeAppRatingDialogue(); //returns true if it can show a native app rating dialogue (and shows it), otherwise returns false

        void LoadCredentialsFromDevice(); //ios only, really

        //KEY IS IGNORED ON ANDROID
        //ALL KEYS ARE COMPILED IN THE FGOLNATIVEANDROID.JAR
        void BackUpSetAuthCredentials(string key, string value);

        //KEY IS IGNORED ON ANDROID
        //ALL KEYS ARE COMPILED IN THE FGOLNATIVEANDROID.JAR
        string BackupGetAuthCredentials(string key);

        string GetCloudUbiquityIdentityToken();

        /// <summary>
        /// Opens applications settings (native) so player can select settings like permissions and cache.
        /// </summary>
        void ShowAppSettings();

        //	Returns type of cellular network used
        string GetCellularNetworkType();

        //	Return name of cellular provider
        string GetCellularProviderName();

        //	Returns cellular country code
        string GetCellularCountryCode();

        //  Gets OBB CRC (Android only)
        string GetExpansionFileCRC();
    }
}
