using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// xử lý các sự kiện và quản lý các hành động enemy
/// </summary>
public partial class HandleEvent : MonoBehaviour
{
    public int EnemiesDestroy = 0;
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
    /// đạn của enemy
    /// </summary>
    [SerializeField] private GameObject _bulletEnemy;

    /// <summary>
    /// tâm hố đen
    /// </summary>
    [SerializeField] private GameObject _blackHoleCentre;

    [SerializeField] private GameObject _enemyDeadEffect;

    [SerializeField] private GameObject _enemyOnHitEffect;

    [SerializeField] private GameObject _eatCoinEffect;

    [SerializeField] private GameObject _enemiesMgr;

    /// <summary>
    /// thời gian tấn công của hố đen
    /// </summary>
    [SerializeField] private float _timeAttackBlackHole;

    [SerializeField] private Transform _parentButtletOfEnemies;

    /// <summary>
    /// quản lý tất cả các enemy sau khi sinh ra, nếu enemy nào chết thì sẽ bị remove khỏi danh sách
    /// khi hoàn thành level thì danh sách này sẽ được reset về rỗng
    /// </summary>
    private Dictionary<GameObject, BaseEnemy> _enemiesOnWave;

    private Dictionary<GameObject, Tomahawk> _tomahawks;

    private Dictionary<GameObject, Arrow> _arrows;

    private Dictionary<GameObject, Genade> _genades;

    /// <summary>
    /// quản lý tất cả các đạn sau khi sinh ra, nếu viên đạn nào va chạm thì sẽ bị remove khỏi danh sách
    /// khi hoàn thành level thì danh sách này sẽ được reset về rỗng
    /// </summary>
    private Dictionary<GameObject, BasicBullet> _bulletsSpawn;



    private const string MOVE_ON_WAVE_METHOD = "MoveEnemyOnWave";
    private const string ENEMY_ATTACK = "EnemyAttack";
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

    public static HandleEvent Instance;


