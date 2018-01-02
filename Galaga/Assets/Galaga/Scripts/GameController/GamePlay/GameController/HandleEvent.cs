using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// xử lý các sự kiện và quản lý các sự kiện cho enemy
/// </summary>
public partial class HandleEvent : Singleton<HandleEvent>
{
    /// <summary>
    /// prefab danh sách items drop
    /// </summary>
    [SerializeField] private GameObject _itemsMgr;

    /// <summary>
    /// game object chứa các path để quái di chuyển trong wave
    /// </summary>
    [SerializeField]
    private GameObject _pathMoveOnWave;

    /// <summary>
    /// player
    /// </summary>
    [SerializeField] private GameObject _player;

    /// <summary>
    /// quản lý tất cả các enemy sau khi sinh ra, nếu enemy nào chết thì sẽ bị remove khỏi danh sách
    /// khi hoàn thành level thì danh sách này sẽ được reset về rỗng
    /// </summary>
    private Dictionary<GameObject, BaseEnemy> _enemiesOnWave;

    /// <summary>
    /// quản lý tất cả các đạn sau khi sinh ra, nếu viên đạn nào va chạm thì sẽ bị remove khỏi danh sách
    /// khi hoàn thành level thì danh sách này sẽ được reset về rỗng
    /// </summary>
    private Dictionary<GameObject, BasicBullet> _bulletsSpawn;



    private const string MOVE_ON_WAVE_METHOD = "MoveEnemyOnWave";
    /// <summary>
    /// danh sách vector 3 chứa các path của _pathMoveOnWave object
    /// </summary>
    private List<MoveInformation> _listPathMoveOnWave;

    /// <summary>
    /// danh sách item và script quản lý item đang hiển thị trên màn hình
    /// </summary>
    private Dictionary<GameObject, BaseItem> _listItemsOnWave;


    #region Public Variable

    /// <summary>
    /// dict chứa danh sách items với key là id của items
    /// id = 0: coin
    /// id > 0: là id của các items khác như đạn, thêm đạn, supper,...
    /// </summary>
    public Dictionary<int, GameObject> ItemsGameObject;


    #endregion




    void Awake()
    {
        InitForEnemies();
        RegisterEnemiesEvents();
        InitBoss();
        RegisterBossEvents();
    }

    void InitForEnemies()
    {
        _listItemsOnWave = new Dictionary<GameObject, BaseItem>();
        ItemsGameObject = new Dictionary<int, GameObject>();
        for (int i = 0; i < _itemsMgr.transform.childCount; i++)
        {
            ItemsGameObject.Add(i, _itemsMgr.transform.GetChild(i).gameObject);
        }
        _enemiesOnWave = new Dictionary<GameObject, BaseEnemy>();
        _bulletsSpawn = new Dictionary<GameObject, BasicBullet>();
        _listPathMoveOnWave = new List<MoveInformation>();
        for (int i = 0; i < _pathMoveOnWave.transform.childCount; i++)
        {
            MoveInformation moveInfor = new MoveInformation();
            moveInfor.Waypoint = _pathMoveOnWave.transform.GetChild(i).GetComponent<DOTweenPath>().wps;
            moveInfor.Duration = _pathMoveOnWave.transform.GetChild(i).GetComponent<DOTweenPath>().duration;
            moveInfor.Type = _pathMoveOnWave.transform.GetChild(i).GetComponent<DOTweenPath>().pathType;
            moveInfor.Mode = _pathMoveOnWave.transform.GetChild(i).GetComponent<DOTweenPath>().pathMode;
            _listPathMoveOnWave.Add(moveInfor);
        }
    }

    void RegisterEnemiesEvents()
    {
        this.RegisterListener(EventID.EnemyDead, (param) => RemoveEnemy((GameObject)param));
        // event enemy cuối đến vị trí cuối cùng - dùng để thực hiện di chuyển enemy sau khi xếp map
        this.RegisterListener(EventID.LastEnemyMoveDone, (param) => MoveEnemyOnWave());
    }

    /// <summary>
    /// di chuyển quái random trong ma trận sau khi sắp xếp
    /// </summary>
    private void MoveEnemyOnWave()
    {
        if (_enemiesOnWave.Count > 0)
        {
            List<GameObject> listEnemies = _enemiesOnWave.Keys.ToList();
            int numberOfEnemyMove = Random.Range(0, 5);
            int randomPath = Random.Range(0, _listPathMoveOnWave.Count);
            //            while (numberOfEnemyMove > 0)
            //            {
            //                int randomEnemy = Random.Range(1, listEnemies.Count - 1);
            //                if (_enemiesOnWave[listEnemies[randomEnemy]].OnMoving())
            //                {
            //                    _enemiesOnWave[listEnemies[randomEnemy]].MovePathOnWave(_listPathMoveOnWave[randomPath]);
            //                    StartCoroutine(DelayTime(0.2f));
            //                    numberOfEnemyMove--;
            //                }
            //                print(randomEnemy + " number: " + numberOfEnemyMove);
            //            }
            //            for (int i = 0; i < numberOfEnemyMove; i++)
            //            {
            //                int randomEnemy = Random.Range(1, listEnemies.Count);
            //                _enemiesOnWave[listEnemies[randomEnemy]].MovePathOnWave(_listPathMoveOnWave[randomPath]);
            //                StartCoroutine(DelayTime(0.2f));
            //            }
            float timeInvoke = Random.Range(1f, 3f);
            Invoke(MOVE_ON_WAVE_METHOD, timeInvoke);
        }
        else
        {
            CancelInvoke(MOVE_ON_WAVE_METHOD);
        }
    }

