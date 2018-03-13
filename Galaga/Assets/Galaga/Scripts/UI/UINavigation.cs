using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _garage;
    [SerializeField] private GameObject _selectLevel;
    void Start()
    {
        _settings.SetActive(false);
        _shop.SetActive(false);
        _garage.SetActive(false);
        _selectLevel.SetActive(false);
    }

    public void GoToSetting()
    {
        _settings.SetActive(true);
    }

    public void GoToShop()
    {
        _shop.SetActive(true);
    }

    public void GoToGarage()
    {
        _garage.SetActive(true);
    }

    public void GoToSelectLevel()
    {
        _selectLevel.SetActive(true);
    }
}
