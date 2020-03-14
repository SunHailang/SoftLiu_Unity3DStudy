using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class CreateAndStripLanguagesBuildStep : IBuildStep
    {
        [MenuItem("SoftLiu/Localization/SetAndroidLoc")]
        public static void SetAndroidLoc()
        {
            Debug.Log("SetAndroidLoc");
        }
        [MenuItem("SoftLiu/Localization/SetiOSLoc")]
        public static void SetiOSLoc()
        {
            Debug.Log("SetiOSLoc");
        }

        public void Execute(BuildTarget target, BuildType type, string path)
        {
            AssetDatabase.StartAssetEditing();

            SplitLocalisation();

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }

        private void SplitLocalisation()
        {

        }
    }
}