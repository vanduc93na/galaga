using System;
using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour, IPointerClickHandler
{
    public Action OnCoinChange;

    [SerializeField] private RectTransform _groupsItem;
    [SerializeField] private int _mekitPrime;
    [SerializeField] private int _globalDMGPrime;
    [SerializeField] private Text _statusMekitPrime;
    [SerializeField] private Text _statusDMGPrime;
    [SerializeField] private Text _overCoinMekit;
    [SerializeField] private Text _overCoinDMG;
    [SerializeField] private float _damageRate;
    [SerializeField] private GameObject _popupPage;
    [SerializeField] private string _textMessagePopupConfirm;
    [SerializeField] private string _textMessagePopupWarning;

    [SerializeField] private Image _imageBlackFilter;


    private int _clickIndex = 0;

    void Start()
    {
        _popupPage.GetComponent<PopupPage>().ActionClickConfirm += ConfirmBuy;
        _clickIndex = 0;
    }

    void OnEnable()
    {
        _groupsItem.anchoredPosition = new Vector2(0, -_groupsItem.rect.height - 70);

        _groupsItem.DOAnchorPosY(-30, .2f).SetUpdate(true);
        _imageBlackFilter.DOFade(0f, 0f).SetUpdate(true);
        _imageBlackFilter.DOFade(.5f, .2f).SetUpdate(true);
        InitUI();
    }

    public void Close()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _imageBlackFilter.DOFade(0f, .2f).SetUpdate(true);
        _groupsItem.DOAnchorPosY(-_groupsItem.rect.height - 70, .2f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }).SetUpdate(true);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Close();
        }
    }

    public void ClickBuyMekit()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        InventoryHelper.Instance.LoadInventory();
        if (InventoryHelper.Instance.UserInventory.coin > _mekitPrime)
        {
            InventoryHelper.Instance.RemoveCoin(_mekitPrime);
            InventoryHelper.Instance.AddLife(1);
            if (OnCoinChange != null)
            {
                OnCoinChange();
            }
            InitUI();
        }
        
    }

    public void ClickBuyDMG()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        if (InventoryHelper.Instance.UserInventory.coin > _globalDMGPrime)
        {
            InventoryHelper.Instance.AddDamageRate(_damageRate);
            InventoryHelper.Instance.RemoveCoin(_globalDMGPrime);
            if (OnCoinChange != null)
            {
                OnCoinChange();
            }
            InitUI();
        }
    }

    public void ClickBuyRemoveAds()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _popupPage.SetActive(true);
        _popupPage.GetComponent<PopupPage>().ShowConfirm(_textMessagePopupConfirm);
        _clickIndex = 1;
    }

    public void ClickBuy20kGold()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _popupPage.SetActive(true);
        _popupPage.GetComponent<PopupPage>().ShowConfirm(_textMessagePopupConfirm);
        _clickIndex = 2;
    }

    public void ClickBuy50kGold()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _popupPage.SetActive(true);
        _popupPage.GetComponent<PopupPage>().ShowConfirm(_textMessagePopupConfirm);
        _clickIndex = 3;
    }

    private void ConfirmBuy()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        switch (_clickIndex)
        {
            case 1:
                Purchaser.Instance.BuyNoAds();
                break;
            case 2:
                Purchaser.Instance.Buy20K();
                break;
            case 3:
                Purchaser.Instance.Buy50K();
                break;
        }
        if (OnCoinChange != null)
        {
            OnCoinChange();
        }
        InitUI();
    }

    void InitUI()
    {
        InventoryHelper.Instance.LoadInventory();
        _statusMekitPrime.text = "YOUR STATUS: " + InventoryHelper.Instance.UserInventory.life.ToString() + " LIFE";
        _statusDMGPrime.text = "GIVE ALL SHIP \n" +
                               ((InventoryHelper.Instance.UserInventory.damageRate - 1) * 100).ToString() +
                               "% DMG IN COMBAT";
        if (InventoryHelper.Instance.UserInventory.coin < _mekitPrime)
        {
            _overCoinMekit.text = "OUT OF STOCK";
        }
        else
        {
            _overCoinMekit.text = ""
                ;
        }
        if (InventoryHelper.Instance.UserInventory.coin < _globalDMGPrime)
        {
            _overCoinDMG.text = "OUT OF STOCK";
        }
        else
        {
            _overCoinDMG.text = "";
        }
    }
}
