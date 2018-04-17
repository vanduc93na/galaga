using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class InventoryHelper
{
    public UserInventory UserInventory;
    public Action OnCoinChange;
    private static InventoryHelper instance;
    public static InventoryHelper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InventoryHelper();
            }
            return instance;
        }
    }

    public bool IsShowAds()
    {
        LoadInventory();
        return UserInventory.adsOn;
    }

    public void SetShowAds(bool isShow)
    {
        LoadInventory();
        UserInventory.adsOn = isShow;
        SaveInventory();
    }

    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(StringKeys.FILE_NAME))
        {
            UserInventory = new UserInventory();
            string data = JsonUtility.ToJson(UserInventory);
            PlayerPrefs.SetString(StringKeys.FILE_NAME, data);
        }
        else
        {
            string data = PlayerPrefs.GetString(StringKeys.FILE_NAME);
            UserInventory = JsonUtility.FromJson<UserInventory>(data);
        }

    }

    public void SaveInventory()
    {
        string data = JsonUtility.ToJson(UserInventory);
        PlayerPrefs.SetString(StringKeys.FILE_NAME, data);
    }

    public void AddCoin(int coin)
    {
        LoadInventory();
        UserInventory.coin += coin;
        SaveInventory();
        if (OnCoinChange != null)
        {
            OnCoinChange();
        }
    }

    public void SetPassLevel(int level)
    {
        LoadInventory();
        UserInventory.passLevel = level;
        SaveInventory();
    }

    public bool RemoveCoin(int coin)
    {
        LoadInventory();
        if (UserInventory.coin < coin)
        {
            return false;
        }
        UserInventory.coin -= coin;
        SaveInventory();
        if (OnCoinChange != null)
        {
            OnCoinChange();
        }
        return true;   
    }

    public void SetIDShipSelected(int id)
    {
        LoadInventory();
        UserInventory.shipSelected = id;
        SaveInventory();
    }

    public void AddIdShipPaid(int id)
    {
        LoadInventory();
        UserInventory.openShip += "," + id.ToString();
        SaveInventory();
    }

    public void AddDamageRate(float rate)
    {
        LoadInventory();
        if (UserInventory.damageRate + rate <= 2f)
        {
            UserInventory.damageRate += rate;
        }
        SaveInventory();
    }

    public void AddSelectedLevel(int level)
    {
        LoadInventory();
        UserInventory.selectedLevel = level;
        SaveInventory();
    }

    public void AddLife(int life)
    {
        LoadInventory();
        UserInventory.life += life;
        SaveInventory();
    }

    public int GetLife()
    {
        LoadInventory();
        return UserInventory.life;
    }
}
