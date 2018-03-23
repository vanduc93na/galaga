using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWin : MonoBehaviour
{
    [Tooltip("text coin từ màn game play")]
    [SerializeField] private Text _coin;
    [SerializeField] private Text _levelTxt;
    [SerializeField] private Text _scoreTxt;
    [SerializeField] private Text _enemiesDestroyTxt;
    [SerializeField] private Text _rewardTxt;
    private int _level = 0;
    private int _score = 0;
    private int _enemiesDestroy = 0;
    private int _reward;

    void OnEnable()
    {
        InventoryHelper.Instance.LoadInventory();
        _level = InventoryHelper.Instance.UserInventory.selectedLevel;
        _score = int.Parse(_coin.text);
        _enemiesDestroy = HandleEvent.Instance.EnemiesDestroy;
        _reward = _level + _score + _enemiesDestroy;
        InventoryHelper.Instance.AddCoin(_reward);
        Time.timeScale = 0;
    }

    public void Replay()
    {
        GameController.Instance.Restart();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void NextLevel()
    {
        GameController.Instance.NextLevel();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ShowAdsVideo()
    {
        API.ShowVideo((() =>
        {
            InventoryHelper.Instance.AddCoin(_reward);
        }));
    }
}
