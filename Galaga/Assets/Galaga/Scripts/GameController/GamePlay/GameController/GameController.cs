using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// class quản lý toàn bộ game bao gồm:
/// - thực hiện điểu khiển map
/// - điểu khiển các trạng thái game
/// - điều khiển UI
/// </summary>
public class GameController : Singleton<GameController>
{
    public GameStage gameStage;

    private LevelInformation _currentLevel;
    private WaveInformation _currentWave;
    private LevelInformation[] _levelsInfor;
    private int _indexWave;
    void Awake()
    {
        // đăng ký sự kiện bắt đầu game
        this.RegisterListener(EventID.PlayGame, (param) => StartGame((int) param));
        this.RegisterListener(EventID.NextWave, (param) => NextWave());
        Init();
    }

    #region Private Mathod

    void Init()
    {
        _levelsInfor = Resources.LoadAll<LevelInformation>("Maps");
        _indexWave = 0;
    }


    #endregion

    /// <summary>
    /// bắt đầu load level game
    /// </summary>
    void StartGame(int level)
    {
        _indexWave = 0;
        gameStage = GameStage.Play;
        _currentLevel = _levelsInfor[level - 1];
        _currentWave = _currentLevel.Waves[_indexWave];
        Play();
    }

    void Play()
    {
        switch (_currentWave.TypeWave)
        {
            case TypeOfWave.Enemies:
                EnemyController.Instance.SpawnEnemy(_currentWave);
                break;
            case TypeOfWave.Boss:
                break;
        }
        
    }

    void NextWave()
    {
        _indexWave++;
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