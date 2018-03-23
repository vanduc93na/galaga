using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPage : MonoBehaviour
{
    [SerializeField] private GameObject _messageGameObject;
    [SerializeField] private GameObject _confirmGameObject;
    [SerializeField] private Text _confirmMessage;
    [SerializeField] private Text _messageText;

    public Action ActionClickConfirm;

    void OnEnable()
    {
        
    }

    public void ShowMessage(string message)
    {
        _confirmGameObject.SetActive(false);
        _messageText.text = message;
        _messageGameObject.SetActive(true);
    }

    public void ShowConfirm(string message)
    {
        _confirmGameObject.SetActive(true);
        _confirmMessage.text = message;
        _messageGameObject.SetActive(false);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void ClickOk()
    {
        if (ActionClickConfirm != null)
        {
            ActionClickConfirm();
        }
        gameObject.SetActive(false);
    }
}
