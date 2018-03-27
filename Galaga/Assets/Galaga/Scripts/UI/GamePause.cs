using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    
    [SerializeField] private GameObject _shopPanel;

    void OnEnable()
    {
        _shopPanel.SetActive(false);
    }

    public void GoHome()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        SceneManager.LoadScene(0);
    }

    public void ReStart()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        GameController.Instance.Restart();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Setting()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        _shopPanel.SetActive(true);
    }

    public void Resume()
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        GameController.Instance.gameStage = GameStage.Play;
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }
}
