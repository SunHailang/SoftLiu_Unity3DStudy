using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu.Build;
using UnityEditor;
using System.IO;
using System.Linq;

public static class BuildProcess
{

    //  Global variable to control Jenkins variants
    public static bool IsJenkinsBuild = false;

    // Flag to perform Direct build steps
    public static bool PerformUnityBuildSteps = true;

    public static BuildType BuildType = BuildType.Development;

    //  Global variable to control if build should run
    public static bool IsAutoRunBuild = false;
    public static bool UploadGameDB = false;

    private static readonly List<IBuildStep> m_steps;

    static BuildProcess()
    {
        SetupBuildType();

        m_steps = new List<IBuildStep>();
        m_steps.Add(new EditorCleanStep());
        m_steps.Add(new RemoveUnusedAndroidArtAssets());
        m_steps.Add(new CreateAndStripLanguagesBuildStep());
        m_steps.Add(new SetupUnityBuildStep());
        m_steps.Add(new BuildVersionStep());
        m_steps.Add(new PlayerBuildStep());
        m_steps.Add(new PostBuildStep());
    }

    public static string GetBuildPath(BuildTarget target, BuildType type, bool fromJenkins = false)
    {
        string path = Application.dataPath + "/../Builds/";
        switch (target)
        {
            case BuildTarget.iOS:
                path += "iOS/";
                break;
            case BuildTarget.Android:
                path += "Android/";
                break;
            default:
                path += "Unkown/";
                break;
        }
        path += GetBuildOutputName(target, type);
        string absolutePath = Path.GetFullPath(path);
        return absolutePath;
    }

    public static string GetBuildOutputName(BuildTarget target, BuildType type)
    {
        string toRet = PlayerSettings.productName.Replace(" ", "") + "_" + type;
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {
            toRet += ".exe";
        }
        return toRet;
    }

    public static List<IBuildStep> GetBuildSteps(BuildStepType type)
    {
        return m_steps.Where(s => s.GetBuildType() == type).ToList();
    }

    public static List<IBuildStep> GetStepSorted()
    {
        List<IBuildStep> steps = new List<IBuildStep>();
        steps.AddRange(GetBuildSteps(BuildStepType.Pre));
        steps.AddRange(GetBuildSteps(BuildStepType.Direct));
        steps.AddRange(GetBuildSteps(BuildStepType.Post));
        return steps;
    }

    public static void Excute(BuildTarget target, BuildType type, string path, bool runAfterBuild = false)
    {
        Debug.Log("Marking build " + target + " to " + path);
        IsAutoRunBuild = runAfterBuild;
        BuildType = type;
        if (!IsJenkinsBuild && EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("Compiling", "Please wait for the Editor to finish compiling.", "OK");
            return;
        }
        try
        {
            // Execute all steps in sequence
            BuildStepExecutor.Execute(GetStepSorted(), target, type, path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Build Process Excute Error: " + e.Message);
        }
        finally
        {
            IsJenkinsBuild = false;
            IsAutoRunBuild = false;
            PerformUnityBuildSteps = true;
            SetupBuildType();
        }
    }

    private static void SetupBuildType()
    {
        BuildType = BuildType.Development;
#if PREPRODUCTION
        BuildType = BuildType.Preproduction;
#elif PRODUCTION
        BuildType = BuildType.Production;
#endif
    }
}
