using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class JSONMinifyTool
{
    public static string Minify(string inputString)
    {
        return Regex.Replace(inputString, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
    }

    public static void MinifyFile(string path)
    {
        Debug.Log("Minifying JSON file " + path);

        string contents = File.ReadAllText(path);
        contents = Regex.Replace(contents, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

        File.WriteAllText(path, contents);
    }
}
