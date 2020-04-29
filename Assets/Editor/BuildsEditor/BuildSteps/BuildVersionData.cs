using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Build
{
    public class BuildVersionData
    {
        // 用于生成JSON的字段
        public string defVersionName;
        public int defVersionCode;
        public int defTargetSdkVersion;
        // 用于获取属性的名字
        public string m_defVersionName { get { return defVersionName; } }
        public int m_defVersionCode { get { return defVersionCode; } }
        public int m_defTargetSdkVersion { get { return defTargetSdkVersion; } }
    }
}
