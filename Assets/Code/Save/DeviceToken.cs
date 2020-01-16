using System;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif

namespace SoftLiu.Save
{
    public class DeviceToken
    {
        private string m_deviceToken = null;

        public string deviceToken
        { get { return m_deviceToken; } }

        public DeviceToken()
        {
            try
            {
                string deviceTokenPath = null;

                if (!string.IsNullOrEmpty(Application.persistentDataPath/*FGOL.Plugins.Native.NativeBinding.Instance.GetPersistentDataPath()*/))
                {
                    //Check for a device token on the device, if we can't find one we will create one
                    deviceTokenPath = Application.persistentDataPath/*FGOL.Plugins.Native.NativeBinding.Instance.GetPersistentDataPath()*/ + "/device.dt";

                    if (File.Exists(deviceTokenPath))
                    {
                        m_deviceToken = File.ReadAllText(deviceTokenPath, Encoding.UTF8);
                    }
                }

                if (m_deviceToken == null)
                {
                    System.Guid deviceToken = System.Guid.NewGuid();
                    m_deviceToken = deviceToken.ToString();

                    if (deviceTokenPath != null)
                    {
                        File.WriteAllText(deviceTokenPath, m_deviceToken, Encoding.UTF8);
#if UNITY_IPHONE && !UNITY_EDITOR
                        //Mark file as not being a file to backup
						Device.SetNoBackupFlag(deviceTokenPath);
#endif
                    }
                }
            }
            catch (Exception) { }
        }

        public override string ToString()
        {
            return m_deviceToken;
        }
    }
}
