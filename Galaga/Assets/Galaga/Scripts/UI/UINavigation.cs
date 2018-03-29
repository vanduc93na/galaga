using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    [SerializeField] private GameObject _home;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _garage;
    [SerializeField] private GameObject _selectLevel;
    [SerializeField] private GameObject _popup;
    [SerializeField] private Text _coinAtHomeNumber;
    [SerializeField] private Text _coinAtSelectNumber;

    void Awake()
    {
        InventoryHelper.Instance.AddCoin(10000);
        InventoryHelper.Instance.OnCoinChange += () =>
        {
            InventoryHelper.Instance.LoadInventory();
            if (_coinAtHomeNumber != null)
            {
                _coinAtHomeNumber.text = InventoryHelper.Instance.UserInventory.coin.ToString();
//                _coinAtSelectNumber.text = InventoryHelper.Instance.UserInventory.coin.ToString();
            }
            
        };
        _shop.GetComponent<Shop>().OnCoinChange += UpdateCoin;
    }

    void Start()
    {
        _settings.SetActive(false);
        _shop.SetActive(false);
        _garage.SetActive(false);
        _selectLevel.SetActive(false);
        _popup.SetActive(false);
        SoundController.PlayBackgroundSound(SoundController.Instance.MenuBackgroundSound);
    }

    void OnEnable()
    {
        InventoryHelper.Instance.LoadInventory();
        _coinAtHomeNumber.text = InventoryHelper.Instance.UserInventory.coin.ToString();
//        _coinAtSelectNumber.text = InventoryHelper.Instance.UserInventory.coin.ToString();
    }

    public void GoToSetting()
    {
        _settings.SetActive(true);
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        //        _home.SetActive(false);
    }

    public void GoToShop()
    {
        _shop.SetActive(true);
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        //        _home.SetActive(false);
    }

    public void GoToGarage()
    {
        _garage.SetActive(true);
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        //        _home.SetActive(false);
    }

    public void GoToSelectLevel()
    {
        _selectLevel.SetActive(true);
        _home.SetActive(false);
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
    }

    void UpdateCoin()
    {
        InventoryHelper.Instance.LoadInventory();
        _coinAtHomeNumber.text = InventoryHelper.Instance.UserInventory.coin.ToString();
    }
}
