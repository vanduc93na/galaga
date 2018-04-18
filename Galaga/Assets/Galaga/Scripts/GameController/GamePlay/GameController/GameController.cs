﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

/// <summary>
/// class quản lý toàn bộ game bao gồm:
/// - thực hiện điểu khiển map
/// - điểu khiển các trạng thái game
/// - điều khiển UI
/// </summary>
public class GameController : MonoBehaviour
{
    public GameStage gameStage;

    [SerializeField] private GamePlayConfig _gamePlayConfig;
    private LevelInformation _currentLevel;
    private WaveInformation _currentWave;
    private LevelInformation[] _levelsInfor;
    private int _indexWave;
    private int _currentLevelIndex;

    public static GameController Instance;

    void Awake()
    {
        
        // đăng ký sự kiện bắt đầu game
        this.RegisterListener(EventID.NextWave, (param) => NextWave());
        this.RegisterListener(EventID.GameOver, (param) => StopAllCoroutines());
        this.RegisterListener(EventID.PlayerDead, (param) => StopAllCoroutines());
        this.RegisterListener(EventID.GameWin, (param) => StopAllCoroutines());
        Init();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    #region Private Method

    void Init()
    {
        _levelsInfor = Resources.LoadAll<LevelInformation>("Maps");
        _levelsInfor = _levelsInfor.OrderBy(s => int.Parse(s.name.Replace("level ", ""))).ToArray();
        _indexWave = 0;
        _currentLevelIndex = 0;
    }


    #endregion

    /// <summary>
    /// bắt đầu load level game
    /// </summary>
    public void StartGame(int level)
    {
        gameStage = GameStage.Play;
        _currentLevel = _levelsInfor[level - 1];
        _currentWave = _currentLevel.Waves[_indexWave];
        _currentLevelIndex = level;
        HandleEvent.Instance.EnemiesDestroy = 0;
        Play();
    }

    public void Restart()
    {
        this.PostEvent(EventID.Restart);
        gameStage = GameStage.Play;
        _indexWave = 0;
        StartCoroutine(WaitForSecondsNextWave(1f));
    }

    public void Play()
    {
        print("lv" + _currentLevelIndex + " +  wave:" + _currentWave.Enemies.Count);
        switch (_currentWave.TypeWave)
        {
            case TypeOfWave.Enemies:
                if (!_currentWave.ClearEnemiesToCompleteWave)
                {
                    StartCoroutine(TimeToCompleteWave(_currentWave.TimeCompleteWave));
                }
                EnemyController.Instance.SpawnEnemy(_currentWave);
                break;
            case TypeOfWave.Boss:
                BossController.Instance.SpawnBoss(_currentWave.WaveBossInformation);
                break;
        }

    }

    public void NextLevel()
    {
        if (_currentLevelIndex < 0 || _currentLevelIndex >= _levelsInfor.Length - 1)
        {
            print("last level");
            return;
        }
        HandleEvent.Instance.ResetLevel();
        this.PostEvent(EventID.NextLevel);
        _currentLevelIndex++;
        StartCoroutine(WaitForSecondsNextWave(_gamePlayConfig.GetTimeDelayBetweenWaves()));
    }

    void NextWave()
    {
        _indexWave++;
        if (_indexWave > _currentLevel.Waves.Count - 1)
        {
            _indexWave = 0;
            print("complete level");
            StartCoroutine(Delay(2f, () =>
            {
                this.PostEvent(EventID.GameWin);
                InventoryHelper.Instance.LoadInventory();
                if (InventoryHelper.Instance.UserInventory.passLevel < _currentLevelIndex)
                {
                    InventoryHelper.Instance.SetPassLevel(_currentLevelIndex);
                }
            }));
            return;
        }
        StartCoroutine(WaitForSecondsNextWave(_gamePlayConfig.GetTimeDelayBetweenWaves()));
    }

    IEnumerator TimeToCompleteWave(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.PostEvent(EventID.NextWave);
    }

    IEnumerator WaitForSecondsNextWave(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartGame(_currentLevelIndex);
    }

    IEnumerator Delay(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }
}

/// <summary>
/// trạng thái game
/// </summary>
public enum GameStage
{
    // trạng thái trước khi vào chơi, có thể ở màn hình home, shopping,...
    // optional - có thể là trạng thái giữa 2 wave
    Waitting,
    // trạng thái khi đang chơi
    Play,
    // trạng thái pause
    Pause,
    // trạng thái khi bị thua
    GameOver,
    // trạng thái thắng
    Win
}