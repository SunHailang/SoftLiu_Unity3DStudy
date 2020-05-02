using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles.Downloader
{
    public struct AssetBundleCRCInfo
    {
        public string m_name;
        public uint m_CRC;
        public long m_fileSizeBytes;


        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("[{0}|{1}|{2}", m_name, m_CRC, m_fileSizeBytes + "]");
        }
    }
}
