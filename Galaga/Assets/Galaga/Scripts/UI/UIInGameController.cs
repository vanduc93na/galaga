using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInGameController : Singleton<UIInGameController>
{

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private Text _coinText;
	// Use this for initialization

    void Awake()
    {
        this.RegisterListener(EventID.EatItem, (param) => EatCoin((GameObject) param));
    }
    
	void Start ()
	{
	    _coinText.text = "0";
        RegisterEvent();
	}

    void OnEnable()
    {
        winPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update ()
    {
		
	}
    /// <summary>
    /// gọi level đã chọn ở select level
    /// </summary>
    public void StartGame()
    {
//        this.PostEvent(EventID.PlayGame, 1);
        loadingPanel.gameObject.SetActive(false);
    }

    void EatCoin(GameObject obj)
    {
        if (obj.tag == GameTag.ITEM_COIN)
        {
            _coinText.text = (int.Parse(_coinText.text) + 1).ToString();
        }
    }

    public void GoMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RePlay()
    {
        StartCoroutine(Delay(1f, () =>
        {
            GameController.Instance.Play();
        }));
    }

    #region WinPanel

    public void NextLevel()
    {
        StartCoroutine(Delay(1f, () =>
        {
            GameController.Instance.NextLevel();
        }));
    }

    #endregion

    #region GameOverPanel

    public void ShowVideoAds()
    {
        
    }

    #endregion

    #region

    void RegisterEvent()
    {
        this.RegisterListener(EventID.GameOver, param => ShowGameOver());
    }

    #endregion

    IEnumerator Delay(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }

    void ShowGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }

    void ShowGameWin()
    {
        winPanel.gameObject.SetActive(true);
    }
}
 