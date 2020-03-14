using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class SetupUnityBuildStep : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (BuildProcess.PerformUnityBuildSteps)
            {
                if (target == BuildTarget.Android)
                {
                    UpdateAndroid(target, type);
                }
                else if (target == BuildTarget.iOS)
                {

                }
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Direct;
        }

        private void UpdatePreprocessorSymbols(BuildTarget target, BuildType type)
        {
            BuildTargetGroup group = BuildTargetGroup.iOS;
            switch (target)
            {
                case BuildTarget.iOS:
                    group = BuildTargetGroup.iOS;
                    break;
                case BuildTarget.Android:
                    group = BuildTargetGroup.Android;
                    break;
                default:
                    Debug.LogError("Build (PreBuildStep) Unknown Build Target - " + target);
                    break;
            }
            SoftLiuBuildTarget buildTarget = SoftLiuBuildTargetResolver.Resolve(target);
            string scriptingDefines = SetupUnityBuildStepSettings.PreprocessorDefines[buildTarget][type];

            if (BuildProcess.IsJenkinsBuild)
            {
                
            }

            UnityEngine.Debug.LogFormat("PlayerSettings.SetScriptingDefineSymbolsForGroup({0}, {1})", group, scriptingDefines);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, scriptingDefines);
        }

        #region Android
        void UpdateAndroid(BuildTarget target, BuildType type)
        {
            //  Custom Android build step logic
            AndroidFillKeyStoreInfo();

            //  Check if SDK is setup
            CheckAndroidSKDPath();
            CheckOrientation(type);
            //  Check for OBB variant
            CheckForOBB(type);
            CheckForAndroidProject();

            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Medium);
            //PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;

            //	Override ETC
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
        }

        void AndroidFillKeyStoreInfo()
        {
            //PlayerSettings.keystorePass = "";
            //PlayerSettings.keyaliasPass = "";
            //PlayerSettings.Android.keystoreName = "";
        }

        void CheckAndroidSKDPath()
        {
            string sdkPath = EditorPrefs.GetString("AndroidSdkRoot");
            if (sdkPath == null || sdkPath.Length == 0)
            {
                throw new Exception("Android SDK path is not set!!!");
            }
        }

        void CheckForOBB(BuildType type)
        {
            //  AAB FILES ARE NOW IN FASION - NO OBBs ANYMORE
            PlayerSettings.Android.useAPKExpansionFiles = false;

#if !SINGLE_APK
            //  Just in case we have to revert it
            PlayerSettings.Android.useAPKExpansionFiles = (type != BuildType.Development);
#endif
        }

        void CheckOrientation(BuildType type)
        {
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        }

        void CheckForAndroidProject()
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        }

        #endregion
    }
}
