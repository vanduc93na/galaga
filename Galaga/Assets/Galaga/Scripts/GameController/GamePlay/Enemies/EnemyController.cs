using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// thực hiện việc sinh và quản lý enemy
/// </summary>
public class EnemyController : Singleton<EnemyController>
{
    [Tooltip("Enemies Manager")] [SerializeField] private GameObject _enemiesMgr;
    [Tooltip("Path Manager")] [SerializeField] private GameObject _pathsManager;
    [Tooltip("Spawn Position")] [SerializeField] private GameObject _startPositionSpawnGameObject;

    /// <summary>
    /// danh sách chứa thông tin kiểu move theo path
    /// </summary>
    private List<MoveInformation> _listMoveConfig;
    /// <summary>
    /// danh sách tất cả enemy được lấy từ _enemiesMgr cache lại
    /// key là id của enemy
    /// </summary>
    private Dictionary<int, GameObject> _enemies;

    /// <summary>
    /// danh sách vị trí bắt đầu tạo enemy được lấy từ _startPositionSpawnGameObject
    /// </summary>
    private Dictionary<string, Vector3> _startPositionSpawnVector3;

    void Awake()
    {
        RegisterEvent();
        Init();
    }

    #region Private Method

    void RegisterEvent()
    {
        
    }

    void Init()
    {
        _enemies = new Dictionary<int, GameObject>();

        _listMoveConfig = new List<MoveInformation>();
        for (int i = 0; i < _pathsManager.transform.childCount; i++)
        {
            MoveInformation moveInfor = new MoveInformation();
            moveInfor.Waypoint = _pathsManager.transform.GetChild(i).GetComponent<DOTweenPath>().wps;
            moveInfor.Duration = _pathsManager.transform.GetChild(i).GetComponent<DOTweenPath>().duration;
            moveInfor.Type = _pathsManager.transform.GetChild(i).GetComponent<DOTweenPath>().pathType;
            moveInfor.Mode = _pathsManager.transform.GetChild(i).GetComponent<DOTweenPath>().pathMode;
            _listMoveConfig.Add(moveInfor);
        }

        // add list Enemies
        for (int i = 0; i < _enemiesMgr.transform.childCount; i++)
        {
            _enemies.Add(i, _enemiesMgr.transform.GetChild(i).gameObject);
        }

        // add list _startPositionSpawnVector3
        _startPositionSpawnVector3 = new Dictionary<string, Vector3>();
        _startPositionSpawnVector3.Add(GameTag.LEFT, _startPositionSpawnGameObject.transform.GetChild(0).gameObject.transform.position);
        _startPositionSpawnVector3.Add(GameTag.RIGHT, _startPositionSpawnGameObject.transform.GetChild(1).gameObject.transform.position);
        _startPositionSpawnVector3.Add(GameTag.TOP, _startPositionSpawnGameObject.transform.GetChild(2).gameObject.transform.position);
        _startPositionSpawnVector3.Add(GameTag.BOTTOM, _startPositionSpawnGameObject.transform.GetChild(3).gameObject.transform.position);
    }

    #region Methods For TypeMove

    IEnumerator SpawnEnemyTypeMoveOneByOne(WaveInformation wave)
    {
        for (int i = 0; i < wave.Row; i++)
        {
            for (int j = 0; j < wave.Col; j++)
            {
                yield return new WaitForSeconds(wave.DelaySpawn);
                GameObject enemySpawn = Lean.LeanPool.Spawn(_enemies[wave.Enemies[i * wave.Row + j].IdEnemy],
                    _startPositionSpawnVector3[GameTag.LEFT],
                    Quaternion.identity);
                enemySpawn.transform.SetParent(transform);
                // add enemy to controller
                HandleEvent.Instance.AddEnemy(enemySpawn);
                enemySpawn.GetComponent<BaseEnemy>().SetTargetPosition(GetTargetPosition(i, j, wave));
                if (i * wave.Row + j + 1 == wave.Enemies.Count)
                {
                    enemySpawn.GetComponent<BaseEnemy>().Init(wave.Enemies[i * wave.Row + j], true);
                }
                else
                {
                    enemySpawn.GetComponent<BaseEnemy>().Init(wave.Enemies[i * wave.Row + j]);
                }
                StartCoroutine(DelayMove(0.1f,
                    _listMoveConfig[wave.Enemies[wave.Row * i + j].IdPath] ,
                    enemySpawn.GetComponent<BaseEnemy>()));
            }
        }

    }

    IEnumerator SpawnEnemyTypeMoveInRow(WaveInformation wave)
    {
        yield return new WaitForSeconds(1f);
    }

    IEnumerator SpawnEnemyTypeMoveInLine(WaveInformation wave)
    {
        yield return new WaitForSeconds(1f);
    }

    IEnumerator SpawnEnemyTypeMoveRandom(WaveInformation wave)
    {
        yield return new WaitForSeconds(1f);
    }

    #endregion

    IEnumerator DelayMove(float seconds, MoveInformation moveInfor, BaseEnemy baseEnemy)
    {
        yield return new WaitForSeconds(seconds);
        baseEnemy.MovePathToSortMatrix(moveInfor);
    }

    IEnumerator DelaySpawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    #region Utilities Method

    /// <summary>
    /// tính vị trí cuối cùng của enemy
    /// vị trí của enemy sẽ tương đối so vơi tọa độ (0, 0)
    /// </summary>
    /// <param name="row">hàng</param>
    /// <param name="col">cột</param>
    /// <returns>Vector 3 là vị trí cuối cùng mà enemy phải xếp vào</returns>
    private Vector3 GetTargetPosition(int row, int col, WaveInformation wave)
    {
        float dx = (float) (wave.SizeDx / wave.Col);
        float dy = (float) (wave.SizeDy / wave.Row);
        float valueX = wave.Dx + col * dx;
        float valueY = wave.Dy - row * dy;
        return new Vector3(valueX, valueY, transform.position.z);
    }


    #endregion

    #endregion

    #region Public Method

    public void SpawnEnemy(WaveInformation wave)
    {
        switch (wave.TypeMove)
        {
            case TypeMove.OneByOne:
                StartCoroutine(SpawnEnemyTypeMoveOneByOne(wave));
                break;
            case TypeMove.MoveInLine:
                StartCoroutine(SpawnEnemyTypeMoveInLine(wave));
                break;
            case TypeMove.MoveInRows:
                StartCoroutine(SpawnEnemyTypeMoveInRow(wave));
                break;
            case TypeMove.Random:
                StartCoroutine(SpawnEnemyTypeMoveRandom(wave));
                break;
            default:
                break;

        }
    }

    #endregion
}
