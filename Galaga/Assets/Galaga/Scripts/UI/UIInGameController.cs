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
    [SerializeField] private GameObject _gamePausePanel;
    [SerializeField] private GameObject _gamePlayPanel;
    [SerializeField] private GameObject _countDownPanel;

    private int _coinInLevel;
	// Use this for initialization

    void Awake()
    {
        RegisterEvent();
    }
    
	void Start ()
	{
        Reset();
	}

    void Reset()
    {
        _coinInLevel = 0;
        _gamePlayPanel.GetComponent<GamePlayUI>().SetCoin(_coinInLevel);
    }

    void OnEnable()
    {
        winPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        _countDownPanel.SetActive(false);
        loadingPanel.SetActive(true);
    }

    void EatCoin(GameObject obj)
    {
        if (obj.tag == GameTag.ITEM_COIN)
        {
            _coinInLevel += 1;
            GamePlayUI.Instance.SetCoin(_coinInLevel);
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

    #endregion

    #region

    void RegisterEvent()
    {
        this.RegisterListener(EventID.GameOver, param => ShowGameOver());
        this.RegisterListener(EventID.GameWin, (param) => ShowGameWin());
        this.RegisterListener(EventID.EatItem, (param) => EatCoin((GameObject)param));
        this.RegisterListener(EventID.Restart, (param) => Reset());
        this.RegisterListener(EventID.NextLevel, (param) => Reset());
    }

    #endregion

    IEnumerator Delay(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }

    void ShowGameOver()
    {
        GameController.Instance.gameStage = GameStage.GameOver;
        _countDownPanel.gameObject.SetActive(true);
    }

    void ShowGameWin()
    {
        GameController.Instance.gameStage = GameStage.Win;
        StartCoroutine(Show(2f, () => winPanel.gameObject.SetActive(true)));
    }

    public int GetCoinInLevel()
    {
        return _coinInLevel;
    }

    IEnumerator Show(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }
}
 