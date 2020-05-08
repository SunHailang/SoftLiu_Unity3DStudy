using SoftLiu.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnalyticsSaveSystem : SaveSystem
{

    public int sessionCount;                // Analytics sessions cound
    public string analyticsUserID;          // Analytics user ID generated upon first time session is started, uses GUID as we don't have server at this point


    public AnalyticsSaveSystem()
    {
        m_systemName = "AnalyticsSaveSystem";
        Reset();
    }

    public override void Downgrade()
    {

    }

    public override void Load()
    {
        try
        {
            sessionCount = GetInt("SessionCount", 0);
            analyticsUserID = GetString("AnalyticsUserID");
        }
        catch (Exception error)
        {
            Debug.LogError("AnalyticsSaveSystem Load Error: " + error.Message);
        }
    }

    public override void Reset()
    {
        try
        {
            sessionCount = 0;
            analyticsUserID = "";
        }
        catch (Exception error)
        {
            Debug.LogError("AnalyticsSaveSystem Reset Error: " + error.Message);
        }
    }

    public override void Save()
    {
        try
        {
            SetInt("SessionCount", sessionCount);
            SetString("AnalyticsUserID", analyticsUserID);
        }
        catch (Exception error)
        {
            Debug.LogError("AnalyticsSaveSystem Save Error: " + error.Message);
        }
    }

    public override bool Upgrade()
    {
        return false;
    }
}

