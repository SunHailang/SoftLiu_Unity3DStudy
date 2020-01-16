using System;
using UnityEngine;

public static class Globals
{
#if !PRODUCTION
    private static string s_debugOverrideVersion = null;

    public static string debugOverrideVersion
    {
        get
        {

            return s_debugOverrideVersion;
        }
        set { s_debugOverrideVersion = value; }
    }
#endif

    public enum Environment
    {
        development,
        preproduction,
        production
    }

    public enum Platform
    {
        iOS,
        Android,
        Amazon,
        Unknown
    }

    public static Environment GetEnvironment()
    {
        Environment environment = Environment.development;

#if PREPRODUCTION || MARKETING_BUILD
		environment = Environment.preproduction;
#elif PRODUCTION
		environment = Environment.production;
#endif

        return environment;
    }

    public static Platform GetPlatform()
    {
        Platform platform = Platform.Unknown;

#if UNITY_IPHONE || UNITY_TVOS
        platform = Platform.iOS;
#elif UNITY_ANDROID
#if AMAZON
        platform = Platform.Amazon;
#else
        platform = Platform.Android;
#endif
#elif !UNITY_EDITOR
        Debug.LogWarning("HSXServer :: Unknown platform being played: " + Application.platform.ToString());
#endif

        return platform;
    }

    public static string GetApplicationVersion(bool useOverrideVersion = false)
    {
        string version = "1.0.1";/*FGOL.Plugins.Native.NativeBinding.Instance.GetBundleVersion()*/;

#if !PRODUCTION
        if (string.IsNullOrEmpty(s_debugOverrideVersion) && useOverrideVersion == true)
        {
            s_debugOverrideVersion = PlayerPrefs.GetString("OverrideVersion");
        }


        if (!string.IsNullOrEmpty(s_debugOverrideVersion))
        {
            version = s_debugOverrideVersion;
        }
#endif

        return version;
    }

    public static int GetUnixTimestamp()
    {
        return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public static DateTime GetDateFromUnixTimestamp(int timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
    }
}
