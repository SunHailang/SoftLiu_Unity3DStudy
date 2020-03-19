using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    public interface IBundle
    {
        string assetBundleName { get; }
        string[] assetPaths { get; }
        AssetBundleDownloadState state { get; }
        long downloadSizeBytes { get; }
        long downloadProgressBytes { get; }
        string error { get; }
        bool isLoadable { get; }
        int downloadRetries { get; }
        float realTimeForNextUnblock { get; }
        long downloadSizeRemainingBytes { get; }
        float downloadSizeRemainingMB { get; }
        void SetPermission(bool has4GPermission);
        void EnableBundle();
        void DisableBundle();
        double GetProgressPercent();
        string GetContentString();
    }
}
