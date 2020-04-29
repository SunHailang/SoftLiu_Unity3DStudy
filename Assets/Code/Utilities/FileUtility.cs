using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public static class FileUtility
{

    /// <summary>
    /// 拷贝一个文件夹内容到另一个文件夹下， 并覆盖
    /// </summary>
    /// <param name="source">源文件夹</param>
    /// <param name="target">目标文件夹</param>
    /// <param name="child">是否包含子文件夹</param>
    /// <param name="withoutExtensions">不包含的 扩展名 ， eg: .meta </param>
    public static void CopyDirectoryFiles(DirectoryInfo source, DirectoryInfo target, bool child = false, bool overwrite = false, params string[] withoutExtensions)
    {
        try
        {
            if (child)
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    CopyDirectoryFiles(dir, target.CreateSubdirectory(dir.Name), child, overwrite, withoutExtensions);
                }
            }
            foreach (FileInfo file in source.GetFiles())
            {
                if (withoutExtensions != null && withoutExtensions.Contains(file.Extension))
                {
                    continue;
                }
                file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
            }
        }
        catch (Exception error)
        {
            Debug.LogError("CopyFileRecursively Error: " + error.Message);
        }
    }

    /// <summary>
    /// 删除文件夹，及其文件内所有文件
    /// </summary>
    /// <param name="source">文件夹目录</param>
    /// <param name="self">是否包含自己</param>
    public static void DeleteDirectory(string source, bool self = false)
    {
        if (Directory.Exists(source))
        {
            DirectoryInfo info = new DirectoryInfo(source);
            foreach (var item in info.GetDirectories())
            {
                DeleteDirectory(item.FullName, true);
            }
            FileInfo[] files = info.GetFiles();
            for (int i = files.Length - 1; i >= 0; i--)
            {
                File.Delete(files[i].FullName);
            }
            if (self)
            {
                Directory.Delete(source);
            }
        }
    }


    public static string GetContentDispositionByName(string disposition, string name)
    {
        string result = null;
        if (!string.IsNullOrEmpty(disposition) && !string.IsNullOrEmpty(name))
        {
            string[] disArray = disposition.Split(';');
            for (int i = 0; i < disArray.Length; i++)
            {
                string[] keyvalues = disArray[i].Split('=');
                if (keyvalues.Length == 2)
                {
                    string key = keyvalues[0].Trim();
                    string value = keyvalues[1].Trim();
                    if (key.Equals(name))
                    {
                        result = value;
                        break;
                    }
                }
            }
        }
        return result;
    }
}
