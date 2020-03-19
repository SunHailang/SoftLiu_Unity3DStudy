using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;

namespace SoftLiu.AssetBundles.Downloader
{
    /*
     * Used to extract which dependencies need downloading from each bundle.
     * Essentially, call "QueueDependenciesForBundle" with a bundle url, and this will queue all dependencies, get the full asset size (for user prompt)
     * It'll then queue all the downloads and wait for a "Start Download" command.
     * This can be used to clear all queue too.
	 *
	 * @Note: Server CRC's only become local CRC's if:
	 *   1. The asset bundle is fully downloaded and loadable.
	 *   2. There was no asset bundle of that name.
	 *   3. The local asset bundle by that name was not loadable.
     */

    public class AssetBundleInformationManager
    {
        private bool m_isApplicationQuitting = false;
        private List<AssetBundleCRCInfo> m_localAssetBundleCRCInformation = null;
        private List<AssetBundleCRCInfo> m_serverAssetBundleCRCInformation = null;
        private Thread m_AssetBundleCRCInformationThread = null;
        public event EventHandler<AssetBundleStateChangedEventArgs> AssetBundleStateChanged;
        public event EventHandler<AssetBundleLocalFileCheckedEventArgs> AssetBundleLocalFileChecked;
        public event EventHandler<AssetBundleEventArgs> AssetBundleFinishedAndRequiresMoving;
        public event EventHandler<AssetBundleDeleteAssetBundleFileEventArgs> AssetBundleDownloadCorruptedOrBroken;
        public event EventHandler<AssetBundleEventArgs> AssetBundleRevokePermision;
        public bool HasLoadedLocalCRCInfo { get; private set; }
        public bool HasLoadedServerCRCInfo { get; private set; }
        public string CRCThreadStatus { get { return Util.GetThreadStatus(m_AssetBundleCRCInformationThread); } }

        private float m_realtimeToPollAgain = 0;

        public AssetBundleInformationManager(List<AssetBundleCRCInfo> includedInBuildCrcList)
        {
            m_serverAssetBundleCRCInformation = new List<AssetBundleCRCInfo>(20);
            m_localAssetBundleCRCInformation = new List<AssetBundleCRCInfo>(includedInBuildCrcList);
            m_AssetBundleCRCInformationThread = null;
        }


        public void ApplicationQuitKillInformationThreads()
        {
            m_isApplicationQuitting = true;
            if (m_AssetBundleCRCInformationThread != null && m_AssetBundleCRCInformationThread.IsAlive)
            {
                m_AssetBundleCRCInformationThread.Abort();
            }
            m_AssetBundleCRCInformationThread = null;
        }



        // Called once every x seconds via AssetBundleManager.
        public void PollUpdate(bool localOnly = false)
        {
            if (m_isApplicationQuitting)
            {
                return;
            }
            if (m_realtimeToPollAgain > Time.realtimeSinceStartup)
            {
                return;
            }
            m_realtimeToPollAgain = Time.realtimeSinceStartup;

            if (m_AssetBundleCRCInformationThread == null || !m_AssetBundleCRCInformationThread.IsAlive)
            {
                if (!HasLoadedLocalCRCInfo)
                {
                    Debug.Log("AssetBundleInformationManager: m_AssetBundleCRCInformationThread start local");
                    m_AssetBundleCRCInformationThread = new Thread(TryGetBundleCRCInformationFromLocalFiles);
                    m_AssetBundleCRCInformationThread.Start();
                }
                else if (!HasLoadedServerCRCInfo && localOnly == false)  //Only do this is we have connectivity
                {
                    Debug.Log("AssetBundleInformationManager: m_AssetBundleCRCInformationThread start server");
                    m_AssetBundleCRCInformationThread = new Thread(GetBundleCRCInformationFromServer);
                    m_AssetBundleCRCInformationThread.Start();
                }
            }
        }

