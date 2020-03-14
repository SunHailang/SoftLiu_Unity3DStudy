using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.Build
{
    public class SetupUnityBuildStepSettings
    {
        public static readonly Dictionary<SoftLiuBuildTarget, Dictionary<BuildType, string>> PreprocessorDefines = new Dictionary<SoftLiuBuildTarget, Dictionary<BuildType, string>>()
        {
            {
                SoftLiuBuildTarget.iOS, new Dictionary<BuildType, string>
                {
                    {BuildType.Development,"ENABLE_LOG;" },
                    {BuildType.Preproduction,"PREPRODUCTION;ENABLE_LOG;" },
                    {BuildType.Production,"PRODUCTION;ENABLE_LOG;" },
                    {BuildType.Marketing,";" }
                }
            },
            {
                SoftLiuBuildTarget.Android, new Dictionary<BuildType, string>
                {
                    {BuildType.Development,"ENABLE_LOG;" },
                    {BuildType.Preproduction,"PREPRODUCTION;ENABLE_LOG;" },
                    {BuildType.Production,"PRODUCTION;ENABLE_LOG;" },
                    {BuildType.Marketing,";" }
                }
            }
        };

    }
}
