using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins;
using UnityEngine;

/// <summary>
/// thực hiện việc sinh và quản lý sinh
/// </summary>
public class EnemyController : Singleton<EnemyController>
{
    [Tooltip("Enemies Manager")] [SerializeField] private GameObject _enemiesMgr;
    [Tooltip("Path Manager")] [SerializeField] private GameObject _pathsManager;
    [Tooltip("Spawn Position")] [SerializeField] private GameObject _startPositionSpawnGameObject;
    [SerializeField] private Transform _parenTransform;

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

    private GameObject _emptyGO;

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
        _emptyGO = new GameObject("empty enemy");
    }

    #region Methods For TypeMove

    IEnumerator SpawnEnemyTypeMoveOneByOne(WaveInformation wave, List<GameObject> enemies)
    {
        Dictionary<int, int> enemyWithPath = new Dictionary<int, int>();
        for (int i = 0; i < wave.Enemies.Count; i++)
        {
            if (!enemyWithPath.ContainsKey(wave.Enemies[i].IdEnemy))
            {
                enemyWithPath.Add(wave.Enemies[i].IdEnemy,
                    wave.Enemies[i].IdPath);
            }
        }
        var listKey = enemyWithPath.Keys.ToList();
        for (int pathIndex = 0; pathIndex < listKey.Count; pathIndex++)
        {
            int index = 0;
            for (int i = 0; i < wave.Row; i++)
            {
                for (int j = 0; j < wave.Col; j++)
                {

                    if (wave.Enemies[index].IdEnemy == listKey[pathIndex])
                    {
                        yield return new WaitForSeconds(wave.DelaySpawn);
                        MoveInformation moveInfor = new MoveInformation();
                        moveInfor.Waypoint = _listMoveConfig[wave.Enemies[index].IdPath].Waypoint;
                        moveInfor.Mode = _listMoveConfig[wave.Enemies[index].IdPath].Mode;
                        moveInfor.Type = _listMoveConfig[wave.Enemies[index].IdPath].Type;
                        moveInfor.Duration = _listMoveConfig[wave.Enemies[index].IdPath].Duration + wave.CustomSpeed;
                        switch (wave.TypeSort)
                        {
                            case TypeSort.LeftToRightAndTopDown:
                                
                                StartCoroutine(DelayMove(0,
                                    moveInfor,
                                    HandleEvent.Instance.GetBaseEnemyScript(enemies[index])));
                                break;
                            case TypeSort.BottomUpAndRightToLeft:
                                StartCoroutine(DelayMove(0,
                                    moveInfor,
                                    HandleEvent.Instance.GetBaseEnemyScript(enemies[enemies.Count - (index) - 1])));
                                break;
                        }
                    }
                    index++;
                }
            }
        }
    }

    IEnumerator SpawnEnemyTypeMoveInRow(WaveInformation wave, List<GameObject> enemies)
    {
        for (int i = 0; i < wave.Row; i++)
        {
            for (int j = 0; j < wave.Col; j++)
            {
                yield return new WaitForSeconds(wave.DelaySpawn);
                int index = i * wave.Col + j;
                MoveInformation moveInfor = new MoveInformation();
                moveInfor.Waypoint = _listMoveConfig[wave.Enemies[index].IdPath].Waypoint;
                moveInfor.Mode = _listMoveConfig[wave.Enemies[index].IdPath].Mode;
                moveInfor.Type = _listMoveConfig[wave.Enemies[index].IdPath].Type;
                moveInfor.Duration = _listMoveConfig[wave.Enemies[index].IdPath].Duration + wave.CustomSpeed;
                StartCoroutine(DelayMove(0, moveInfor, HandleEvent.Instance.GetBaseEnemyScript(enemies[index])));
            }
            yield return new WaitForSeconds(wave.DelayMove);
        }
        yield return new WaitForSeconds(1f);
    }

    IEnumerator SpawnEnemyTypeMoveInCol(WaveInformation wave, List<GameObject> enemies)
    {
        for (int i = 0; i < wave.Col; i++)
        {
            for (int j = 0; j < wave.Row; j++)
            {
                yield return new WaitForSeconds(wave.DelaySpawn);
                int index = j * wave.Col + i;
                MoveInformation moveInfor = new MoveInformation();
                moveInfor.Waypoint = _listMoveConfig[wave.Enemies[index].IdPath].Waypoint;
                moveInfor.Mode = _listMoveConfig[wave.Enemies[index].IdPath].Mode;
                moveInfor.Type = _listMoveConfig[wave.Enemies[index].IdPath].Type;
                moveInfor.Duration = _listMoveConfig[wave.Enemies[index].IdPath].Duration + wave.CustomSpeed;
                StartCoroutine(DelayMove(0, moveInfor, HandleEvent.Instance.GetBaseEnemyScript(enemies[index])));
            }
            yield return new WaitForSeconds(wave.DelayMove);
        }
        
    }

    IEnumerator SpawnEnemyTypeMoveRandom(WaveInformation wave)
    {
        yield return new WaitForSeconds(1f);
    }

    #endregion

    IEnumerator DelayMove(float seconds, MoveInformation moveInfor, BaseEnemy baseEnemy)
    {
        yield return new WaitForSeconds(seconds);
        if (baseEnemy != null)
        {
            baseEnemy.MovePathToSortMatrix(moveInfor);
        }
    }

    IEnumerator DelaySpawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    #region UtilitiesGameTool Method

    /// <summary>
    /// tính vị trí cuối cùng của enemy
    /// vị trí của enemy sẽ tương đối so vơi tọa độ (0, 0)
    /// </summary>
    /// <param name="row">hàng</param>
    /// <param name="col">cột</param>
    /// <returns>Vector 3 là vị trí cuối cùng mà enemy phải xếp vào</returns>
    private Vector3 GetTargetPosition(int row, int col, WaveInformation wave)
    {
        float dx = (float)(wave.SizeDx / wave.Col);
        float dy = (float)(wave.SizeDy / wave.Row);
        float valueX = wave.Dx + col * dx;
        if (row % 2 == 1)
        {
            valueX = wave.OverLapping ? valueX - (float)dx / 2 : valueX;
        }
        float valueY = wave.Dy - row * dy;
        return new Vector3(valueX, valueY, transform.position.z);
    }

    /// <summary>
    /// hàm set drop item cho enemy
    /// </summary>
    /// <param name="listRandom">danh sách random enemy được khởi tạo</param>
    /// <param name="index">vị trí enemy thứ bao nhiêu trong ma trận</param>
    /// <param name="spawnEnemy">gameobject enemy</param>
    /// <param name="items">danh sách tổng item</param>
    private void SetRandomDropItems(int[] listRandom, int index, GameObject spawnEnemy, ref List<int> items, int totalCoin, int[] listRandomCoin)
    {
        List<int> itemOfOneEnemy = new List<int>();
        for (int i = 0; i < listRandom[index]; i++)
        {
            itemOfOneEnemy.Add(items[i]);
        }
        // xóa danh sách list item
        for (int i = 0; i < listRandom[index]; i++)
        {
            if (i < items.Count)
            {
                items.RemoveAt(0);
            }
        }
        spawnEnemy.GetComponent<BaseEnemy>().SetCoind(listRandomCoin[index]);
        spawnEnemy.GetComponent<BaseEnemy>().SetDopItem(itemOfOneEnemy);
    }

    /// <summary>
    /// hàm sinh random danh sách enemy sẽ được set drop item
    /// </summary>
    /// <param name="totalEnemy">tổng enemy</param>
    /// <param name="totalItemDrop">tổng item</param>
    /// <returns>mảng chứa các phần tử là số lượng drop </returns>
    private int[] Randomized(int totalEnemy, int totalItemDrop)
    {
        int[] randomList = new int[totalEnemy];
        for (int i = 0; i < totalEnemy; i++)
        {
            randomList[i] = 0;
        }
        int index = 0;
        int totalRandom = 0;
        while (true)
        {
            if (totalRandom == totalItemDrop)
            {
                break;
            }
            int random = (int)Random.Range(1f, 6.5f);
            index += random;
            randomList[index % totalEnemy] += 1;
            totalRandom += 1;
        }

        return randomList;
    }

    #endregion

    #endregion

    #region Public Method

    /// <summary>
    /// sinh enemy từ boss
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="parentTransform"></param>
    /// <returns></returns>
    public GameObject SpawnEnemy(EnemyInformation enemy, Transform parentTransform)
    {
        var obj = Lean.LeanPool.Spawn(_enemies[enemy.IdEnemy], parentTransform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        var baseEnemy = obj.GetComponent<BaseEnemy>();
        baseEnemy.SetCoind((int)Random.Range(0, 3));
        baseEnemy.Init(enemy, false, true);
        baseEnemy.SetTargetPosition(parentTransform.position);
        HandleEvent.Instance.AddEnemy(obj);
        return obj;
    }
    
    /// <summary>
    /// sinh enemy nếu như wave ko có boss
    /// tất cả enemy sẽ được sinh tại đây và được cộng vào 1 list
    /// và được setting các thông số như coin, item, path,..
    /// cách di chuyển sẽ quyết định lựa chọn con nào trong list để di chuyển
    /// </summary>
    /// <param name="wave"></param>
    public void SpawnEnemy(WaveInformation wave)
    {
        // danh sách tổng item trên wave
        // nếu item được cộng vào enemy thì danh sách này sẽ bị xóa item đó
        List<int> itemOnWave = new List<int>();
        List<GameObject> enemies = new List<GameObject>();
        for (int i = 0; i < wave.ListItemDop.Count; i++)
        {
            for (int j = 0; j < wave.ListItemDop[i].Count; j++)
            {
                itemOnWave.Add(wave.ListItemDop[i].IdItem);
            }
        }
        int totalCoin = (int)Random.Range(wave.MinCoindDrop, wave.MaxCoindDrop);
        int[] listRandom = Randomized(wave.Enemies.Count, itemOnWave.Count);
        int[] listCoinDrop = Randomized(wave.Enemies.Count, totalCoin);
        int index = 0;
        for (int i = 0; i < wave.Row; i++)
        {
            for (int j = 0; j < wave.Col; j++)
            {
                string startPos = GameTag.LEFT;
                switch (wave.Enemies[index].StartPosition)
                {
                    case StartPosition.Left:
                        startPos = GameTag.LEFT;
                        break;
                    case StartPosition.Right:
                        startPos = GameTag.RIGHT;
                        break;
                    case StartPosition.Top:
                        startPos = GameTag.TOP;
                        break;
                    case StartPosition.Bottom:
                        startPos = GameTag.BOTTOM;
                        break;
                }

                if (wave.Enemies[index].IdEnemy < 0)
                {
                    var empty = Lean.LeanPool.Spawn(_emptyGO);
                    empty.transform.SetParent(_parenTransform);
                    enemies.Add(empty);
                }
                else
                {
                    GameObject enemySpawn = Lean.LeanPool.Spawn(_enemies[wave.Enemies[index].IdEnemy],
                        _startPositionSpawnVector3[startPos],
                        Quaternion.identity);
                    enemySpawn.transform.position = _startPositionSpawnVector3[startPos];
                    enemies.Add(enemySpawn);
                    enemySpawn.transform.SetParent(transform);
                    // add enemy to controller
                    HandleEvent.Instance.AddEnemy(enemySpawn);
                    enemySpawn.GetComponent<BaseEnemy>().SetTargetPosition(GetTargetPosition(i, j, wave));
                    if (index + 1 == wave.Enemies.Count)
                    {
                        enemySpawn.GetComponent<BaseEnemy>().Init(wave.Enemies[index], true, false);
                    }
                    else
                    {
                        enemySpawn.GetComponent<BaseEnemy>().Init(wave.Enemies[index]);
                    }
                    // gọi hàm drop item
                    SetRandomDropItems(listRandom, index, enemySpawn, ref itemOnWave, totalCoin, listCoinDrop);
                }
                index++;
            }
        }
        switch (wave.TypeMove)
        {
            case TypeMove.OneByOne:
                StartCoroutine(SpawnEnemyTypeMoveOneByOne(wave, enemies));
                break;
            case TypeMove.MoveInCol:
                StartCoroutine(SpawnEnemyTypeMoveInCol(wave, enemies));
                break;
            case TypeMove.MoveInRows:
                StartCoroutine(SpawnEnemyTypeMoveInRow(wave, enemies));
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
