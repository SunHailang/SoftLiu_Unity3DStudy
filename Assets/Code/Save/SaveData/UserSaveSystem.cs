using SoftLiu.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserSaveSystem : SaveSystem
{
    public bool userTouristMode = false;



    public UserSaveSystem()
    {
        m_systemName = "UserSaveSystem";
    }

    public override void Downgrade()
    {

    }

    public override void Load()
    {
        try
        {
            userTouristMode = GetBool("userTouristMode", false);
        }
        catch (Exception error)
        {
            Debug.LogError("UserSaveSystem Load Error: " + error.Message);
        }
    }

    public override void Reset()
    {
        userTouristMode = false;
    }

    public override void Save()
    {
        SetBool("userTouristMode", userTouristMode);
    }

    public override bool Upgrade()
    {
        return false;
    }
}
