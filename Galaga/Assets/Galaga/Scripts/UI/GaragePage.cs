using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaragePage : MonoBehaviour
{
    [SerializeField] private Text _buttonText;
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
            InventoryHelper.Instance.SetIDShipSelected(value);
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
    }

    void ActionReceiveIdShipSelect()
    {
        CurShip = ScrollLoop.CurIndex;
        InventoryHelper.Instance.LoadInventory();
        int idShip = CurShip % 3;
        if (idShip == InventoryHelper.Instance.UserInventory.shipSelected)
        {
            _buttonText.text = SELECTED;
        }
        else if (isShipPaid(idShip))
        {
            _buttonText.text = SELECT;
        }
        else
        {
            _buttonText.text = "2000$";
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
            if (InventoryHelper.Instance.RemoveCoin(2000))
            {
                InventoryHelper.Instance.AddIdShipPaid(idShip);
            }
            else
            {
                print("not enough coin");
            }
        }
        ActionReceiveIdShipSelect();
    }

    bool isShipPaid(int idShip)
    {
        string s = idShip.ToString();
        return InventoryHelper.Instance.UserInventory.openShip.Contains(s);
    }
}
