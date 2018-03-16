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
        SceneManager.LoadScene(0);
    }

    public void ReStart()
    {
        GameController.Instance.Restart();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Setting()
    {
        _shopPanel.SetActive(true);
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
