using System;
using SoftLiu.Save;
using CodeStage.AntiCheat.ObscuredTypes;

public class BankSaveSystem : SaveSystem
{

    public ObscuredInt earnedCoins = 0;
    public ObscuredInt purchasedCoins = 0;

    public ObscuredInt earnedGems = 0;
    public ObscuredInt purchasedGems = 0;

    public ObscuredInt earnedPearls = 0;
    public ObscuredInt purchasedPearls = 0;

    public BankSaveSystem()
    {
        m_systemName = "BankSaveSystem";
        Reset();
    }

    public override void Downgrade()
    {

    }

    public override void Load()
    {
        try
        {
            earnedCoins = GetInt("EarnedCoins");
            purchasedCoins = GetInt("PurchasedCoins", 0, true);

            earnedGems = GetInt("EarnedGems");
            purchasedGems = GetInt("PurchasedGems", 0, true);

            earnedPearls = GetInt("EarnedPearls");
            purchasedPearls = GetInt("PurchasedPearls", 0, true);
        }
        catch (Exception error)
        {
            Debug.LogError("BankSaveSystem Load Error: " + error.Message);
        }
    }

    public override void Reset()
    {
        try
        {
            earnedCoins = 0;
            purchasedCoins = 0;

            earnedGems = 0;
            purchasedGems = 0;

            earnedPearls = 0;
            purchasedPearls = 0;
        }
        catch (Exception error)
        {
            Debug.LogError("BankSaveSystem Reset Error: " + error.Message);
        }
    }

    public override void Save()
    {
        try
        {
            SetInt("EarnedCoins", earnedCoins);
            SetInt("PurchasedCoins", purchasedCoins, true);

            SetInt("EarnedGems", earnedGems);
            SetInt("PurchasedGems", purchasedGems, true);

            SetInt("EarnedPearls", earnedPearls);
            SetInt("PurchasedPearls", purchasedPearls, true);
        }
        catch (Exception error)
        {
            Debug.LogError("BankSaveSystem Save Error: " + error.Message);
        }
    }

    public override bool Upgrade()
    {
        return false;
    }
}

