using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

namespace SoftLiu.AssetBundles.Downloader
{
    public enum DownloadPriority
    {
        Immediate = 0, //can interupt current download
        High = 200,
        Medium = 400,
        Low = 600,
        DoNotDownload = 800

    }
    public class AssetBundleDownloaderManager
    {
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public event EventHandler<AssetBundleStateChangedEventArgs> AssetBundleStateChanged;
        public event EventHandler<AssetBundleDeleteAssetBundleFileEventArgs> AssetBundleDownloadCorruptedOrBroken;

        private bool m_isApplicationQuitting = false;
        public bool IsDownloading { get { return m_downloadThread != null && m_downloadThread.IsAlive; } }
        private Thread m_downloadThread = null;
        public int DownloadRateKBS { get; private set; }
        public string DownloadLocation { get; private set; }
        public long CurrentDownloadSpeed { get; private set; }
        public string DownloadThreadStatus { get { return Util.GetThreadStatus(m_downloadThread); } }


        public void ForceRecheckOfBundle(Bundle assetBundleQueueInfo)
        {
            AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(assetBundleQueueInfo.assetBundleName, AssetBundleDownloadState.CRCCheck));
        }
        public bool ShouldDownloadWithCurrentConnection(Bundle assetBundleQueueInfo)
        {
            // Wifi = Always yes.
            if (AssetBundleManager.Instance.CurrentReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                return true;
            }
            else if (AssetBundleManager.Instance.CurrentReachability == NetworkReachability.ReachableViaCarrierDataNetwork
                && assetBundleQueueInfo.GetPermission())
            {
                return true;
            }
            return false;
        }

