using SoftLiu.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

public class UserSaveSystem : SaveSystem
{
    public bool userTouristMode = false;
    public ObscuredBool isHacker = false;


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
            userTouristMode = GetBool("UserTouristMode", false);
            isHacker = GetBool("IsHacker", false);
        }
        catch (Exception error)
        {
            Debug.LogError("UserSaveSystem Load Error: " + error.Message);
        }
    }

    public override void Reset()
    {
        userTouristMode = false;
        isHacker = false;
    }

    public override void Save()
    {
        try
        {
            SetBool("UserTouristMode", userTouristMode);
            SetBool("IsHacker", isHacker);
        }
        catch (Exception error)
        {
            Debug.LogError("UserSaveSystem Save Error: " + error.Message);
        }
    }

    public override bool Upgrade()
    {
        return false;
    }
}