        public void TryGetBundleCRCInformationFromLocalFiles()
        {
            Debug.Log("AssetBundleInformationManager: GetBundleCRCInformationFromLocalFiles");
            string crcFile = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/assetbundles.crc";
            string tmpCrcFile = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/assetbundles.crc_tmp";

            bool shouldLoad = false;
            if (File.Exists(crcFile))
            {
                //cool, we have prior knowledge of assetbundles.
                //we need to check all files vs the crcs and filelengths
                shouldLoad = true;
            }
            else if (File.Exists(tmpCrcFile))
            {
                try
                {
                    File.Move(tmpCrcFile, crcFile);
                    shouldLoad = true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError("Exception while moving tmpCrcFile to crc file location. We assume no CRC info. " + e);
                }
            }

            if (shouldLoad)
            {
                Debug.Log("AssetBundleInformationManager: Loading local CRC Info");
                string assetBundleCRCContents = File.ReadAllText(crcFile);
                try
                {
                    ParseCRCInformation(assetBundleCRCContents, m_localAssetBundleCRCInformation);
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse assetbundle local CRCInfo: " + e.ToString());
                }
            }
            else
            {
                Debug.Log("AssetBundleInformationManager: No Local CRC Info");
            }

            HasLoadedLocalCRCInfo = true;

            // Even if we don't have a local file by this point, we might have the includedInBuild one.
            // So lets initialize bundles anyway.
            // We know for sure we don't have server info here, so null is passed.
            for (int i = 0; i < m_localAssetBundleCRCInformation.Count; i++)
            {
                AssetBundleCRCInfo localCrc = m_localAssetBundleCRCInformation[i];
                CheckLocalAssetBundle(localCrc, i, null);
            }
        }



        public void CheckLocalAssetBundle(string assetBundleName)
        {
            var serverCrc = TryGetCrcInfo(assetBundleName, m_serverAssetBundleCRCInformation);
            for (int i = 0; i < m_localAssetBundleCRCInformation.Count; i++)
            {
                AssetBundleCRCInfo localCrc = m_localAssetBundleCRCInformation[i];
                if (localCrc.m_name == assetBundleName)
                {
                    CheckLocalAssetBundle(localCrc, i, serverCrc);
                    break;
                }
            }
        }



        private static AssetBundleCRCInfo? TryGetCrcInfo(string assetBundleName, List<AssetBundleCRCInfo> query)
        {
            foreach (AssetBundleCRCInfo assetBundleCrcInfo in query)
            {
                if (assetBundleCrcInfo.m_name == assetBundleName)
                {
                    return assetBundleCrcInfo;
                }
            }
            return null;
        }



        public void CheckLocalAssetBundle(AssetBundleCRCInfo localCrc, int localCrcIndex, AssetBundleCRCInfo? serverCrc)
        {
            bool isInGame = AssetBundleManager.Instance.IsInGame;
            string fullFileName = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/" + localCrc.m_name;
            string tempFileName = fullFileName + ".incomplete";
            FileInfo fullAssetFileInfo = new FileInfo(fullFileName);

            if (fullAssetFileInfo.Exists)
            {
                //check filesize first
                if (fullAssetFileInfo.Length == localCrc.m_fileSizeBytes)
                {
                    if (isInGame)
                    {
                        //the file is the correct size, but CRC is slow, so we'll delay the CRC
                        //don't do this now!
                        return;
                    }

                    Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle : Completed file: " + localCrc.m_name);
                    //sweet, same length, it's complete, now to check CRC
                    uint crc = AssetBundleUtils.GenerateCRC32FromFile(fullFileName);
                    if (crc == localCrc.m_CRC)
                    {
                        //it's the right file, untampered.
                        //it's loadable.
                        Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle : Local Asset Bundle " + localCrc.m_name + " is Loadable. SUCCESS");
                        AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(localCrc.m_name, true, localCrc.m_fileSizeBytes, localCrc.m_fileSizeBytes));
                        AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(localCrc.m_name, AssetBundleDownloadState.Loadable, null));
                        AssetBundleRevokePermision.Invoke(this, new AssetBundleEventArgs(localCrc.m_name));
                    }
                    else
                    {
                        //tampered? or damaged? Delete it.
                        Debug.LogError("AssetBundleInformationManager::CheckLocalAssetBundle : Local Asset Bundle " + localCrc.m_name + " failed the CRC check. Deleting.");
                        AssetBundleDownloadCorruptedOrBroken.Invoke(this, new AssetBundleDeleteAssetBundleFileEventArgs(localCrc.m_name, "FAILED_LOCAL_CRC_CHECK"));
                    }
                }
                else
                {
                    //it's not an incomplete dl, and it's either too big or too small, not sure how that could happen, wipe it and force a redownload
                    //no point crcing as the bytes are definitely different.
                    Debug.LogError("AssetBundleInformationManager: Local Asset Bundle " + localCrc.m_name + " failed the LENGTH check. How!? Deleting.");
                    AssetBundleDownloadCorruptedOrBroken.Invoke(this, new AssetBundleDeleteAssetBundleFileEventArgs(localCrc.m_name, "FAILED_LOCAL_LENGTH_CHECK"));
                }
            }

