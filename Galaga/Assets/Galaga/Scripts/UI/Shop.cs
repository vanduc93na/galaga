using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _groupsItem;
    [SerializeField] private int _mekitPrime;
    [SerializeField] private int _globalDMGPrime;
    [SerializeField] private Text _messageMekit;
    [SerializeField] private Text _messageDMG;
    [SerializeField] private float _damageRate;
    [SerializeField] private GameObject _popupPage;
    [SerializeField] private string _textMessagePopupConfirm;
    [SerializeField] private string _textMessagePopupWarning;
    private int _clickIndex = 0;

    void Start()
    {
        _popupPage.GetComponent<PopupPage>().ActionClickConfirm += ConfirmBuy;
        _clickIndex = 0;
    }

    void OnEnable()
    {
        _groupsItem.anchoredPosition = new Vector2(0, -_groupsItem.rect.height - 70);
        _groupsItem.DOAnchorPosY(0, .7f).SetUpdate(true);
    }

    public void Close()
    {
        _groupsItem.DOAnchorPosY(-_groupsItem.rect.height - 70, .7f).OnComplete(() =>
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
        InventoryHelper.Instance.LoadInventory();
    }

    public void ClickBuyDMG()
    {
        InventoryHelper.Instance.AddDamageRate(_damageRate);
    }

    public void ClickBuyRemoveAds()
    {
        _popupPage.SetActive(true);
        _popupPage.GetComponent<PopupPage>().ShowConfirm(_textMessagePopupConfirm);
        _clickIndex = 1;
    }

    public void ClickBuy20kGold()
    {
        _popupPage.SetActive(true);
        _popupPage.GetComponent<PopupPage>().ShowConfirm(_textMessagePopupConfirm);
        _clickIndex = 2;
    }

    public void ClickBuy50kGold()
    {
        _popupPage.SetActive(true);
        _popupPage.GetComponent<PopupPage>().ShowConfirm(_textMessagePopupConfirm);
        _clickIndex = 3;
    }

    private void ConfirmBuy()
    {
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
    }
}
