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
    
    void OnEnable()
    {
        _groupsItem.anchoredPosition = new Vector2(0, -_groupsItem.rect.height - 70);
        _groupsItem.DOAnchorPosY(0, .7f);
    }

    public void Close()
    {
        _groupsItem.DOAnchorPosY(-_groupsItem.rect.height - 70, .7f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
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
        Purchaser.Instance.BuyNoAds();
    }

    public void ClickBuy20kGold()
    {
        Purchaser.Instance.Buy20K();
    }

    public void ClickBuy50kGold()
    {
        Purchaser.Instance.Buy50K();
    }
}
