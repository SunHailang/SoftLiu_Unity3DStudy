using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class MinifyJsonBuildStep : IBuildStep
    {
        private List<string> m_pathsToMinify = new List<string>()
        {
            // json Assets + path
        };

        public void Execute(BuildTarget target, BuildType type, string path)
        {
            foreach (var item in m_pathsToMinify)
            {
                string fullpath = Application.dataPath + item;
                JSONMinifyTool.MinifyFile(fullpath);
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }
    }
}