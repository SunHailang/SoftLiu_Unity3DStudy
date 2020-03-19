using Force.Crc32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    public class AssetBundleUtils
    {

        //about 5 times faster than md5. Ballache though
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
                Debug.LogError("AssetBundleUtils | GenerateCRC32FromFile failed to generate CRC: " + fileName + ". Exception: " + e.ToString());
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
            Debug.Log("CRC32 " + fileName + ":" + x.ElapsedMilliseconds + "ms <> " + crc.ToString());
#endif
            return crc;
        }
    }
}
