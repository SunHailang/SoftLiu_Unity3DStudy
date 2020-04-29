using SoftLiu.Build;
using SoftLiu.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SoftLiu.Build.Utilities
{
    public static class BuildUtility
    {
        public static void HandleAndroidXml(string path)
        {
            string unityLibraryManifest = Path.Combine(path, "unityLibrary/src/main/AndroidManifest.xml");
            string writeManifestLines;
            using (StreamReader sr = new StreamReader(unityLibraryManifest))
            {
                string readLines = sr.ReadToEnd();
                int indexStart = readLines.IndexOf("<activity");
                int indexEnd = readLines.LastIndexOf("</activity>");
                if (indexStart >= 0 && indexEnd >= 0)
                {
                    string startData = readLines.Substring(0, indexStart);
                    string endData = readLines.Substring(indexEnd + 11);
                    writeManifestLines = startData + endData;
                }
                else
                {
                    writeManifestLines = readLines;
                }
            }
            using (StreamWriter sw = new StreamWriter(unityLibraryManifest))
            {
                sw.Write(writeManifestLines);
            }
            string unityStringXml = Path.Combine(path, "launcher/src/main/res/values/strings.xml");
            string writeStringLines;
            using (StreamReader sr = new StreamReader(unityStringXml))
            {
                string readData = sr.ReadToEnd();
                writeStringLines = readData.Replace(Application.productName, "孙海浪");
            }
            using (StreamWriter sw = new StreamWriter(unityStringXml))
            {
                sw.Write(writeStringLines);
            }
        }
        public static void RunGradleProcess(string buildPath, string gradleBuildType, string packageType = "assemble")
        {
            string executable = Path.Combine(buildPath, "gradlew.bat");
            string arguments = packageType + gradleBuildType;

            // Run python to start build
            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.FileName = executable;
            procStartInfo.Arguments = arguments;
            procStartInfo.UseShellExecute = false;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.WorkingDirectory = buildPath;
            procStartInfo.CreateNoWindow = true;
            Process proc = new Process();
            Debug.Log("RunGradleProcess: " + executable + " " + arguments);
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            string gradleLog = Path.Combine(buildPath, "../gradle_" + packageType + ".log");
            string gradleErrorLog = Path.Combine(buildPath, "../gradle_error_" + packageType + ".log");
            if (result.Length > 1)
            {
                if (File.Exists(gradleLog))
                {
                    File.Delete(gradleLog);
                }
                File.WriteAllText(gradleLog, result);
            }
            if (error.Length > 0)
            {
                if (File.Exists(gradleErrorLog))
                {
                    File.Delete(gradleErrorLog);
                }
                File.WriteAllText(gradleErrorLog, error);
            }
            proc.Close();
        }

        public static void HandleGradleVersion(string gradlePath, BuildVersionData gradleData)
        {
            if (!File.Exists(gradlePath) || gradleData == null) return;

            Type type = typeof(BuildVersionData);
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            StringBuilder writeLines = new StringBuilder();
            using (StreamReader sr = new StreamReader(File.OpenRead(gradlePath)))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    var infos = propertyInfos.Where(item => { return line.Contains("ext." + item.Name.Substring(2)); });
                    if (infos != null && infos.FirstOrDefault() != null)
                    {
                        var info = infos.FirstOrDefault();
                        if (info.PropertyType == typeof(string))
                        {
                            writeLines.Append(string.Format("ext.{0} = \"{1}\"\n", info.Name.Substring(2), info.GetValue(gradleData).ToString()));
                        }
                        else
                        {
                            writeLines.Append(string.Format("ext.{0} = {1}\n", info.Name.Substring(2), info.GetValue(gradleData).ToString()));
                        }
                    }
                    else
                    {
                        writeLines.Append(line + "\n");
                    }
                }
            }
            if (File.Exists(gradlePath))
            {
                File.Delete(gradlePath);
            }
            File.WriteAllText(gradlePath, writeLines.ToString());
        }
    }
}
