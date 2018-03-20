﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI: Singleton<GamePlayUI>
{

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _coinTxt;
    [SerializeField] private Text _lifeTxt;

    void OnEnable()
    {
        InventoryHelper.Instance.LoadInventory();
        _lifeTxt.text = InventoryHelper.Instance.UserInventory.life.ToString();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        _pausePanel.SetActive(true);
    }

    public void SetCoin(int coin)
    {
        _coinTxt.text = coin.ToString();
    }

    public void SetLife(int life)
    {
        _lifeTxt.text = life.ToString();
    }
}