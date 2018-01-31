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

    [SerializeField] private GamePlayConfig _gamePlayConfig;
    private LevelInformation _currentLevel;
    private WaveInformation _currentWave;
    private LevelInformation[] _levelsInfor;
    private int _indexWave;
    private int _currentLevelIndex;
    void Awake()
    {
        // đăng ký sự kiện bắt đầu game
        this.RegisterListener(EventID.PlayGame, (param) => StartGame((int)param));
        this.RegisterListener(EventID.NextWave, (param) => NextWave());
        Init();
    }

    #region Private Method

    void Init()
    {
        _levelsInfor = Resources.LoadAll<LevelInformation>("Maps");
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
        Play();
    }

    public void Play()
    {
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
        _currentLevelIndex++;
        Play();
    }

    void NextWave()
    {
        if (_indexWave >= _currentLevel.Waves.Count - 1)
        {
            print("complete level");
            this.PostEvent(EventID.NextLevel);
            return;
        }
        _indexWave++;
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