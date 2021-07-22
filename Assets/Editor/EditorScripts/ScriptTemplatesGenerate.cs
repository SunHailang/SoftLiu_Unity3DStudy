using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptTemplatesGenerate
{
    // 模板的位置
    static string tempCshapeMonoBehaviourPath = Application.dataPath + "/Editor/EditorScripts/NewBehaviourScriptTemplates.txt";
    static string tempCshapePath = Application.dataPath + "/Editor/EditorScripts/NewScriptTemplates.txt";

    [MenuItem("Assets/Create/My C# Script", false, 80)]
    public static void CreateCShapeScrtip()
    {
        ScriptGenerate(false);
    }

    [MenuItem("Assets/Create/My C# Behaviour Script", false, 80)]
    public static void CreateBehaviourCShapeScrtip()
    {
        ScriptGenerate(true);
    }

    private static void ScriptGenerate(bool isMono)
    {
        try
        {
            if (isMono)
                ProjectWindowUtil.CreateAssetWithContent("NewBehaviourScript.cs", 
                    File.ReadAllText(tempCshapeMonoBehaviourPath), 
                    EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
            else
                ProjectWindowUtil.CreateAssetWithContent("NewScript.cs", 
                    File.ReadAllText(tempCshapePath), 
                    EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("模板文件路径错误!!! " + ex.Message);
        }
    }

    /// <summary>
    /// 给脚本添加标题头
    /// </summary>
    class AddFileHeadComment : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// 此函数在asset被创建，文件已经生成到磁盘上，生成了.meta文件没有正式创建完成之间调用(我觉得) 和import之前被调用
        /// </summary>
        /// <param name="newFileMeta">newfilemeta 是由创建文件的path加上.meta组成的</param>
        public static void OnWillCreateAsset(string newFileMeta)
        {

            //把meta去掉
            string newFilePath = newFileMeta.Replace(".meta", "");
            //得到扩展名
            string fileExt = Path.GetExtension(newFilePath);

            if (fileExt != ".cs") return;

            string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
            string scriptContent = File.ReadAllText(realPath);

            //这里实现自定义的一些规则
            scriptContent = scriptContent.Replace("#SCRIPTNAME#", Path.GetFileName(Path.GetFileNameWithoutExtension(newFilePath)));
            //scriptContent = scriptContent.Replace("#COMPANY#", PlayerSettings.companyName);
            scriptContent = scriptContent.Replace("#AUTHOR#", "hlsun");
            scriptContent = scriptContent.Replace("#DESC#", "文件描述");
            scriptContent = scriptContent.Replace("#VERSION#", "1.0");
            scriptContent = scriptContent.Replace("#UNITYVERSION#", Application.unityVersion);
            scriptContent = scriptContent.Replace("#DATE#", DateTime.Now.ToString("yyyy-MM-dd"));

            File.WriteAllText(realPath, scriptContent);
            AssetDatabase.ImportAsset(newFilePath);
        }
    }
}
