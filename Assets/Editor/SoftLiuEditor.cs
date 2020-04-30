using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SoftLiuEditor
{
    [MenuItem("SoftLiu/Utility/Fonts/Text", priority = 1)]
    public static void FontsTextUpdate()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "../Tools/Fonts/newText.txt");
            string gameTextPath = Path.Combine(Application.dataPath, "Resources/Localization_Chinese.csv");
            if (File.Exists(path))
            {
                UnityEngine.Debug.Log("strat");
                bool overwrite = false;

                string str = File.ReadAllText(path);
                string gameText = File.ReadAllText(gameTextPath);

                StringBuilder sb = new StringBuilder();
                List<char> charLins = new List<char>();
                char[] strArrary = str.ToCharArray();
                charLins.AddRange(strArrary);
                for (int i = 0; i < gameText.Length; i++)
                {
                    char textChar = gameText[i];
                    if (textChar == '\n' || textChar == ' ' || textChar == '\t' || textChar == '\0' || textChar == '\r')
                    {
                        continue;
                    }
                    if (!charLins.Contains(textChar))
                    {
                        charLins.Add(textChar);
                        UnityEngine.Debug.Log("new word: " + textChar);
                        overwrite = true;
                    }
                }

                for (int i = 0; i < charLins.Count; i++)
                {
                    sb.Append(charLins[i]);
                }
                File.WriteAllText(path, sb.ToString());
                if (overwrite)
                {
                    string gamePath = Path.Combine(Application.dataPath, "Arts/GUI/Fonts/fontText.txt");
                    if (!File.Exists(gamePath))
                        Debug.LogError(gamePath + " is not exist.");
                    File.WriteAllText(gamePath, sb.ToString());
                    AssetDatabase.Refresh();
                }
                UnityEngine.Debug.Log("finish");
            }
            else
            {
                Debug.LogError("file not exists.");
            }
        }
        catch (System.Exception error)
        {
            UnityEngine.Debug.LogError("FontsTextUpdate Error: " + error.Message);
        }
    }
}

