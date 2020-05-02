using SoftLiu.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngameSaveSystem : SaveSystem
{
    public float imgamePlayerRotationSpeed = 1.5f;


    public IngameSaveSystem()
    {
        m_systemName = "IngameSaveSystem";
    }

    public override void Downgrade()
    {

    }

    public override void Load()
    {
        try
        {
            imgamePlayerRotationSpeed = GetFloat("imgameCameraMoveSpeed", 1.5f);
        }
        catch (Exception error)
        {
            Debug.LogError("IngameSaveSystem Load Error: " + error.Message);
        }
    }

    public override void Reset()
    {
        imgamePlayerRotationSpeed = 1.5f;
    }

    public override void Save()
    {
        try
        {
            SetFloat("imgameCameraMoveSpeed", imgamePlayerRotationSpeed);
        }
        catch (Exception error)
        {
            Debug.LogError("IngameSaveSystem Save Error: " + error.Message);
        }
    }

    public override bool Upgrade()
    {
        return false;
    }
}
