
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
        public static SoftLiuBuildTarget Resolve(UnityEditor.BuildTarget target)
        {
            switch (target)
            {
                case UnityEditor.BuildTarget.iOS:
                    return SoftLiuBuildTarget.iOS;
                case UnityEditor.BuildTarget.Android:
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
