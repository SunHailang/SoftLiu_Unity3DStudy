using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using System;
using UnityEngine;
using Assets.Code.Utils.ZipData;
using System.Threading.Tasks;

public static class SharpZipUtility
{

    public static void GetChildDicsName(string path, ZipHandlerData zipData)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        zipData.fileCount += dir.GetFiles().Length;
        DirectoryInfo[] childDirs = dir.GetDirectories();

        if (childDirs != null && childDirs.Length > 0)
        {
            foreach (DirectoryInfo dirChild in childDirs)
            {
                GetChildDicsName(dirChild.FullName, zipData);
            }
        }
    }

    /// <summary>
    /// 压缩 文件夹
    /// </summary>
    /// <param name="strFile">要压缩的文件夹</param>
    /// <param name="strZip">要压缩成的.zip文件</param>
    public static ZipHandlerData ZipFie(string strFile, string strZip, ZipHandlerData zipData)
    {
        Task.Factory.StartNew((data) =>
        {
            ZipHandlerData zip = (ZipHandlerData)data;

            using (ZipOutputStream outStream = new ZipOutputStream(File.Create(strZip)))
            {
                outStream.SetLevel(6);
                GetChildDicsName(strFile, zip);
                ZipCompress(strFile, outStream, strFile, zip);
                zip.isDone = true;
                outStream.Finish();
            }

        }, zipData, TaskCreationOptions.None);

        return zipData;
    }
    private static void ZipCompress(string strFile, ZipOutputStream outstream, string staticFile, ZipHandlerData zipData)
    {
        string curFile = string.Empty;
        try
        {
            Crc32 crc = new Crc32();
            //获取指定目录下所有文件和子目录文件名称
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            //遍历文件
            foreach (string file in filenames)
            {
                curFile = file;
                if (Directory.Exists(file))
                {
                    ZipCompress(file, outstream, staticFile, zipData);
                }
                //否则，直接压缩文件
                else
                {
                    //打开文件
                    using (FileStream fs = File.OpenRead(file))
                    {
                        //定义缓存区对象
                        byte[] buffer = new byte[fs.Length];
                        //通过字符流，读取文件
                        fs.Read(buffer, 0, buffer.Length);
                        //得到目录下的文件（比如:D:\Debug1\test）,test
                        int index = staticFile.TrimEnd('/', '\\').LastIndexOfAny(new char[] { '/', '\\' });
                        string tempfile = file.Substring(index + 1);
                        ZipEntry entry = new ZipEntry(tempfile);
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;
                        outstream.PutNextEntry(entry);
                        //写文件
                        outstream.Write(buffer, 0, buffer.Length);
                    }
                    zipData.fileCurrentCount++;
                }
            }
        }
        catch (Exception e)
        {
            zipData.isError = true;
            zipData.errorText = string.Format("File Name: {0} , Error: {1}", curFile, e.Message);
        }
    }
    /// <summary>
    /// 解压Zip文件
    /// </summary>
    /// <param name="TargetFile">需要解压的文件  .zip  文件</param>
    /// <param name="fileDir">解压到的文件夹</param>
    /// <returns></returns>
    public static ZipHandlerData UnZipFile(string TargetFile, string fileDir, ZipHandlerData zipData)
    {
        string rootFile = "";
        try
        {
            Task.Factory.StartNew((zipdata) =>
            {
                ZipHandlerData zip = (ZipHandlerData)zipdata;
                if (!File.Exists(TargetFile))
                {
                    zipData.isError = true;
                    zipData.errorText = string.Format("Error: {0} File Not Exists.", TargetFile);
                    //return zipData;
                }
                else
                {
                    //string filePath = Path.GetFileNameWithoutExtension(TargetFile);
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    //读取压缩文件（zip文件），准备解压缩
                    using (ZipInputStream inputstream = new ZipInputStream(File.OpenRead(TargetFile.Trim())))
                    {
                        ZipEntry entry;
                        using (ZipFile zipFile = new ZipFile(TargetFile.Trim()))
                        {
                            Debug.Log(string.Format("ZipFile Count: {0}", zipFile.Count));
                            zip.fileCount = zipFile.Count;
                        }

                        while ((entry = inputstream.GetNextEntry()) != null)
                        {
                            string rootDir = Path.GetDirectoryName(entry.Name);
                            string fileDirChild = fileDir + "/" + rootDir;
                            if (!Directory.Exists(fileDirChild))
                            {
                                Directory.CreateDirectory(fileDirChild);
                            }
                            string fileNameChild = Path.GetFileName(entry.Name);
                            rootFile = fileNameChild;
                            if (!string.IsNullOrEmpty(fileNameChild))
                            {
                                using (FileStream fs = File.Create(fileDirChild + "/" + fileNameChild))
                                {
                                    int size = 0;
                                    byte[] data = new byte[1024 * 2];
                                    while ((size = inputstream.Read(data, 0, data.Length)) > 0)
                                    {
                                        fs.Write(data, 0, size);
                                    }
                                    zip.fileCurrentCount++;
                                }
                            }
                        }
                    }
                }
                zipData.isDone = true;
            }, zipData, TaskCreationOptions.None);
        }
        catch (Exception ex)
        {
            zipData.isError = true;
            zipData.errorText = string.Format("UnZipFile: {0}, Error: {1}", rootFile, ex.Message);
        }
        return zipData;
    }
}
