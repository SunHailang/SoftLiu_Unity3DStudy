using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public enum SoftLiuBuildTarget
    {
        Unknown,
        iOS,
        Android,
    }

    public static class SoftLiuBuildTargetResolver
    {
        public static SoftLiuBuildTarget Resolve(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.iOS:
                    return SoftLiuBuildTarget.iOS;
                case BuildTarget.Android:
                    return SoftLiuBuildTarget.Android;
            }
            return SoftLiuBuildTarget.Unknown;
        }
    }

    // <summary>
    /// Possible build types
    /// </summary>
    public enum BuildType
    {
        Development,
        Preproduction,
        Production,
        Marketing
    }
}
