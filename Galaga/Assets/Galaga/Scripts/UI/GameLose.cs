using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLose : MonoBehaviour
{
    public Action OnReplay;

    [SerializeField] private GameObject _shopPanel;

    void OnEnable()
    {
        Time.timeScale = 0;
        _shopPanel.SetActive(false);
    }

    public void Replay()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        if (OnReplay != null)
        {
            OnReplay();
        }
        GameController.Instance.Restart();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Shopping()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _shopPanel.SetActive(true);
    }

    public void Exit()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ShowAdsVideo()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
    }
}