    void Awake()
    {
        InitForEnemies();
        RegisterEnemiesEvents();
        InitBoss();
        RegisterBossEvents();
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void InitForEnemies()
    {
        EnemiesDestroy = 0;
        _blackHoleCentre.SetActive(false);
        _listItemsOnWave = new Dictionary<GameObject, BaseItem>();
        ItemsGameObject = new Dictionary<int, GameObject>();
        for (int i = 0; i < _itemsMgr.transform.childCount; i++)
        {
            ItemsGameObject.Add(i, _itemsMgr.transform.GetChild(i).gameObject);
        }
        _enemiesOnWave = new Dictionary<GameObject, BaseEnemy>();
        _bulletsSpawn = new Dictionary<GameObject, BasicBullet>();
        _tomahawks = new Dictionary<GameObject, Tomahawk>();
        _genades = new Dictionary<GameObject, Genade>();
        _arrows = new Dictionary<GameObject, Arrow>();
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

    void Start()
    {
        EnemyAttack();
        MoveEnemyOnWave();
        SoundController.PlayBackgroundSound(SoundController.Instance.GameBackgroundSound);
    }

    void RegisterEnemiesEvents()
    {
        this.RegisterListener(EventID.EnemyDead, (param) => RemoveEnemy((GameObject)param));
        // event enemy cuối đến vị trí cuối cùng - dùng để thực hiện di chuyển enemy sau khi xếp map
        this.RegisterListener(EventID.LastEnemyMoveDone, (param) =>
        {
            MoveAllEnemies();
            MoveEnemyOnWave();
        });

        this.RegisterListener(EventID.Restart, (param) => RestartGame());
    }

    void RestartGame()
    {
        var keysEnemy = _enemiesOnWave.Keys.ToList();
        for (int i = 0; i < keysEnemy.Count; i++)
        {
            Lean.LeanPool.Despawn(_enemiesOnWave[keysEnemy[i]]);
        }
        var keyBoss = _bosses.Keys.ToList();

        for (int i = 0; i < keyBoss.Count; i++)
        {
            Lean.LeanPool.Despawn(_bosses[keyBoss[i]]);
        }
        Reset();
    }

    void MoveAllEnemies()
    {
        float nextXPossition = Random.Range(-1, 1);
        _enemiesMgr.transform.DOMoveX(nextXPossition, 1f);
        Invoke("MoveAllEnemies", 1.5f);
    }

    /// <summary>
    /// di chuyển quái random trong ma trận sau khi sắp xếp
    /// </summary>
    void MoveEnemyOnWave()
    {

        if (_enemiesOnWave.Count > 0)
        {
            List<GameObject> listEnemies = _enemiesOnWave.Keys.ToList();
            //            int numberOfEnemyMove = Random.Range(0, 5);
            //            int randomPath = Random.Range(0, _listPathMoveOnWave.Count);
            //            while (numberOfEnemyMove > 0)
            //            {
            //                int randomEnemy = Random.Range(1, listEnemies.Count - 1);
            //                if (_enemiesOnWave[listEnemies[randomEnemy]].OnMoving())
            //                {
            //                    _enemiesOnWave[listEnemies[randomEnemy]].MovePathOnWave(_listPathMoveOnWave[randomPath]);
            //                    StartCoroutine(DelayTime(0.2f, null));
            //                    numberOfEnemyMove--;
            //                }
            //                print(randomEnemy + " number: " + numberOfEnemyMove);
            //            }
            //            for (int i = 0; i < numberOfEnemyMove; i++)
            //            {
            //                int randomEnemy = Random.Range(1, listEnemies.Count);
            //                _enemiesOnWave[listEnemies[randomEnemy]].MovePathOnWave(_listPathMoveOnWave[randomPath]);
            //                StartCoroutine(DelayTime(0.2f, null));
            //            }
            int random = Random.Range(0, listEnemies.Count - 1);
            if (!_enemiesOnWave[listEnemies[random]].OnMoving())
            {
                Vector3 rootPos = _enemiesOnWave[listEnemies[random]].transform.localPosition;
                Vector3 playerPos = _player.transform.position;
                _enemiesOnWave[listEnemies[random]].transform.DOLocalMove(playerPos, 2f).OnComplete(() =>
                {
                    _enemiesOnWave[listEnemies[random]].transform.DOLocalMove(rootPos, 1f);
                });
            }
            float timeInvoke = Random.Range(3f, 5f);
            Invoke(MOVE_ON_WAVE_METHOD, timeInvoke);
        }
        else
        {
            CancelInvoke(MOVE_ON_WAVE_METHOD);
        }
    }

    void EnemyAttack()
    {
        if (_enemiesOnWave.Count > 0)
        {
            int random = Random.Range(0, _enemiesOnWave.Count - 1);
            var listEnemy = _enemiesOnWave.Keys.ToList();
            _enemiesOnWave[listEnemy[random]].Attack(_bulletEnemy, _parentButtletOfEnemies);
        }
        float timerInvoke = Random.Range(1f, 5f);
        Invoke(ENEMY_ATTACK, timerInvoke);

    }

    IEnumerator DelayTime(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }

    /// <summary>
    /// khi enemy chết thi remove key và value đi
    /// </summary>
    /// <param name="enemy"></param>
    private void RemoveEnemy(GameObject enemy)
    {
        if (_enemiesOnWave.ContainsKey(enemy))
        {
            if (_enemiesOnWave[enemy].IsAlive())
            {
                StartCoroutine(Effect(_enemyDeadEffect, _enemiesOnWave[enemy].transform.position, 0.3f));
            }
            _enemiesOnWave.Remove(enemy);
            EnemiesDestroy += 1;
            SoundController.PlaySoundEffect(SoundController.Instance.EnemyDead);
        }

        if (_enemiesOnWave.Count == 0 && _bosses.Count == 0)
        {
            this.PostEvent(EventID.NextWave);
        }
    }

    IEnumerator Effect(GameObject effect, Vector3 position, float time)
    {
        var effectSpawn = Lean.LeanPool.Spawn(effect);
        effectSpawn.transform.position = position;
        yield return new WaitForSeconds(time);
        Lean.LeanPool.Despawn(effectSpawn);
    }

    #region Public Methods

    /// <summary>
    /// lắng nghe và xử lý các va chạm xảy ra của viên đạn
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
                    var temp = _enemiesOnWave[targetTriggerObject];
                    _enemiesOnWave[targetTriggerObject].OnHit(dame);
                    if (temp.IsAlive())
                    {
                        StartCoroutine(Effect(_enemyOnHitEffect, temp.transform.position, 0.3f));
                    }
                }

            }
            if (targetTriggerObject.tag == GameTag.BOSS)
            {
                if (_bosses.ContainsKey(targetTriggerObject))
                {
                    int dame = _bulletsSpawn[trigger].Dame();
                    _bosses[targetTriggerObject].OnHit(dame);
                    if (_bosses[targetTriggerObject].IsAlive())
                    {
                        StartCoroutine(Effect(_enemyOnHitEffect, _enemiesOnWave[targetTriggerObject].transform.position, 00.3f));
                    }
                }
            }

