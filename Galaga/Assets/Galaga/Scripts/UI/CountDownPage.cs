using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountDownPage : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Text _timerText;
    [SerializeField] private int _timerCountDown;
    public Action OnReturnPlay;

    void OnEnable()
    {
//        Time.timeScale = 0;
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        _timerText.text = _timerCountDown.ToString();
        for (int i = 1; i <= _timerCountDown; i++)
        {
            yield return new WaitForSeconds(1f);
            _timerText.text = (_timerCountDown - i).ToString();
            if (i == _timerCountDown)
            {
                GoToGameOver();
            }
        }
    }

    public void ShowAds()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        API.ShowVideo(() =>
        {
            StopCoroutine(CountDown());

            // sau khi xem video xong
            GameController.Instance.gameStage = GameStage.Play;
            if (OnReturnPlay != null)
            {
                OnReturnPlay();
            }
            gameObject.SetActive(false);
            Time.timeScale = 1;
        });
    }

    public void GoToGameOver()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _gameOverPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ResumeWithCoin()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        StopCoroutine(CountDown());
        InventoryHelper.Instance.LoadInventory();
        if (InventoryHelper.Instance.UserInventory.coin > 1000)
        {
            InventoryHelper.Instance.RemoveCoin(1000);
            GameController.Instance.gameStage = GameStage.Play;
            _gameOverPanel.SetActive(true);
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            Debug.Log("not enough coin");
        }
        
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
