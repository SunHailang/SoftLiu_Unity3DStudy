using SoftLiu.Event;
using SoftLiu.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bank
{

    public enum CurrencyType
    {
        Coins,
        Gems,
        Pearls
    }

    public static string GetCurrencyTextSymbol(CurrencyType type, bool small = false)
    {
        string currencySymbol = null;
        string currencySymbolText = null;

        switch (type)
        {
            case Bank.CurrencyType.Coins:
                currencySymbol = small ? "[COINSMALL]" : "[COIN]";
                currencySymbolText = Localization.Instance.Get("STRING_IAP_TAB_COINS");
                break;
            case Bank.CurrencyType.Gems:
                currencySymbol = small ? "[GEMSMALL]" : "[GEM]";
                currencySymbolText = Localization.Instance.Get("STRING_IAP_TAB_GEMS");
                break;
            case Bank.CurrencyType.Pearls:
                currencySymbol = small ? "[PEARLSMALL]" : "[PEARL]";
                currencySymbolText = Localization.Instance.Get("STRING_IAP_TAB_PEARLS");
                break;
        }

        if (Localization.Instance.language == "Chinese")
        {
            currencySymbol = currencySymbolText;
        }

        return currencySymbol;
    }

    public static string GetCurrencyText(CurrencyType type)
    {
        string currencyText = null;

        switch (type)
        {
            case Bank.CurrencyType.Coins:
                currencyText = Localization.Instance.Get("STRING_GLOBAL_COINS");
                break;
            case Bank.CurrencyType.Gems:
                currencyText = Localization.Instance.Get("STRING_GLOBAL_GEMS");
                break;
            case Bank.CurrencyType.Pearls:
                currencyText = Localization.Instance.Get("STRING_GLOBAL_PEARLS");
                break;
        }

        return currencyText;
    }

    public static string GetIconText(CurrencyType type)
    {
        string iconText = null;

        switch (type)
        {
            case Bank.CurrencyType.Coins:
                iconText = Localization.Instance.Get("STRING_GLOBAL_COINS_ICON");
                break;
            case Bank.CurrencyType.Gems:
                iconText = Localization.Instance.Get("STRING_GLOBAL_GEM_ICON");
                break;
            case Bank.CurrencyType.Pearls:
                iconText = Localization.Instance.Get("STRING_GLOBAL_PEARL_ICON");
                break;
        }

        return iconText;
    }

    public static string GetCurrencyTypeSpriteName(Bank.CurrencyType type)
    {
        string spriteName = "";
        switch (type)
        {
            case CurrencyType.Coins:
                spriteName = "FE_icon_currencyPrimary";
                break;
            case CurrencyType.Gems:
                spriteName = "FE_icon_currencySecondary";
                break;
            case CurrencyType.Pearls:
                spriteName = "FE_icon_currencyTertiary";
                break;

        }
        return spriteName;
    }

    private BankSaveSystem m_saveSystem = null;

    #region Public Method

    public Bank()
    {
        m_saveSystem = new BankSaveSystem();
        SaveFacade.Instance.RegisterSaveSystem(m_saveSystem);
    }

    public bool AddCurrency(CurrencyType type, int amount, bool premium = false)
    {
        switch (type)
        {
            case CurrencyType.Coins:
                return AddCoins(amount, premium);
            case CurrencyType.Gems:
                return AddGems(amount, premium);
            case CurrencyType.Pearls:
                return AddPearls(amount, premium);
        }
        return false;
    }

    //Coins
    public bool AddCoins(int amount, bool premium = false)
    {
        bool success = false;
        if (amount > 0)
        {
            if (premium)
            {
                m_saveSystem.purchasedCoins += amount;
            }
            else
            {
                m_saveSystem.earnedCoins += amount;
            }

            NotifyAndSave(premium, CurrencyType.Coins, true);
            success = true;
        }
        else
        {
            Debug.LogWarning("Trying to add a negative or zero amount of coins");
        }
        return success;
    }
    public bool RemoveCoins(int amount)
    {
        bool success = false;
        if (amount >= 0)
        {
            if ((m_saveSystem.earnedCoins + m_saveSystem.purchasedCoins) >= amount)
            {
                int purchasedCoinsRemoved = Math.Min(amount, m_saveSystem.purchasedCoins);

                m_saveSystem.purchasedCoins -= purchasedCoinsRemoved;
                m_saveSystem.earnedCoins -= Math.Max(0, amount - purchasedCoinsRemoved);

                NotifyAndSave(false, CurrencyType.Coins, false);
                success = true;
            }
            else
            {
                Debug.Log("Bank (RemoveCoins) :: Trying to remove an amount more than the number of coins we have");
            }
        }
        else
        {
            Debug.LogWarning("Bank (RemoveCoins) :: Trying to remove a negative amount of coins");
        }
        return success;
    }
    public int GetCoins()
    {
        return m_saveSystem.earnedCoins + m_saveSystem.purchasedCoins;
    }

    //Gems
    public bool AddGems(int amount, bool premium = false)
    {
        bool success = false;
        if (amount > 0)
        {
            if (premium)
            {
                m_saveSystem.purchasedGems += amount;
            }
            else
            {
                m_saveSystem.earnedGems += amount;
            }
            NotifyAndSave(premium, CurrencyType.Gems, true);
            success = true;
        }
        else
        {
            Debug.LogWarning("Bank (AddGems) :: Trying to add a negative or zero amount of gems");
        }
        return success;
    }
    public bool RemoveGems(int amount)
    {
        bool success = false;
        if (amount >= 0)
        {
            if ((m_saveSystem.earnedGems + m_saveSystem.purchasedGems) >= amount)
            {
                int purchasedGemsRemoved = Math.Min(amount, m_saveSystem.purchasedGems);
                m_saveSystem.purchasedGems -= purchasedGemsRemoved;
                m_saveSystem.earnedGems -= Math.Max(0, amount - purchasedGemsRemoved);
                NotifyAndSave(false, CurrencyType.Gems, false);
                success = true;
            }
            else
            {
                Debug.Log("Bank (RemoveGems) :: Trying to remove an amount more than the number of gems we have");
            }
        }
        else
        {
            Debug.LogWarning("Bank (RemoveGems) :: Trying to remove a negative amount of gems");
        }
        return success;
    }
    public int GetGems()
    {
        return m_saveSystem.earnedGems + m_saveSystem.purchasedGems;
    }

    //Pearls
    public bool AddPearls(int amount, bool premium = false)
    {
        bool success = false;
        if (amount > 0)
        {
            if (premium)
            {
                m_saveSystem.purchasedPearls += amount;
            }
            else
            {
                m_saveSystem.earnedPearls += amount;
            }
            NotifyAndSave(premium, CurrencyType.Pearls, true);
            success = true;
        }
        else
        {
            Debug.LogWarning("Bank (AddPearls) :: Trying to add a negative or zero amount of Pearls");
        }
        return success;
    }
    public bool RemovePearls(int amount)
    {
        bool success = false;
        if (amount >= 0)
        {
            if ((m_saveSystem.earnedPearls + m_saveSystem.purchasedPearls) >= amount)
            {
                int purchasedPearlsRemoved = Math.Min(amount, m_saveSystem.purchasedPearls);
                m_saveSystem.purchasedPearls -= purchasedPearlsRemoved;
                m_saveSystem.earnedPearls -= Math.Max(0, amount - purchasedPearlsRemoved);
                NotifyAndSave(false, CurrencyType.Pearls, false);
                success = true;
            }
            else
            {
                Debug.Log("Bank (RemovePearls) :: Trying to remove an amount more than the number of Pearls we have");
            }
        }
        else
        {
            Debug.LogWarning("Bank (RemovePearls) :: Trying to remove a negative amount of Pearls");
        }
        return success;
    }
    public int GetPearls()
    {
        return m_saveSystem.earnedPearls + m_saveSystem.purchasedPearls;
    }

    public int GetBalance(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Coins:
                return GetCoins();
            case CurrencyType.Gems:
                return GetGems();
            case CurrencyType.Pearls:
                return GetPearls();
        }
        return 0;
    }
    #endregion

    private void NotifyAndSave(bool purchased, CurrencyType type, bool add)
    {
        EventManager<Events>.Instance.TriggerEvent(Events.BankUpdated, purchased, type, add);

        SaveFacade.Instance.Save(m_saveSystem.name, true);
    }
}

