using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace SoftLiu.Build
{
    public class EditorCleanStep : IBuildStep
    {
        [MenuItem("SoftLiu/Utility/CleanConsole")]
        public static void CleanConsoleEditor()
        {
            CleanConsole();
        }
        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }

        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target == BuildTarget.Android)
            {
                CleanConsole();
            }
            else if (target == BuildTarget.iOS)
            {

            }
        }

        private static void CleanConsole()
        {
            // unity 2017 after
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
            // unity 2017 before
            //var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            //if (logEntries == null)
            //{
            //    Debug.LogError("CleanConsole  is null.");
            //    return;
            //}
            //var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            //clearMethod.Invoke(null, null);
        }
    }
}
