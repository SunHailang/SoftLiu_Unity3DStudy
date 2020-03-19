using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles.Downloader
{
    public class AssetBundleEventArgs : EventArgs
    {
        public AssetBundleEventArgs() { }
        public AssetBundleEventArgs(string assetBundleName)
        {
            AssetBundleName = assetBundleName;
        }
        public string AssetBundleName { get; protected set; }
    }

    public class AssetBundleStateChangedEventArgs : AssetBundleEventArgs
    {
        public AssetBundleStateChangedEventArgs(string assetBundleName, AssetBundleDownloadState state, string error = null)
        {
            AssetBundleName = assetBundleName;
            State = state;
            Error = error;
            IsRetry = error != null;
        }
        public string Error { get; private set; }
        public AssetBundleDownloadState State { get; private set; }
        public bool IsRetry { get; private set; }
    }

    public class AssetBundleDeleteAssetBundleFileEventArgs : AssetBundleEventArgs
    {
        public AssetBundleDeleteAssetBundleFileEventArgs(string assetBundleName, string error)
        {
            AssetBundleName = assetBundleName;
            Error = error;
        }
        public string Error { get; private set; }
    }

    public class DownloadProgressChangedEventArgs : AssetBundleEventArgs
    {
        public DownloadProgressChangedEventArgs(string assetBundleName, long totalReceived, long currentSpeed)
        {
            AssetBundleName = assetBundleName;
            BytesReceived = totalReceived;
            CurrentSpeed = currentSpeed;
        }
        public long BytesReceived { get; private set; }
        public long CurrentSpeed { get; private set; }
    }

    public class AssetBundleLocalFileCheckedEventArgs : AssetBundleEventArgs
    {
        public AssetBundleLocalFileCheckedEventArgs(string assetBundleName, bool isLoadable, long bytesDownloaded, long fileSizeBytes)
        {
            AssetBundleName = assetBundleName;
            BytesDownloaded = bytesDownloaded;
            IsLoadable = isLoadable;
            FileSizeBytes = fileSizeBytes;
        }
        public bool IsLoadable { get; private set; }
        public long BytesDownloaded { get; private set; }
        public long FileSizeBytes { get; private set; }

    }

    public class AssetBundleCRCInfoReceivedEventArgs : AssetBundleEventArgs
    {
        public AssetBundleCRCInfoReceivedEventArgs(string assetBundleName, string crcFileData)
        {
            AssetBundleName = assetBundleName;
            CRCFileData = crcFileData;
        }
        public string CRCFileData { get; private set; }
    }
}
