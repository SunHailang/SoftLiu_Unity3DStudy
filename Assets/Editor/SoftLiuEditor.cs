using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Specialized;

public class SoftLiuEditor
{

    [MenuItem("SoftLiu/PSDtoPNG", priority = 5)]
    public static void PSDtoPNG()
    {
        // 获取选择的对象列表
        GameObject[] selectList = Selection.GetFiltered<GameObject>(SelectionMode.TopLevel);
        Debug.Log("egret tools： " + selectList.Length);
        for (int i = 0; i < selectList.Length; i++)
        {
            UnityEngine.Debug.Log(selectList[i].gameObject.name);
            //Egret3DExportTools.ExportPrefabTools.ExportPrefab(null);
        }
    }

    [MenuItem("SoftLiu/PrintSelect", priority = 0)]
    public static void callbackEgretTools()
    {



    }

    [MenuItem("SoftLiu/Fonts/Text", priority = 1)]
    public static void FontsText()
    {
        string path = Application.dataPath + "/Resources/newText.txt";
        string path1 = Application.dataPath + "/Resources/newText1.txt";
        if (File.Exists(path))
        {
            Debug.Log("strat");
            string str = File.ReadAllText(path);
            string str1 = File.ReadAllText(path1);
            StringCollection sc = new StringCollection();
            StringBuilder sb = new StringBuilder();
            List<char> charLins = new List<char>();
            Debug.Log(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n' || str[i] == ' ' || str[i] == '\t' || str[i] == '\0')
                {
                    continue;
                }
                if (str[i] == '\r')
                {
                    continue;
                }
                if (!charLins.Contains(str[i]))
                {
                    charLins.Add(str[i]);
                }
                //if (!str1.Contains(str[i].ToString()))
                //{
                //    Debug.Log((int)str[i]);
                //}
            }

            for (int i = 0; i < charLins.Count; i++)
            {
                sb.Append(charLins[i]);
            }
            Debug.Log(sb.Length);
            File.WriteAllText(path, sb.ToString());
            Debug.Log("finish");
        }
        else
        {
            Debug.LogError("file not exists.");
        }
    }
}
