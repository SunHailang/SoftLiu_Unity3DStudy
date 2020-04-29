using SoftLiu.Build.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class PostBuildStep : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target == BuildTarget.iOS)
            {

            }else if(target == BuildTarget.Android)
            {
                PostBuildAndroid(type, path);
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Post;
        }

        private void PostBuildAndroid(BuildType type, string path)
        {
            if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                Debug.Log("ExportAsGooleAndroidProject is switched on. Running Android Greadle PostProcess.");

                // Patch the Perforce ridiculuseness
                // handle unityLibrary AndroidManifest.xml
                BuildUtility.HandleAndroidXml(path);
                // Patch the Perforce ridiculuseness
                string gradlePath = Path.Combine(Application.dataPath, "../JenkinsScripts/AndroidGradleStuff");
                // handle gradle version name
                if (type == BuildType.Development)
                {
                    string versionPath = Path.Combine(Application.dataPath, "Misc/buildVersion.json");
                    BuildVersionData data = null;
                    using (StreamReader sr = new StreamReader(versionPath))
                    {
                        string versionData = sr.ReadToEnd();
                        data = JsonUtility.FromJson<BuildVersionData>(versionData);
                    }
                    if (data == null) return;
                    BuildUtility.HandleGradleVersion(Path.Combine(gradlePath, "launcher/build.gradle"), data);
                }

                FileUtility.CopyDirectoryFiles(new DirectoryInfo(gradlePath), new DirectoryInfo(path), true, true);
                string androidPath = Path.Combine(Application.dataPath, "../Tools/Android/Builds");
                FileUtility.CopyDirectoryFiles(new DirectoryInfo(androidPath), new DirectoryInfo(path), true, true);

                string gradleBuildType = "Debug";
                if (type == BuildType.Preproduction || type == BuildType.Production)
                {
                    gradleBuildType = "Release";
                }
                try
                {
                    // Run APK support
                    BuildUtility.RunGradleProcess(path, gradleBuildType);
                    // Bundle support
                    BuildUtility.RunGradleProcess(path, gradleBuildType, "bundle");
                }
                catch (System.Exception error)
                {
                    Debug.LogError("Android build python process failed. msg : " + error.Message);
                }
            }
        }
    }
}
