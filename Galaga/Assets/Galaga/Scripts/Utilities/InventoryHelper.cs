using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class InventoryHelper
{
    public UserInventory UserInventory;
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


    public void LoadInventory()
    {
        string data = PlayerPrefs.GetString(StringKeys.FILE_NAME);
        if (data == string.Empty)
            UserInventory = new UserInventory();
        else
            UserInventory = JsonUtility.FromJson<UserInventory>(data);
    }

    public void SaveInventory()
    {
        string data = JsonUtility.ToJson(UserInventory);
        PlayerPrefs.SetString(data, StringKeys.FILE_NAME);
    }

    public void AddCoin(int coin)
    {
        LoadInventory();
        UserInventory.coin += coin;
        SaveInventory();
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
        return true;
    }


}
