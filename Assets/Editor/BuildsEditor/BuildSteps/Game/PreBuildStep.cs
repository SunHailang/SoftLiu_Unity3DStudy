using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class PreBuildStep : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target == BuildTarget.Android)
            {

            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }
    }
}
