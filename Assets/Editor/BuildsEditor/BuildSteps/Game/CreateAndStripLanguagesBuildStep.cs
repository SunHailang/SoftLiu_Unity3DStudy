using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            CreateAndStripLanguagesBuildStep langStep = new CreateAndStripLanguagesBuildStep();
            langStep.Execute(BuildTarget.Android, BuildType.Production, "");
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

        private static void SplitLocalisation()
        {
            try
            {
                List<string> DuplicateKeys = new List<string>();
                Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();
                // Try to load the Localization CSV
                StreamReader sr = new StreamReader(Application.dataPath + "/Misc/Localization/Localization.csv");

                List<string> languageKeys = new List<string>(sr.ReadLine().Split(','));
                languageKeys.Remove("KEY");
                foreach (string languageKey in languageKeys)
                {
                    languages.Add(languageKey, new Dictionary<string, string>());
                }
                Debug.Log("languages Length: " + languages.Count);
                //build into maps
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> loc = new List<string>();
                    bool inQuote = false;
                    int imax = line.Length;
                    for (int i = 0; i < imax; ++i)
                    {
                        char x = line[i];
                        if (x == '"')
                        {
                            inQuote = !inQuote;
                        }
                        else if (x == ',' && !inQuote)
                        {
                            loc.Add(line.Substring(0, i));
                            line = line.Substring(i);
                            i = 0;
                            imax = line.Length;
                        }
                    }

                    string key = loc[0];
                    int index = 1;
                    foreach (string languageKey in languageKeys)
                    {
                        try
                        {
                            languages[languageKey].Add(key, loc[index]);
                        }
                        catch (ArgumentException)
                        {
                            if (!DuplicateKeys.Contains(key))
                            {
                                DuplicateKeys.Add(key);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.ToString());
                            Debug.LogError("LOCALISATION STRIPPING FAILED");
                            Debug.LogError("LINE:" + line);
                            Debug.LogError(languageKey + " : " + key + " = " + index);
                        }
                        index++;
                    }
                }

                sr.Close();
                //if (!BuildProcess.IsJenkinsBuild && DuplicateKeys.Count > 0)
                //{
                //    string message = "The following keys have duplicate entries in the localization file:";
                //    for (int i = 0; i < DuplicateKeys.Count; i++)
                //    {
                //        message += "\n" + DuplicateKeys[i];
                //    }
                //    EditorUtility.DisplayDialog("Duplicate Keys Found", message, "sadness");
                //    return;
                //}

                if (BuildProcess.IsJenkinsBuild)
                {
                    //File.Delete(Application.dataPath + "/Misc/Localization/Localization.csv");
                }
                //sweet, got it all.

                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in languages)
                {
                    string fileName = "Localization_" + kvp.Key + ".csv";
                    StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/" + fileName);
                    sw.WriteLine("KEY," + kvp.Key);
                    foreach (KeyValuePair<string, string> stringLoc in kvp.Value)
                    {
                        sw.WriteLine(stringLoc.Key + stringLoc.Value);
                    }
                    sw.Close();
                }
            }
            catch (Exception error)
            {
                Debug.LogError("SplitLocalisation Error: " + error.Message);
            }
            Debug.Log("Complete Language");
        }
    }
}