        public void StartDownloadThread(Bundle bundleInfo)
        {
            if (m_isApplicationQuitting)
            {
                return;
            }
            if (!ShouldDownloadWithCurrentConnection(bundleInfo))
            {
                return;
            }

            if (m_downloadThread != null && m_downloadThread.IsAlive)
            {
                return;
            }

            if (bundleInfo != null)
            {
                Debug.Log("AssetBundleDownloadManager. Starting Download: " + bundleInfo.assetBundleName);
                m_downloadThread = new Thread(() => DoDownload(bundleInfo, AssetBundleManager.Instance.CurrentReachability));
                m_downloadThread.Start();
                AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(bundleInfo.assetBundleName, AssetBundleDownloadState.Downloading));
            }
        }

        public void ApplicationQuitKillDownloadThreads()
        {
            m_isApplicationQuitting = true;
            if (m_downloadThread != null && m_downloadThread.IsAlive)
            {
                m_downloadThread.Abort();
            }
            m_downloadThread = null;
        }

        private void DoDownload(Bundle BundleInfo, NetworkReachability reachability)
        {
            Debug.Log("Download Thread Started: " + BundleInfo.assetBundleName);

            NetworkReachability startingReachability = reachability;
            string downloadLocation = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/" + BundleInfo.assetBundleName;
            FileInfo tempFileInfo = new FileInfo(downloadLocation + ".incomplete");
            FileStream saveFileStream = null;
            try
            {
                Debug.Log("AssetBundler DoDownload " + BundleInfo.assetBundleName + " to " + AssetBundleManager.Instance.CurrentVersionDownloadLocation);
                CreateDownloadDirectory();
                long existingLength = 0;
                if (tempFileInfo.Exists)
                {
                    existingLength = tempFileInfo.Length;
                }
                else
                {
                    //making file incomplete, to mark it for future automatic download
                    try
                    {
                        FileStream fs = tempFileInfo.Create();
                        fs.Close();
                    }
                    catch (Exception e)
                    {
                        //if we can't make a temp file, we quit. go away
                        Debug.LogException(e);
                        Debug.LogError("AssetBundler DoDownload: Critical failure in creating temp file: " + e.ToString());
                        AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.Blocked, "FAILED_TO_WRITE_TO_DISK"));
                        return;
                    }
                }
                Debug.Log("AssetBundler DoDownload: Resuming incomplete DL. " + existingLength + " bytes downloaded already.");

                string downloadURL = AssetBundleManager.Instance.BundleServerURL + BundleInfo.assetBundleName;
                Debug.Log("AssetBundler DoDownload: " + downloadURL);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(downloadURL);
                request.Timeout = 5000; //5 secs timeout
                request.ReadWriteTimeout = 5000;
                request.AddRange((int)existingLength, (int)BundleInfo.downloadSizeBytes);
                request.KeepAlive = false;

                Debug.Log("Request URI: " + request.RequestUri.AbsoluteUri);
                bool didComplete = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.ContentLength == 0)
                    {
                        //no more to download from the server
                        if (existingLength >= BundleInfo.downloadSizeBytes)
                        {
                            didComplete = true;
                        }
                        else
                        {
                            throw new Exception("Error. Data on server not sufficient: " + existingLength + " bytes available, expected " + BundleInfo.downloadSizeBytes);
                        }
                    }

                    long serverFileSize = existingLength + response.ContentLength; //response.ContentLength gives me the size that is remaining to be downloaded

                    bool downloadResumable; // need it for not sending any progress
                    int responseCode = (int)response.StatusCode;
                    downloadResumable = CheckForResumeableReponseCode(BundleInfo, downloadURL, responseCode);
                    if (!downloadResumable)
                    {
                        existingLength = 0;
                    }

                    DownloadProgressChanged.Invoke(this, new DownloadProgressChangedEventArgs(BundleInfo.assetBundleName, existingLength, 0));

                    using (saveFileStream = tempFileInfo.Open(downloadResumable ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            stream.ReadTimeout = 5000;

                            byte[] downBuffer = new byte[4096];
                            long totalReceived = existingLength;
                            long sessionReceived = 0;
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            while (totalReceived < serverFileSize)
                            {

#if !PRODUCTION && !PREPRODUCTION
                                bool paused = false;// Assets.Code.Common.DebugOptions.GetToggleValue(Assets.Code.Common.DebugToggle.downloadsPaused);
                                if (paused)
                                {
                                    Thread.Sleep(100);
                                    continue;
                                }
                                DebugNoInterentConnection();

                                DebugThrottleConnectionSpeed();
#endif

                                //if connection has downgraded to a non-allowed network,
                                //or if user has changed the current prioritised download, this pauses the download
                                if (startingReachability != AssetBundleManager.Instance.CurrentReachability
                                    || AssetBundleManager.Instance.CheckAndResetCurrentPriorityChanged()
                                    || AssetBundleManager.Instance.CheckAndResetDownloadTimeout())
                                {
                                    AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.Queued));
                                    AssetBundleManager.Instance.ResetPollingTime();  //we reset the poll to start the next download immediately
                                    stopwatch.Stop();
                                    stream.Close();
                                    response.Close();
                                    saveFileStream.Close();
                                    saveFileStream = null;
                                    return;
                                }

                                int byteSize = stream.Read(downBuffer, 0, downBuffer.Length);
                                saveFileStream.Write(downBuffer, 0, byteSize);
                                totalReceived += byteSize;
                                sessionReceived += byteSize;



                                float currentSpeed = sessionReceived / (float)stopwatch.Elapsed.TotalSeconds;
                                DownloadProgressChanged.Invoke(this, new DownloadProgressChangedEventArgs(BundleInfo.assetBundleName, totalReceived, (long)currentSpeed));
                            }
                            didComplete = true;
                            stopwatch.Stop();
                            stream.Close();
                        }
                    }
                    response.Close();
                    saveFileStream.Close();
                    saveFileStream = null;
                }
                if (didComplete)
                {
                    Debug.Log("AssetBundler DoDownload. Completed. Throwing to CRC Check.");
                    //download completed
                    AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.CRCCheck));
                    AssetBundleManager.Instance.ResetPollingTime(); //we reset the poll to start the next download immediately
                }
            }
            catch (WebException we)
            {
                //416 error means we requested some bytes which don't exist.
                //this means the local game is expecting more data than the server has.
                //in this case, the file must be finished (no more data on the server), so we throw it to a CRC check to confirm
                if ((int)we.Status == 416)
                {
                    Debug.LogWarning("AssetBundler DoDownload: Error 416. Assuming File is completed");
                    //this is considered an error for retry purposes
                    AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.CRCCheck, "ERROR_416_FROM_SERVER"));
                }
                if (we.Status == WebExceptionStatus.Timeout)
                {
                    Debug.LogException(we);
                    Debug.LogWarning("Connection Timed Out, likly due to poor or loss of connection, restarting bundle download");
                    AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.CRCCheck));
                }
                else
                {
                    Debug.LogException(we);
                    Debug.LogError("AssetBundler DoDownload: Exception caused assetbundle download failure " + BundleInfo.assetBundleName + " . Performing full file/CRC to work out file status: " + we.ToString());
                    AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.CRCCheck)); //no error because timeouts aren't a problem with the bundle
                }
            }
            catch (IOException ioe)
            {
                Debug.LogException(ioe);
                Debug.LogError("AssetBundler DoDownload: IO/Write Exception. Deleting offending file: " + tempFileInfo.Name + ": " + ioe.ToString());
                AssetBundleDownloadCorruptedOrBroken.Invoke(this, new AssetBundleDeleteAssetBundleFileEventArgs(BundleInfo.assetBundleName, "ASSETBUNDLE_IO_WRITE_EXCEPTION"));
                //kick it right back off again
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Debug.LogWarning(objectDisposedException.Message);
                Debug.LogWarning("Aborting download " + BundleInfo.assetBundleName + ", likly due to loss of connection, recheck bundle");
                AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.CRCCheck));

            }
            catch (ThreadAbortException)
            {
                Debug.LogWarning("AssetBundler DoDownload: Thread aborted. Assuming intentionally.");
                //AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(assetBundleQueueInfo.AssetBundleName, AssetBundleDownloadState.CRCCheck, ""));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("AssetBundler DoDownload: Exception caused assetbundle download failure. Performing full file/CRC to work out file status. Error:  " + e.ToString());
                AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(BundleInfo.assetBundleName, AssetBundleDownloadState.CRCCheck, e.ToString()));
            }
            finally
            {
                if (saveFileStream != null)
                {
                    saveFileStream.Close();
                    saveFileStream = null;
                }
            }
            Debug.Log("Download Thread Ended: " + BundleInfo.assetBundleName);
        }

        private static void CreateDownloadDirectory()
        {
            if (!Directory.Exists(AssetBundleManager.Instance.CurrentVersionDownloadLocation))
            {
                Directory.CreateDirectory(AssetBundleManager.Instance.CurrentVersionDownloadLocation);
            }
        }

        private bool CheckForResumeableReponseCode(Bundle BundleInfo, string downloadURL, int responseCode)
        {
            bool downloadResumable;
            if (responseCode == 206) //same as: response.StatusCode == HttpStatusCode.PartialContent
            {
                Debug.Log("AssetBundler DoDownload: " + downloadURL + " is resumable (206 status)");
                downloadResumable = true;
            }
            else if (responseCode >= 200 && responseCode <= 299) //2xx is success
            {
                Debug.Log("AssetBundler DoDownload: " + downloadURL + " is not resumable (" + responseCode + ")");
                downloadResumable = false;
                //wipe any copy of the file, as it's not resumable. Don't consider this a failure, just start downloading it anew
                AssetBundleDownloadCorruptedOrBroken.Invoke(this, new AssetBundleDeleteAssetBundleFileEventArgs(BundleInfo.assetBundleName, null));
            }
            else
            {
                Debug.Log("AssetBundler DoDownload: file not accessible. Status code: " + responseCode);
                downloadResumable = false;
            }

            return downloadResumable;
        }

        private static void DebugNoInterentConnection()
        {
            //spoofed no internet
            int connectionSpoofed = 0;// Assets.Code.Common.DebugOptions.GetFieldValue(Assets.Code.Common.DebugField.spoofConnection);
            if (connectionSpoofed == 2)
            {
                throw new WebException("Internet Disconnected", null, WebExceptionStatus.ConnectFailure, null);
            }
        }

        private static void DebugThrottleConnectionSpeed()
        {
            //debug throttling logic
            int internetThrottleStatus = 0;//Assets.Code.Common.DebugOptions.GetFieldValue(Assets.Code.Common.DebugField.spoofSlowInternet);
            if (internetThrottleStatus == 1)
            {
                //16ms sleep =  250kb/sec max speed
                Thread.Sleep(16);
            }
            else if (internetThrottleStatus == 2)
            {
                //80ms sleep =  50kb/sec max speed
                Thread.Sleep(80);
            }
            else if (internetThrottleStatus == 3)
            {
                //800ms sleep = 5kb/sec max speed
                Thread.Sleep(800);
            }
        }

    }
}