            FileInfo tempAssetFileInfo = new FileInfo(tempFileName);
            if (tempAssetFileInfo.Exists)
            {
                Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle : Temporary File: " + localCrc.m_name);
                if (isInGame)
                {
                    if (serverCrc.HasValue && tempAssetFileInfo.Length < serverCrc.Value.m_fileSizeBytes)
                    {
                        // We don't know what this file is, but we'll assume it's a partially downloaded NEW asset bundle and put it back in the queue
                        Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle : Temporary Asset Bundle " + localCrc.m_name + " failed CRC. It's small enough to feasibly be a partially-downloaded new asset bundle. Download continuing.");
                        AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(serverCrc.Value.m_name, false, tempAssetFileInfo.Length, serverCrc.Value.m_fileSizeBytes));
                        AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(serverCrc.Value.m_name, AssetBundleDownloadState.Queued, null));
                    }
                    return;
                }
                else
                {
                    uint crc = AssetBundleUtils.GenerateCRC32FromFile(tempFileName);

                    bool crcMatchesLocal = localCrc.m_CRC == crc;
                    bool crcMatchesServer = serverCrc.HasValue && serverCrc.Value.m_CRC == crc;

                    if (crcMatchesLocal || crcMatchesServer)
                    {
                        // Crc matches either!

                        if (!crcMatchesLocal)
                        {
                            // The recently completed ".incomplete" file will now be moved to the final destination (oooh).
                            // Thus, the local CRC info needs to match the new crc.
                            m_localAssetBundleCRCInformation[localCrcIndex] = serverCrc.Value;
                        }

                        // The file is loadable, so move it to the right file name. Yay!
                        Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle : Temporary Asset Bundle " + localCrc.m_name + " matched a CRC check and is now loadable. SUCCESS." +
                                "\nServer: " + crcMatchesServer + ", Local: " + crcMatchesLocal);

                        AssetBundleFinishedAndRequiresMoving.Invoke(this, new AssetBundleEventArgs(localCrc.m_name));
                    }
                    else if (serverCrc.HasValue && tempAssetFileInfo.Length < serverCrc.Value.m_fileSizeBytes)
                    {
                        // We don't know what this file is, but we'll assume it's a partially downloaded NEW asset bundle.
                        Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle :  Temporary Asset Bundle " + localCrc.m_name + " failed CRC. It's small enough to feasibly be a partially-downloaded new asset bundle. Download continuing.");
                        AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(serverCrc.Value.m_name, false, tempAssetFileInfo.Length, serverCrc.Value.m_fileSizeBytes));
                        AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(serverCrc.Value.m_name, AssetBundleDownloadState.Queued, null));
                    }
                    else if (HasLoadedServerCRCInfo)
                    {
                        // It failed all CRC AND we have got all server CRC's. Thus, this file must be corrupted.
                        Debug.LogError("AssetBundleInformationManager::CheckLocalAssetBundle : Local Asset Bundle " + localCrc.m_name + " does not match any CRC, and we have all server CRC's. Assumed corrupted.");
                        AssetBundleDownloadCorruptedOrBroken.Invoke(this, new AssetBundleDeleteAssetBundleFileEventArgs(localCrc.m_name, "FAILED_SERVER_CRC_CHECK"));
                    }
                    else
                    {
                        // We don't know if this is a new version, so no checks are worth doing on this until we have server info
                        Debug.Log("AssetBundleInformationManager::CheckLocalAssetBundle : Local Asset Bundle " + localCrc.m_name + " does not match local CRC and we do not currently have the server CRC's. " +
                                "It's probably a new bundle in a server CRC that we haven't downloaded for this play session yet. We'll queue it for download, pending the server CRC's.");
                        AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(localCrc.m_name, false, tempAssetFileInfo.Length, localCrc.m_fileSizeBytes));
                        AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(localCrc.m_name, AssetBundleDownloadState.WaitingManualPermission, null));
                    }
                }
            }

            // CRC checking complete. Phew.

            tempAssetFileInfo.Refresh();
            fullAssetFileInfo.Refresh();
            if (!tempAssetFileInfo.Exists && !fullAssetFileInfo.Exists)
            {
                // No file left after CRC checks, so the only thing to do is begin downloading afresh.
                AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(localCrc.m_name, false, 0, localCrc.m_fileSizeBytes));
                AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(localCrc.m_name, AssetBundleDownloadState.WaitingManualPermission, null));
            }
        }


        public void GetBundleCRCInformationFromServer()
        {
            Debug.Log("AssetBundleInformationManager: GetBundleCRCInformation");

            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(AssetBundleManager.Instance.BundleServerURL + "assetbundles.crc");
                request.Timeout = 5000; //5 secs timeout

                Debug.Log("Request URI (CRC): " + request.RequestUri.AbsoluteUri);


                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    m_realtimeToPollAgain += m_realtimeToPollAgain; // Take longer each time we call to prevent over polling.

                    if ((int)response.StatusCode == 200)
                    {
                        Debug.Log("AssetBundleInformationManager: LoadCRCInformation Server");

                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string strResponse = reader.ReadToEnd();
                        ParseCRCInformation(strResponse, m_serverAssetBundleCRCInformation);


                        //SERVER CRC INFO:

                        // Compare server CRC's with local CRC's for changes.
                        foreach (var newServerCrc in m_serverAssetBundleCRCInformation)
                        {
                            foreach (AssetBundleCRCInfo localCrc in m_localAssetBundleCRCInformation)
                            {
                                if (localCrc.m_name == newServerCrc.m_name)
                                {
                                    //this is an asset bundle we already know about from local crc info

                                    if (localCrc.m_fileSizeBytes != newServerCrc.m_fileSizeBytes || localCrc.m_CRC != newServerCrc.m_CRC)
                                    {
                                        Debug.Log("AssetBundleInformationManager::GetBundleCRCInformationFromServer - Detected a new version of the bundle from the server. Wiping the current file and downloading the new one.");
                                        //the server has a file that's different from the one we have here. Wipe any copy of this file, but don't count it as a failure to load
                                        AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(localCrc.m_name, false, 0, newServerCrc.m_fileSizeBytes));
                                        AssetBundleDownloadCorruptedOrBroken.Invoke(this, new AssetBundleDeleteAssetBundleFileEventArgs(localCrc.m_name, null));
                                        AssetBundleRevokePermision.Invoke(this, new AssetBundleEventArgs(localCrc.m_name));

                                        //no point keeping old crc info for a file we never had/couldn't load. Simply replace the crc info
                                        int index = m_localAssetBundleCRCInformation.IndexOf(localCrc);
                                        m_localAssetBundleCRCInformation[index] = newServerCrc;

                                    }

                                    //otherwise it didn't change, and we can simply continue
                                    goto localCrcFound;
                                }
                            }

                            //this code is skipped if the file was found in the known assetbundle list
                            //unknown asset bundle, add it to the list
                            m_localAssetBundleCRCInformation.Add(newServerCrc);
                            AssetBundleLocalFileChecked.Invoke(this, new AssetBundleLocalFileCheckedEventArgs(newServerCrc.m_name, false, 0, newServerCrc.m_fileSizeBytes));
                            AssetBundleStateChanged.Invoke(this, new AssetBundleStateChangedEventArgs(newServerCrc.m_name, AssetBundleDownloadState.WaitingManualPermission, null));


                            localCrcFound:;
                        }

                        WriteCRCInfoToDisk();

                        reader.Close();
                        HasLoadedServerCRCInfo = true;
                        m_AssetBundleCRCInformationThread = null;

                    }
                    else
                    {
                        Debug.LogError("AssetBundleInformationManager: GetBundleCRCInformation Failed With Response: " + response.StatusCode);
                        m_AssetBundleCRCInformationThread = null;
                    }
                    response.Close();
                }
            }
            catch (WebException e)
            {
                if (AssetBundleManager.Instance.CurrentReachability == NetworkReachability.NotReachable)
                {
                    Debug.LogWarning("No internet access, cannot get asset bundle data");
                    return;
                }
                m_realtimeToPollAgain += m_realtimeToPollAgain;
                Debug.LogException(e);
                Debug.LogError("AssetBundleInformationManager: GetBundleCRCInformationFromServer WebException " + e.ToString());

                m_AssetBundleCRCInformationThread = null;
            }
            catch (ThreadAbortException)
            {
                m_realtimeToPollAgain += m_realtimeToPollAgain;
                Debug.LogWarning("AssetBundleInformationManager: GetBundleCRCInformationFromServer Thread aborted. Assuming intentionally.");
            }
            catch (Exception e)
            {
                m_realtimeToPollAgain += m_realtimeToPollAgain;
                Debug.LogException(e);
                Debug.LogError("AssetBundleInformationManager: GetBundleCRCInformationFromServer Exception " + e.ToString());
            }
        }

        public void WriteCRCInfoToDisk()
        {
            Debug.Log("AssetBundleInformationManager: WriteCRCInfoToDisk");
            try
            {
                string fileData = "";
                //gonna write and swap.string fileData = "";
                foreach (AssetBundleCRCInfo abcrci in m_localAssetBundleCRCInformation)
                {
                    if (fileData != "")
                    {
                        fileData = fileData + "\n";
                    }
                    fileData = fileData + abcrci.m_name + "|" + abcrci.m_CRC.ToString() + "|" + abcrci.m_fileSizeBytes;
                }
                string tmpFile = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/assetbundles.crc_tmp";
                string file = AssetBundleManager.Instance.CurrentVersionDownloadLocation + "/assetbundles.crc";
                File.WriteAllText(tmpFile, fileData);
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                File.Move(tmpFile, file);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("AssetBundleInformationManager: WriteCRCInfoToDisk Failed: " + e.ToString());
            }
        }

        /// <summary>
        ///		APPENDS found crcInfo's to the specified list.
        /// </summary>
        public static void ParseCRCInformation(string fileContents, List<AssetBundleCRCInfo> infos)
        {
            Debug.Log("ParseCRCInformation:\n" + fileContents);

            string[] assetBundleLines = fileContents.Split('\n');

            foreach (string assetBundle in assetBundleLines)
            {
                string[] infoParts = assetBundle.Split('|');

                if (infoParts.Length == 3)
                {
                    AssetBundleCRCInfo newAssetBundleCRCInfo = new AssetBundleCRCInfo();
                    newAssetBundleCRCInfo.m_name = infoParts[0];
                    newAssetBundleCRCInfo.m_CRC = UInt32.Parse(infoParts[1]);
                    newAssetBundleCRCInfo.m_fileSizeBytes = Int32.Parse(infoParts[2]);

                    int index = infos.FindIndex(x => x.m_name == infoParts[0]);
                    if (index >= 0)
                    {
                        infos[index] = newAssetBundleCRCInfo;
                    }
                    else
                    {
                        infos.Add(newAssetBundleCRCInfo);
                    }

                }
                // Else file is done.
            }
        }



        public string OnGUI()
        {
            return
                m_localAssetBundleCRCInformation.Aggregate("\nLocal: ", (c, n) => c + n + "\n")
                + m_serverAssetBundleCRCInformation.Aggregate("\nServer: ", (c, n) => c + n + "\n");
        }
    }
}
