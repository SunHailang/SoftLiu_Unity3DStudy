using SoftLiu.Build;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Force.Crc32;
using SoftLiu.AssetBundles.Downloader;


public class AssetBundlesMenuEditor
{


    [MenuItem("SoftLiu/AssetBundles/Android/Build Development", false, 0)]
    public static void AssetBundles_BuildAndroidDev()
    {
        BuildTarget platform = BuildTarget.Android;
        BuildType buildType = BuildType.Development;
        string buildDir = Application.dataPath + "/../Builds/AssetBundles/" + platform.ToString() + "/" + buildType.ToString() + "/" + Application.version;
        if (Directory.Exists(buildDir))
        {
            Directory.Delete(buildDir, true);
        }
        Directory.CreateDirectory(buildDir);
        BuildPipeline.BuildAssetBundles(buildDir, BuildAssetBundleOptions.UncompressedAssetBundle, platform);
        GenerateCRCFileInfoPlatform(platform, buildType);
    }


    public static void GenerateCRCFileInfoPlatform(BuildTarget platform, BuildType buildType)
    {
        Debug.Log("GenerateCRCFileInfo " + platform.ToString());
        try
        {
            string buildDir = Application.dataPath + "/../Builds/AssetBundles/" + platform.ToString() + "/" + buildType.ToString() + "/" + Application.version;
            if (Directory.Exists(buildDir))
            {
                if (File.Exists(buildDir + "/assetbundles.crc"))
                {
                    File.Delete(buildDir + "/assetbundles.crc");
                }
                DirectoryInfo buildDirInfo = new DirectoryInfo(buildDir);
                // 删除 所有的 .manifest 文件
                FileUtilities.DeleteDirectoryFiles(buildDirInfo, true, ".manifest");

                List<AssetBundleCRCInfo> m_crcInfoList = new List<AssetBundleCRCInfo>();
                GetCRCInfoList(buildDirInfo.FullName, buildDirInfo, m_crcInfoList, platform);

                string fileData = "";
                foreach (AssetBundleCRCInfo abcrci in m_crcInfoList)
                {
                    if (fileData != "")
                    {
                        fileData = fileData + "\n";
                    }
                    fileData = fileData + abcrci.m_name + "|" + abcrci.m_CRC.ToString() + "|" + abcrci.m_fileSizeBytes;
                }
                File.WriteAllText(buildDir + "/assetbundles.crc", fileData);
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("GenerateCRCFileInfoPlatform Failed " + e.Message);
        }
    }

    private static void GetCRCInfoList(string parent, DirectoryInfo buildDir, List<AssetBundleCRCInfo> crcInfoList, BuildTarget platform)
    {
        //Debug.Log("AssetBundleCRCInfo parent: " + parent);
        foreach (DirectoryInfo dir in buildDir.GetDirectories())
        {
            GetCRCInfoList(parent, dir, crcInfoList, platform);
        }
        foreach (FileInfo abFile in buildDir.GetFiles())
        {
            if (abFile.Name != platform.ToString() && abFile.Name != Application.version)
            {
                AssetBundleCRCInfo abcrci = new AssetBundleCRCInfo();
                //Debug.Log("AssetBundleCRCInfo FullName: " + abFile.FullName);
                string name = abFile.FullName.Substring(parent.Length);
                name = name.Trim('\\', '/');
                name = name.Replace('\\', '/');
                //Debug.Log("AssetBundleCRCInfo Name: " + name);
                abcrci.m_name = name;
                abcrci.m_fileSizeBytes = abFile.Length;
                abcrci.m_CRC = GenerateCRC32FromFile(abFile.FullName);
                crcInfoList.Add(abcrci);
            }
        }
    }

    public static uint GenerateCRC32FromFile(string fileName)
    {
#if UNITY_EDITOR
        Stopwatch x = new Stopwatch();
        x.Start();
#endif
        uint crc = 0;
        FileStream fs = null;
        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            int i = 0;
            int block = 4096;
            byte[] buffer = new byte[block];
            int l = (int)fs.Length;
            while (i < l)
            {
                fs.Read(buffer, 0, block);
                crc = Crc32Algorithm.Append(crc, buffer);
                i += block;
            }
            fs.Close();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("AssetBundleUtils | GenerateCRC32FromFile failed to generate CRC: " + fileName + ". Exception: " + e.ToString());
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
#if UNITY_EDITOR
        x.Stop();
        //UnityEngine.Debug.Log("CRC32 " + fileName + ":" + x.ElapsedMilliseconds + "ms <> " + crc.ToString());
#endif
        return crc;
    }
}
