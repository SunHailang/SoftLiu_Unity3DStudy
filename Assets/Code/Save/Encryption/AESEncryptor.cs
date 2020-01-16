using SoftLiu.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SoftLiu.Save.Encryption
{
    public class AESEncryptor
    {
        public const int KeySize = 128;

        public static byte[] Encrypt(byte[] key, byte[] iv, byte[] data)
        {
            byte[] encrypted = null;

            try
            {
                using (RijndaelManaged crypto = new RijndaelManaged())
                {
                    crypto.BlockSize = KeySize;
                    crypto.Padding = PaddingMode.PKCS7;
                    crypto.Key = key;
                    crypto.IV = iv;
                    crypto.Mode = CipherMode.CBC;

                    ICryptoTransform encryptor = crypto.CreateEncryptor(crypto.Key, crypto.IV);

                    using (MemoryStream encMemStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(encMemStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();

                            encrypted = encMemStream.ToArray();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("AESEncryptor :: Encryption failed with exception: " + e.Message);
                Debug.LogError(e);
            }

            return encrypted;
        }

        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] data)
        {
            byte[] decrypted = null;

            try
            {
                using (RijndaelManaged crypto = new RijndaelManaged())
                {
                    crypto.BlockSize = KeySize;
                    crypto.Padding = PaddingMode.PKCS7;
                    crypto.Key = key;
                    crypto.IV = iv;
                    crypto.Mode = CipherMode.CBC;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = crypto.CreateDecryptor(crypto.Key, crypto.IV);

                    using (MemoryStream memStream = new MemoryStream(data))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (MemoryStream decMemStream = new MemoryStream())
                            {
                                var buffer = new byte[512];
                                var bytesRead = 0;

                                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    decMemStream.Write(buffer, 0, bytesRead);
                                }

                                decrypted = decMemStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("AESEncryptor :: Decryption failed with exception: " + e.Message);
                Debug.LogError(e);
            }

            return decrypted;
        }

        public static Dictionary<string, object> DecryptDecodeDeserialize(byte[] key, byte[] iv, byte[] data, Encoding encoding)
        {
            try
            {
                using (RijndaelManaged crypto = new RijndaelManaged())
                {
                    crypto.BlockSize = KeySize;
                    crypto.Padding = PaddingMode.PKCS7;
                    crypto.Key = key;
                    crypto.IV = iv;
                    crypto.Mode = CipherMode.CBC;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = crypto.CreateDecryptor(crypto.Key, crypto.IV);

                    using (MemoryStream memStream = new MemoryStream(data))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream, encoding))
                            {
                                return MiniJSON.Deserialize(streamReader) as Dictionary<string, object>;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("AESEncryptor :: Decryption failed with exception: " + e.Message);
                Debug.LogError(e);
            }

            return null;
        }
    }
}