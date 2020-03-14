using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{

    public class RemoveUnusedAndroidArtAssets : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target== BuildTarget.Android)
            {
                RemoveUnusedGUIAssets();
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }

        private void RemoveUnusedGUIAssets()
        {

        }
    }
}
