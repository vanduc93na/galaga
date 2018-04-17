using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWin : MonoBehaviour
{
    public Action OnReplay;

    [Tooltip("text coin từ màn game play")]
    [SerializeField] private Text _coin;
    [SerializeField] private Text _levelTxt;
    [SerializeField] private Text _scoreTxt;
    [SerializeField] private Text _enemiesDestroyTxt;
    [SerializeField] private Text _rewardTxt;
    [SerializeField] private Button _rewardButton;
    private int _level = 0;
    private int _score = 0;
    private int _enemiesDestroy = 0;
    private int _reward;
    private int _countTapRewardButton;

    void OnEnable()
    {
        _rewardButton.interactable = true;
        _countTapRewardButton = 0;
        InventoryHelper.Instance.LoadInventory();
        _level = InventoryHelper.Instance.UserInventory.selectedLevel;
        _score = int.Parse(_coin.text);
        _enemiesDestroy = HandleEvent.Instance.EnemiesDestroy;
        _reward = _level + _score + _enemiesDestroy;
        InventoryHelper.Instance.AddCoin(_reward);
        Time.timeScale = 0;
        ShowResult();
    }

    void ShowResult()
    {
        _levelTxt.text = _level.ToString();
        _scoreTxt.text = _score.ToString();
        _enemiesDestroyTxt.text = _enemiesDestroy.ToString();
        _rewardTxt.text = _reward.ToString();
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

    public void NextLevel()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        GameController.Instance.NextLevel();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Exit()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ShowAdsVideo()
    {
        if (_countTapRewardButton >= 1)
        {
            _rewardButton.interactable = false;
            return;
        }
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        API.ShowVideo((() =>
        {
            InventoryHelper.Instance.AddCoin(_reward);
            ShowResult();
            _countTapRewardButton += 1;
        }));
    }
}
