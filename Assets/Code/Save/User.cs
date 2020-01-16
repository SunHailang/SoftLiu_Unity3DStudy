using SoftLiu.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{

    private bool m_cloudSaveAvailable = false;

    private bool m_loaded = false;

    private string m_userID = null;
    private string m_saveID = null;
    private string m_nickName = "";
    private string m_flag = "";
    public string ID
    {
        get { return m_userID; }
        set { m_userID = value; }
    }

    public string saveID
    {
        get { return m_saveID; }
        set
        {
            Debug.Log("SaveID updated");
            m_saveID = value;
        }
    }

    public User()
    {

    }

    public void Load()
    {
        if (!m_loaded)
        {
            string userID = PlayerPrefs.GetString("LocalUserID", "UserID");

            if (!string.IsNullOrEmpty(userID))
            {
                m_userID = userID;
            }

            string saveID = PlayerPrefs.GetString("LocalSaveID", "SaveID");

            if (!string.IsNullOrEmpty(saveID))
            {
                m_saveID = saveID;
            }

            string username = PlayerPrefs.GetString("Username", "name");

            if (!string.IsNullOrEmpty(username))
            {
                m_nickName = username;
            }
            else
            {
                m_nickName = "";
            }

            string userflag = PlayerPrefs.GetString("Userflag", "m_flag");
            if (!string.IsNullOrEmpty(userflag))
            {
                m_flag = string.IsNullOrEmpty(userflag) ? "flag_generic" : userflag;
            }
            else
            {
                m_flag = "flag_generic";
            }


            m_cloudSaveAvailable = PlayerPrefs.GetInt("LocalUserCloudSaveAvailable", 0) == 1;

            string socialNetworksJson = PlayerPrefs.GetString("LocalUserAssociatedNetworks");

            m_loaded = true;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("LocalUserID", m_userID);
        PlayerPrefs.SetString("LocalSaveID", m_saveID);
        PlayerPrefs.SetString("Username", m_nickName);
        PlayerPrefs.SetString("Userflag", m_flag);
        PlayerPrefs.SetInt("LocalUserCloudSaveAvailable", m_cloudSaveAvailable ? 1 : 0);
    }

    public static void Clear(bool loginCredentials = true)
    {
        PlayerPrefs.DeleteKey("LocalUserID");
        PlayerPrefs.DeleteKey("LocalSaveID");
        PlayerPrefs.DeleteKey("LocalUserCloudSaveAvailable");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.Save();
    }
}