            if (targetTriggerObject.tag == GameTag.BORDER || targetTriggerObject.tag == GameTag.ENEMY || targetTriggerObject.tag == GameTag.BOSS)
            {
                Lean.LeanPool.Despawn(trigger);
                _bulletsSpawn.Remove(trigger);
            }

        }
    }

    public void TriggerTomahawkVsEnemies(GameObject tomahawk, GameObject targetObject)
    {
        if (_tomahawks.ContainsKey(tomahawk))
        {
            if (targetObject.tag == GameTag.ENEMY)
            {
                if (_enemiesOnWave.ContainsKey(targetObject))
                {
                    var temp = _enemiesOnWave[targetObject];
                    int dame = tomahawk.GetComponent<Tomahawk>().GetDame();
                    _enemiesOnWave[targetObject].OnHit(dame);
                    if (temp.IsAlive())
                    {
                        StartCoroutine(Effect(_enemyOnHitEffect, temp.transform.position, 0.3f));
                    }
                }
            }

            if (targetObject.tag == GameTag.BORDER || targetObject.tag == GameTag.ENEMY)
            {
                _tomahawks.Remove(tomahawk);
                Lean.LeanPool.Despawn(tomahawk);
            }
        }
    }

    public void TriggerGenadevsEnemies(GameObject genade, GameObject targetObject)
    {
        if (targetObject.tag == GameTag.ENEMY)
        {
            List<GameObject> enemiesKeyGO = _enemiesOnWave.Keys.ToList();
            int dame = genade.GetComponent<Genade>().GetDame();
            for (int i = 0; i < enemiesKeyGO.Count; i++)
            {
                float distance = Vector3.Distance(genade.transform.position, _enemiesOnWave[enemiesKeyGO[i]].transform.position);
                if (distance <= 2)
                {
                    var temp = _enemiesOnWave[enemiesKeyGO[i]];
                    int dameTaken = (int)(dame * (1 - (float)distance / 2));
                    _enemiesOnWave[enemiesKeyGO[i]].OnHit(dameTaken);
                    if (temp.IsAlive())
                    {
                        StartCoroutine(Effect(_enemyOnHitEffect, temp.transform.position, 0.3f));
                    }
                }
            }
            if (_genades.ContainsKey(genade))
            {
                Lean.LeanPool.Despawn(genade);
                _genades.Remove(genade);
            }
        }
        else if (targetObject.tag == GameTag.BORDER)
        {
            if (_genades.ContainsKey(genade))
            {
                Lean.LeanPool.Despawn(genade);
                _genades.Remove(genade);
            }
        }
    }

    public void TriggerArrowVsEnemies(GameObject arrow, GameObject targetObject)
    {
        if (_arrows.ContainsKey(arrow))
        {
            if (targetObject.tag == GameTag.ENEMY)
            {
                if (_enemiesOnWave.ContainsKey(targetObject))
                {
                    var temp = _enemiesOnWave[targetObject];
                    int dame = arrow.GetComponent<Arrow>().GetDame();
                    _enemiesOnWave[targetObject].OnHit(dame);
                    if (temp.IsAlive())
                    {
                        StartCoroutine(Effect(_enemyOnHitEffect, temp.transform.position, 0.3f));
                    }
                }
            }
            if (targetObject.tag == GameTag.BORDER)
            {
                Lean.LeanPool.Despawn(targetObject);
                _arrows.Remove(arrow);
            }
        }
    }

    public IEnumerator BlackHoleAttack(float seconds, int dame)
    {
        _blackHoleCentre.SetActive(true);
        StartCoroutine(DelayTime(seconds, () =>
        {
            _blackHoleCentre.SetActive(false);
        }));

        List<GameObject> enemiesGO = _enemiesOnWave.Keys.ToList();

        for (int i = 0; i < enemiesGO.Count; i++)
        {
            if (_enemiesOnWave[enemiesGO[i]].IsAlive())
            {
                _enemiesOnWave[enemiesGO[i]].BlackHoleAttack(_blackHoleCentre);
            }
            else
            {
                continue;
            }
        }

        for (int i = 0; i < enemiesGO.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            if (_enemiesOnWave[enemiesGO[i]].IsAlive())
            {
                _enemiesOnWave[enemiesGO[i]].OnHit(dame);
            }
            else
            {
                continue;
            }
        }
    }

    public void TriggerLazerVsOther(GameObject other, int dame)
    {
        if (other.tag == GameTag.ENEMY)
        {
            var temp = _enemiesOnWave[other];
            _enemiesOnWave[other].OnHit(dame);
            if (temp.IsAlive())
            {
                StartCoroutine(Effect(_enemyOnHitEffect, temp.transform.position, 0.3f));
            }
        }

        if (other.tag == GameTag.BOSS)
        {
            var temp = _bosses[other];
            _bosses[other].OnHit(dame);
            if (temp.IsAlive())
            {
                StartCoroutine(Effect(_enemyOnHitEffect, temp.transform.position, 0.3f));
            }
        }
    }
    /// <summary>
    /// xư lý va chạm của item với player
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
                    if (item.tag == GameTag.ITEM_COIN)
                    {
                        SoundController.PlaySoundEffect(SoundController.Instance.EatCoin);
                        StartCoroutine(Effect(_eatCoinEffect, other.transform.position, 0.5f));
                    }
                    else
                    {
                        SoundController.PlaySoundEffect(SoundController.Instance.EatItem);
                    }
                    this.PostEvent(EventID.EatItem, item);
                    Lean.LeanPool.Despawn(item);
                    RemoveItem(item);
                }
                else
                {
                    print("_listItemOnWave doen't contains key: " + item);
                }

            });
        }
        else if (other.tag == GameTag.BORDER)
        {
            if (_listItemsOnWave.ContainsKey(item))
            {
                RemoveItem(item);
                Lean.LeanPool.Despawn(item);
            }
            else
            {
                print("_listItemOnWave doen't contains key: " + item);
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
    /// 
    /// </summary>
    /// <param name="tomahawk"></param>
    public void AddTomahawk(GameObject tomahawk)
    {
        _tomahawks.Add(tomahawk, tomahawk.GetComponent<Tomahawk>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genade"></param>
    public void AddGenade(GameObject genade)
    {
        _genades.Add(genade, genade.GetComponent<Genade>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrow"></param>
    public void AddArrow(GameObject arrow)
    {
        _arrows.Add(arrow, arrow.GetComponent<Arrow>());
    }

    /// <summary>
    /// hàm reset được gọi sau khi hoàn thành wave
    /// </summary>
    public void Reset()
    {
        EnemiesDestroy = 0;
        _enemiesOnWave.Clear();
        _tomahawks.Clear();
        _genades.Clear();
        _arrows.Clear();
        _bulletsSpawn.Clear();
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

    /// <summary>
    /// lấy tất cả danh sách script của enemies còn sống
    /// </summary>
    /// <returns></returns>
    public List<BaseEnemy> GetAllEnemiesAlive()
    {
        return _enemiesOnWave.Values.ToList();
    }

    #endregion
}
