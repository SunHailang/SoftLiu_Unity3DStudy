using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GameDataBuilder
{
    public static string BinaryDataPath
    {
        get
        {
            return string.Format("Assets/Resources/{0}.gdb.bytes", GameDataManager.GameDBPath);
        }
    }

    public static void BuildGameData()
    {
        Debug.Log("GameDataBuilder (BuildGameData) :: Building game data");

        string processGameDBPath = BinaryDataPath;

        string json = JSONMinifyTool.Minify(File.ReadAllText(GameDataImporter.GameDBJsonPath));
        byte[] encrypted = GameDataUtils.EncryptGameData(json, Encoding.Default);
        if (encrypted != null)
        {
            using (FileStream fs = new FileStream(processGameDBPath, FileMode.Create, FileAccess.Write))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(encrypted, 0, encrypted.Length);
                    ms.WriteTo(fs);
                }
            }
        }
        else
        {
            throw new Exception("Cannot encrypt gameDB! Failing the build.");
        }
        AssetDatabase.Refresh();
    }
}
