using SoftLiu.Build.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class BuildVersionStep : IBuildStep
    {
        [MenuItem("SoftLiu/Misc/Build Version")]
        public static void BuildVersionModify()
        {
            BuildVersionWindow.Init((buildData) =>
            {
                ModifyBuildVersion(buildData);

                string gradlePath = Path.Combine(Application.dataPath, "../JenkinsScripts/AndroidGradleStuff/launcher/build.gradle");
                BuildUtility.HandleGradleVersion(gradlePath, buildData);
            });
        }

        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target == BuildTarget.Android)
            {
                if (type == BuildType.Development)
                {
                    ModifyBuildVersion();
                }
            }
            else if (target == BuildTarget.iOS)
            {

            }
        }

        private static void ModifyBuildVersion(BuildVersionData buildData = null)
        {
            try
            {
                string versionJson = Path.Combine(Application.dataPath, "Misc/buildVersion.json");
                if (buildData != null)
                {
                    if (File.Exists(versionJson)) File.Delete(versionJson);
                    File.WriteAllText(versionJson, JsonUtility.ToJson(buildData));
                    AssetDatabase.Refresh();
                }
                else
                {
                    BuildVersionData versionData;
                    using (StreamReader sr = new StreamReader(versionJson))
                    {
                        string datas = sr.ReadToEnd();
                        versionData = JsonUtility.FromJson<BuildVersionData>(datas);
                    }
                    string versionName = versionData.defVersionName;
                    string[] versions = versionName.Split('.');
                    if (versions.Length != 4)
                    {
                        Debug.LogError("ModifyBuildVersion versionName: " + versionName);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(versions[0] + ".");
                        sb.Append(versions[1] + ".");
                        sb.Append(versions[2] + ".");
                        string versionIndex = "00";
                        if (!string.IsNullOrEmpty(versions[3]))
                        {
                            if (versions[3].Substring(0, 2) == DateTime.Now.Month.ToString("00") && versions[3].Substring(2, 2) == DateTime.Now.Day.ToString("00"))
                            {
                                string indexStr = versions[3].Substring(versions[3].Length - 2);
                                int result = 0;
                                int.TryParse(indexStr, out result);
                                result++;
                                versionIndex = result.ToString("00");
                            }
                        }
                        sb.Append(string.Format("{0}{1}{2}", DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), versionIndex));
                        if (File.Exists(versionJson)) File.Delete(versionJson);
                        File.WriteAllText(versionJson, JsonUtility.ToJson(new BuildVersionData() { defVersionName = sb.ToString(), defVersionCode = versionData.defVersionCode, defTargetSdkVersion = versionData.defTargetSdkVersion }));
                        AssetDatabase.Refresh();
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogError("ModifyBuildVersion Error: " + error.Message);
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }
    }

    public class BuildVersionWindow : EditorWindow
    {
        private static System.Action<BuildVersionData> m_callback = null;

        private string m_versionJson = string.Empty;

        private BuildVersionData m_buildVersionData = null;

        private string defVersionName = "";
        private string defVersionCode = "";
        private string defTargetSdkVersion = "";

        private static int m_displayIndex = 0;

        public static void Init(System.Action<BuildVersionData> action)
        {
            m_callback = action;
            BuildVersionWindow window = (BuildVersionWindow)EditorWindow.GetWindow(typeof(BuildVersionWindow), false, "Build Version Window", true);
            int width = 450;
            int height = 250;
            window.maxSize = new Vector2(width, height);
            window.minSize = new Vector2(width, height);
            m_displayIndex = PlayerPrefs.GetInt("BuildWindowDisplayIndex", 0);
            Debug.Log("displays: " + Display.displays.Length);
            if (m_displayIndex >= Display.displays.Length)
            {
                m_displayIndex = 0;
            }
            float x = 0;
            float y = 0;
            for (int i = 0; i < Display.displays.Length; i++)
            {
                if (i < m_displayIndex)
                {
                    x += Display.displays[i].renderingWidth;
                }
                else if (i == m_displayIndex)
                {
                    y = (Display.displays[i].renderingHeight / 2) - (height / 2);
                    x += (Display.displays[i].renderingWidth / 2) - (width / 2);
                }
            }
            Debug.Log(string.Format("x:{0} -> y:{1} -> displayIndex:{2}", x, y, m_displayIndex));
            window.position = new Rect(x, y, width, height);
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        private void OnEnable()
        {
            m_versionJson = Path.Combine(Application.dataPath, "Misc/buildVersion.json");
            if (!File.Exists(m_versionJson)) return;
            try
            {
                using (StreamReader sr = new StreamReader(m_versionJson))
                {
                    string datas = sr.ReadToEnd();
                    m_buildVersionData = JsonUtility.FromJson<BuildVersionData>(datas);
                }
                if (m_buildVersionData != null)
                {
                    defVersionName = m_buildVersionData.defVersionName;
                    defVersionCode = m_buildVersionData.defVersionCode.ToString();
                    defTargetSdkVersion = m_buildVersionData.defTargetSdkVersion.ToString();
                }
            }
            catch (Exception error)
            {
                Debug.LogError("BuildVersionWindow OnEnable Error: " + error.Message);
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.Label(string.Format("Position: x={0} , y={1}", position.x, position.y));
            GUILayout.Label("修改版本的 Version 信息，同时写入 gradle 打包文件里。");
            GUILayout.EndVertical();

            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.Label("defVersionName: ");
            defVersionName = GUILayout.TextArea(defVersionName, GUILayout.Width(150), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("defVersionCode: ");
            defVersionCode = GUILayout.TextField(defVersionCode, GUILayout.Width(150), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("defTargetSdkVersion: ");
            defTargetSdkVersion = GUILayout.TextField(defTargetSdkVersion, GUILayout.Width(150), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Sure"))
            {
                Debug.Log("Sure On Click");
                string versionName = defVersionName.Trim();
                string[] names = versionName.Split('.');
                if (names.Length != 4)
                {
                    Debug.LogError("defVersionName Error: " + defVersionName);
                    return;
                }
                int versionCode = 0;
                int.TryParse(defVersionCode, out versionCode);
                int targetSdkVersion = 0;
                int.TryParse(defTargetSdkVersion, out targetSdkVersion);
                m_buildVersionData = new BuildVersionData() { defVersionName = versionName, defVersionCode = versionCode, defTargetSdkVersion = targetSdkVersion };
                Debug.Log("Build Version Data ： " + JsonUtility.ToJson(m_buildVersionData));
                if (m_callback != null)
                    m_callback(m_buildVersionData);
            }
            GUILayout.EndVertical();

            float x = position.x;
            float sumX = 0;
            for (int i = 0; i < Display.displays.Length; i++)
            {
                sumX += Display.displays[i].renderingWidth;
                if (sumX > x)
                {
                    sumX = 0;
                    if (m_displayIndex != i)
                    {
                        m_displayIndex = i;
                        PlayerPrefs.SetInt("BuildWindowDisplayIndex", m_displayIndex);
                    }
                    continue;
                }
            }
        }
    }
}
