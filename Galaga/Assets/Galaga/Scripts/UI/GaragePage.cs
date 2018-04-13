using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaragePage : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Text _buttonText;
    [SerializeField] private Text _shipNameText;
    [SerializeField] private Text _coinText;
    [SerializeField] private Text _shipDes;
    [SerializeField] private Text _shipFunc;
    [SerializeField] private Sprite _select;
    [SerializeField] private Sprite _selected;
    [SerializeField] private Sprite _buy;
    [SerializeField] private string[] _shipName;
    [SerializeField] private int[] _shipPrime;
    [SerializeField] private string[] _shipSizeDes;
    [SerializeField] private string[] _shipFunction;
    private const string SELECT = "SELECT";
    private const string SELECTED = "SELECTED";
    private const string PRIME = " $";

    private int _curShip;

    public int CurShip
    {
        get { return _curShip; }
        set
        {
            _curShip = value;
        }

    }

    public ScrollLoop ScrollLoop;

    void Start()
    {
        ScrollLoop.OnFocusComplete += ActionReceiveIdShipSelect;
        InventoryHelper.Instance.LoadInventory();
        CurShip = InventoryHelper.Instance.UserInventory.shipSelected;
        ScrollLoop.CurIndex = CurShip;
        print(CurShip);
        ScrollLoop.FocusWithID();
        _coinText.text = InventoryHelper.Instance.UserInventory.coin.ToString();
    }

    void ActionReceiveIdShipSelect()
    {
        _button.interactable = true;
        CurShip = ScrollLoop.CurIndex;
        InventoryHelper.Instance.LoadInventory();
        int idShip = CurShip % 3;
        _shipNameText.text = _shipName[idShip];
        _shipDes.text = _shipSizeDes[idShip];
        _shipFunc.text = _shipFunction[idShip].Replace("\\n", "\n");
        if (idShip == InventoryHelper.Instance.UserInventory.shipSelected)
        {
            _buttonText.text = SELECTED;
            _button.GetComponent<Image>().sprite = _selected;
        }
        else if (isShipPaid(idShip))
        {
            _buttonText.text = SELECT;
            _button.GetComponent<Image>().sprite = _select;
        }
        else
        {
            if (InventoryHelper.Instance.UserInventory.coin < _shipPrime[idShip])
            {
                _button.interactable = false;
            }
            _buttonText.text = _shipPrime[idShip].ToString();
            _button.GetComponent<Image>().sprite = _buy;
        }
        InventoryHelper.Instance.SaveInventory();
    }

    public void ClickButton()
    {
        int idShip = CurShip % 3;
        if (_buttonText.text == SELECT)
        {
            _buttonText.text = SELECTED;
            InventoryHelper.Instance.SetIDShipSelected(idShip);
        }
        else if (_buttonText.text == SELECTED)
        {
            return;
        }
        else
        {
            // buy item
            if (InventoryHelper.Instance.RemoveCoin(_shipPrime[idShip]))
            {
                InventoryHelper.Instance.AddIdShipPaid(idShip);
            }
            else
            {

            }
            _coinText.text = InventoryHelper.Instance.UserInventory.coin.ToString();
        }
        ActionReceiveIdShipSelect();
    }

    public void BackAction()
    {
        gameObject.SetActive(false);
    }

    bool isShipPaid(int idShip)
    {
        string s = idShip.ToString();
        return InventoryHelper.Instance.UserInventory.openShip.Contains(s);
    }
}