    IEnumerator DelayTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    /// <summary>
    /// khi enemy chết thi remove key và value đi
    /// </summary>
    /// <param name="enemy"></param>
    private void RemoveEnemy(GameObject enemy)
    {
        if (_enemiesOnWave.ContainsKey(enemy))
        {
            _enemiesOnWave[enemy].InstanceDropItem();
            _enemiesOnWave.Remove(enemy);
        }

        if (_enemiesOnWave.Count == 0)
        {
            this.PostEvent(EventID.NextWave);
        }
    }

    #region Public Methods

    /// <summary>
    /// lắng nghe và xử lý các va chạm xảy ra
    /// </summary>
    /// <param name="trigger">object va chạm</param>
    /// <param name="targetTriggerObject">object bị va chạm</param>
    public void TriggerBulletVsOther(GameObject trigger, GameObject targetTriggerObject)
    {
        if (_bulletsSpawn.ContainsKey(trigger))
        {
            if (targetTriggerObject.tag == GameTag.ENEMY)
            {
                int dame = _bulletsSpawn[trigger].Dame();
                if (_enemiesOnWave.ContainsKey(targetTriggerObject))
                {
                    _enemiesOnWave[targetTriggerObject].OnHit(dame);
                }
                
            }
            if (targetTriggerObject.tag == GameTag.BOSS)
            {
                if (_bosses.ContainsKey(targetTriggerObject))
                {
                    int dame = _bulletsSpawn[trigger].Dame();
                    _bosses[targetTriggerObject].OnHit(dame);
                }
            }
            
            if (targetTriggerObject.tag == GameTag.BORDER || targetTriggerObject.tag == GameTag.ENEMY || targetTriggerObject.tag == GameTag.BOSS)
            {
                Lean.LeanPool.Despawn(trigger);
                _bulletsSpawn.Remove(trigger);
            }

        }
    }

    /// <summary>
    /// xư lý va chạm của viên đạn
    /// </summary>
    /// <param name="item"></param>
    /// <param name="other"></param>
    public void TriggerItemVsOther(GameObject item, GameObject other)
    {

        if (other.tag == GameTag.PLAYER)
        {
            item.GetComponent<BaseItem>().MoveToPlayer(_player.transform.position, () =>
            {
                if (_listItemsOnWave.ContainsKey(item))
                {
                    RemoveItem(item);
                    Lean.LeanPool.Despawn(item);
                }
            });
        }
        else
        {
            if (_listItemsOnWave.ContainsKey(item))
            {
                RemoveItem(item);
                Lean.LeanPool.Despawn(item);
            }
        }


    }

    public void AddItem(GameObject item)
    {
        if (_listItemsOnWave.ContainsKey(item))
        {
            Debug.Log("item: " + item.name + " already exits");
            return;
        }
        _listItemsOnWave.Add(item, item.GetComponent<BaseItem>());
    }

    public void RemoveItem(GameObject item)
    {
        if (!_listItemsOnWave.ContainsKey(item))
        {
            Debug.Log("list item is not contain key: " + item.name);
            return;
        }
        _listItemsOnWave.Remove(item);
    }
    /// <summary>
    /// thêm 1 viên đạn vào danh sách
    /// </summary>
    /// <param name="bullet">gameobject là viên đạn</param>
    public void AddBullet(GameObject bullet)
    {
        _bulletsSpawn.Add(bullet, bullet.GetComponent<BasicBullet>());
    }

    /// <summary>
    /// thêm enemy vào danh sách các enemy có trên màn hình
    /// </summary>
    /// <param name="enemy">gameobject là enemy</param>
    public void AddEnemy(GameObject enemy)
    {
        _enemiesOnWave.Add(enemy, enemy.GetComponent<BaseEnemy>());
    }

    /// <summary>
    /// hàm reset được gọi sau khi hoàn thành wave
    /// </summary>
    public void Reset()
    {
        _enemiesOnWave.Clear();
    }

    public BaseEnemy GetBaseEnemyScript(GameObject obj)
    {
        if (_enemiesOnWave.ContainsKey(obj))
        {
            return _enemiesOnWave[obj];
        }
        Debug.Log("_enemies dict is not constain Key: " + obj);
        return null;
    }

    #endregion
}
