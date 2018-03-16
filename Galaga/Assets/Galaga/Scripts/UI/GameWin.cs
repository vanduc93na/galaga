using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWin : MonoBehaviour
{
    void OnEnable()
    {
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
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowAdsVideo()
    {
        
    }
}
