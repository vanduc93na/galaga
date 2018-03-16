using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLose : MonoBehaviour
{

    [SerializeField] private GameObject _shopPanel;

    void OnEnable()
    {
        Time.timeScale = 0;
        _shopPanel.SetActive(false);
    }

    public void Replay()
    {
        GameController.Instance.Restart();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Shopping()
    {
        _shopPanel.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ShowAdsVideo()
    {

    }
}
