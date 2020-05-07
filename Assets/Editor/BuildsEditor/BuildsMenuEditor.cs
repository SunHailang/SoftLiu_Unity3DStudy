using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SoftLiu.Build;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class BuildsMenuEditor
{

    private static string GetLoadedScene()
    {
        Scene currentScent = EditorSceneManager.GetActiveScene();
        return currentScent.path;
    }

    private static void PerformBuild(BuildTarget target, BuildType type, bool fromJenkins = false, bool runAfterBuild = false, bool performUnityBuildSteps = true)
    {
        string openedScene = GetLoadedScene();
        BuildProcess.PerformUnityBuildSteps = performUnityBuildSteps;

        string buildPath = BuildProcess.GetBuildPath(target, type, fromJenkins);
        if (target == BuildTarget.Android || target == BuildTarget.iOS)
        {
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            else
            {
                FileUtilities.DeleteDirectory(buildPath);
            }
        }
        BuildProcess.Excute(target, type, buildPath, runAfterBuild);
        if (!string.IsNullOrEmpty(openedScene) && openedScene != GetLoadedScene())
        {
            EditorSceneManager.OpenScene(openedScene);
        }
    }

    #region Android
    [MenuItem("SoftLiu/Builds/Android/Build Development", false, 50)]
    public static void AndroidBuildEditor_Development()
    {
        Build_Android_Development();
    }
    public static void Build_Android_Development()
    {
        PerformBuild(BuildTarget.Android, BuildType.Development, false);
    }

    [MenuItem("SoftLiu/Builds/Android/Build PreProduction", false, 50)]
    public static void AndroidBuildEditor_PreProduction()
    {
        Build_Android_PreProduction();
    }
    public static void Build_Android_PreProduction()
    {
        PerformBuild(BuildTarget.Android, BuildType.Preproduction, false);
    }

    [MenuItem("SoftLiu/Builds/Android/Build Production", false, 50)]
    public static void AndroidBuildEditor_Production()
    {
        Build_Android_Production();
    }
    public static void Build_Android_Production()
    {
        PerformBuild(BuildTarget.Android, BuildType.Production, false);
    }

    #endregion
}
