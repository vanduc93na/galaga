using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField] private GameObject panel;

    [SerializeField] private Text _coinText;
	// Use this for initialization

    void Awake()
    {
        this.RegisterListener(EventID.EatCoin, (param) => EatCoin());
    }
    
	void Start ()
	{
	    _coinText.text = "0";
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void StartGame()
    {
        this.PostEvent(EventID.PlayGame, 1);
        panel.gameObject.SetActive(false);
    }

    void EatCoin()
    {
        _coinText.text = (int.Parse(_coinText.text) + 1).ToString();
    }
}
