using SoftLiu.Save.Encryption;
using SoftLiu.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GameDataUtils
{
    public const string PKIV = "MUItOEItQkYtODAtM0YtMzMtMTEtNjItNTEtREUtMkQtREQtQ0ItNjktNDMtMzcvREItMDktQzAtQUYtMTEtNjMtQjYtNDItNjMtREMtN0YtMkEtREQtQjYtMzUtNjQ=";

    public static Dictionary<string, object> GetMergedData(Dictionary<string, object> baseData, string overrideJson)
    {
        Dictionary<string, object> mergedData = null;

        Dictionary<string, object> overrideData = MiniJSON.Deserialize(overrideJson) as Dictionary<string, object>;

        if (overrideData != null)
        {
            mergedData = MergeGameDictionaries(baseData, overrideData);
        }
        else
        {
            Debug.LogError("GameDataUtils :: Failed to deserialize cached gameDB json!");
        }

        return mergedData;
    }

    public static byte[] EncryptGameData(string gameData, Encoding encoding)
    {
        byte[] encrypted = null;

        using (MemoryStream compMemStream = new MemoryStream())
        {
            using (StreamWriter writer = new StreamWriter(compMemStream, encoding))
            {
                writer.Write(gameData);
                writer.Close();

                encrypted = EncryptBytes(compMemStream.ToArray());
            }
        }

        return encrypted;
    }

    public static string DecryptGameData(byte[] encrypted, Encoding encoding)
    {
        string decrypted = null;

        try
        {
            byte[] decryptedData = DecryptBytes(encrypted);

            if (decryptedData != null)
            {
                decrypted = encoding.GetString(decryptedData);
            }
            else
            {
                Debug.LogError("GameDataUtils (DecryptGameData):: Error Decrypting Game DB");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("GameDataUtils (DecryptGameData):: Error Decrypting Game DB - " + e);
        }

        return decrypted;
    }

    public static byte[] EncryptBytes(byte[] bytes)
    {
        byte[] encrypted = null;

        string[] parts = Encoding.UTF8.GetString(Convert.FromBase64String(PKIV)).Split('/');
        encrypted = AESEncryptor.Encrypt(KeyToBytes(parts[0]), KeyToBytes(parts[1]), bytes);

        return encrypted;
    }

    public static byte[] DecryptBytes(byte[] bytes)
    {
        byte[] decrypted = null;

        try
        {
            string[] parts = Encoding.UTF8.GetString(Convert.FromBase64String(PKIV)).Split('/');
            decrypted = AESEncryptor.Decrypt(KeyToBytes(parts[0]), KeyToBytes(parts[1]), bytes);
        }
        catch (Exception e)
        {
            Debug.LogError("GameDataUtils (DecryptBytes):: Error Dencrypting Bytes - " + e);
        }

        return decrypted;
    }

    public static Dictionary<string, object> DecryptDecodeDeserialize(byte[] bytes, Encoding encoding)
    {
        try
        {
            string[] parts = Encoding.UTF8.GetString(Convert.FromBase64String(PKIV)).Split('/');
            return AESEncryptor.DecryptDecodeDeserialize(KeyToBytes(parts[0]), KeyToBytes(parts[1]), bytes, encoding);
        }
        catch (Exception e)
        {
            Debug.LogError("GameDataUtils (DecryptBytes):: Error Dencrypting Bytes - " + e);
        }

        return null;
    }

    public static byte[] KeyToBytes(string key)
    {
        return Array.ConvertAll<string, byte>(key.Split('-'), s => Convert.ToByte(s, 16));
    }

    private static Dictionary<string, object> MergeGameDictionaries(Dictionary<string, object> baseGameData, Dictionary<string, object> overrideGameData)
    {
        Dictionary<string, object> merged = baseGameData;

        foreach (KeyValuePair<string, object> pair in overrideGameData)
        {
            if (baseGameData.ContainsKey(pair.Key))
            {
                if (pair.Value != null)
                {
                    Dictionary<string, object> childDict = pair.Value as Dictionary<string, object>;

                    if (childDict != null)
                    {
                        Dictionary<string, object> baseChildDict = baseGameData[pair.Key] as Dictionary<string, object>;

                        if (baseChildDict != null)
                        {
                            merged[pair.Key] = MergeGameDictionaries(baseChildDict, childDict);
                        }
                        else
                        {
                            merged[pair.Key] = childDict;
                        }
                    }
                    else
                    {
                        List<object> childList = pair.Value as List<object>;

                        if (childList != null)
                        {
                            List<object> baseChildList = baseGameData[pair.Key] as List<object>;

                            if (baseChildList != null)
                            {
                                merged[pair.Key] = MergeGameLists(baseChildList, childList);
                            }
                            else
                            {
                                merged[pair.Key] = childList;
                            }
                        }
                        else
                        {
                            merged[pair.Key] = pair.Value;
                        }
                    }

                }
                else
                {
                    merged.Remove(pair.Key);
                }
            }
            else if (pair.Value != null)
            {
                merged.Add(pair.Key, pair.Value);
            }
        }

        return merged;
    }

    private static List<object> MergeGameLists(List<object> baseData, List<object> overrideData)
    {
        List<object> merged = baseData.GetRange(0, Math.Min(baseData.Count, overrideData.Count));

        for (int i = 0; i < overrideData.Count; i++)
        {
            if (overrideData[i] != null)
            {
                if (i < baseData.Count)
                {
                    List<object> childList = overrideData[i] as List<object>;

                    if (childList != null)
                    {
                        List<object> baseChildList = baseData[i] as List<object>;

                        if (baseChildList != null)
                        {
                            merged[i] = MergeGameLists(baseChildList, childList);
                        }
                        else
                        {
                            merged[i] = childList;
                        }
                    }
                    else
                    {
                        Dictionary<string, object> childDict = overrideData[i] as Dictionary<string, object>;

                        if (childDict != null)
                        {
                            Dictionary<string, object> baseChildDict = baseData[i] as Dictionary<string, object>;

                            if (baseChildDict != null)
                            {
                                merged[i] = MergeGameDictionaries(baseChildDict, childDict);
                            }
                            else
                            {
                                merged[i] = childDict;
                            }
                        }
                        else
                        {
                            merged[i] = overrideData[i];
                        }
                    }
                }
                else
                {
                    merged.Add(overrideData[i]);
                }
            }
        }

        return merged;
    }
}
