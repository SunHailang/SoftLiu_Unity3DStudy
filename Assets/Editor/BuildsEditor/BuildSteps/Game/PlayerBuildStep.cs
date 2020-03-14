using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SoftLiu.Build
{
    public class PlayerBuildStep : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            string[] scenes = GetScenes();
            BuildOptions options = GetOptions(target, type == BuildType.Development, BuildProcess.IsAutoRunBuild);

            // Create output dir
            string outputName = path;

            // Start player build
            BuildReport buildRepoart = BuildPipeline.BuildPlayer(scenes, outputName, target, options);
            if (buildRepoart.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("Build Player Error: " + buildRepoart.summary);
                throw new Exception("BuildPlayer returned errors: " + buildRepoart.summary.totalErrors);
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Direct;
        }

        private BuildOptions GetOptions(BuildTarget target, bool debug, bool run)
        {
            BuildOptions options = BuildOptions.None;
            if (BuildProcess.PerformUnityBuildSteps)
            {
                if (target == BuildTarget.Android)
                    options |= BuildOptions.AcceptExternalModificationsToPlayer;
                if (debug)
                    options |= BuildOptions.Development;
                if (run)
                    options |= BuildOptions.AutoRunPlayer;
            }
            else
            {
                if (target == BuildTarget.Android)
                {
                    options |= BuildOptions.AcceptExternalModificationsToPlayer;
                }
#if !PRODUCTION && !PRE_PRODUCTION
                options |= BuildOptions.Development;
#endif
                if (BuildProcess.IsAutoRunBuild)
                {
                    options |= BuildOptions.AutoRunPlayer;
                }
            }

            return options;
        }

        private string[] GetScenes()
        {
            List<string> toRet = new List<string>();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && File.Exists(scene.path))
                {
                    toRet.Add(scene.path);
                }
            }

            return toRet.ToArray();
        }
    }
}
