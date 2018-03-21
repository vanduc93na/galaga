using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseBoss : MonoBehaviour
{
    [Tooltip("thời gian delay sinh enemies")]
    [SerializeField] private float _timeDelaySpawnEnemiesAttack = 0;
    [Tooltip("thời gian delay để di chuyển trên màn hình")]
    [SerializeField] private float _timeDelayMove = 0;
    [Tooltip("thanh máu")]
    [SerializeField] private Transform _healthTransform;
    [SerializeField]
    private SkeletonAnimation _skAnim;

    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _deadEffect;
    [Tooltip("path cho enemies di chuyển")] [SerializeField] private GameObject _pathObj;
    private MoveInformation _moveInfor;
    private Vector2 _nextPosition;
//    private Animator anim;
    private int _hitState;
    private int _deadState;
    protected BossInfor _bossInfor;
    private int _health = 0;
    private const string MOVE_METHOD = "MoveToTarget";
    private const string ATTACK_METHOD = "SpawnEnemiesAttack";
    protected WaveBoss _waveBossInfor;
    protected bool _isMove;
    

    private float _minX = -2.2f, _minY = -4.2f, _maxX = 2.2f, _maxY = 4.2f;

    protected void Awake()
    {
        _isMove = false;
        _moveInfor = new MoveInformation();
        if (_pathObj.GetComponent<DOTweenPath>())
        {
            var tmp = _pathObj.GetComponent<DOTweenPath>();
            _moveInfor.Waypoint = tmp.wps;
            _moveInfor.Duration = tmp.duration;
            _moveInfor.Type = tmp.pathType;
        }
        else
        {
            print("object: " + _pathObj + " doen't contains component DOTweenPath");
        }
//        _renderer = GetComponent<SpriteRenderer>();
//        anim = GetComponent<Animator>();
        _hitState = GetComponent<BossHashIDs>().Hit;
        _deadState = GetComponent<BossHashIDs>().Dead;
        _bossInfor = new BossInfor();
    }

    protected void OnEnable()
    {
        CancelInvoke(MOVE_METHOD);
        Invoke(MOVE_METHOD, _timeDelayMove);
        Invoke(ATTACK_METHOD, _timeDelaySpawnEnemiesAttack);
    }

    #region Public Method

    /// <summary>
    /// trúng đạn
    /// </summary>
    /// <param name="dame"></param>
    public void OnHit(int dame)
    {
        _bossInfor.Health -= dame;
        _healthTransform.localScale = new Vector3(((float)_bossInfor.Health / _health), 1, 1);
        if (_bossInfor.Health <= 0)
        {
            StartCoroutine(Effect(0.3f, _deadEffect));
            StartCoroutine(OnDieAnimation());
        }
        else
        {
            StartCoroutine(Effect(0.1f, _hitEffect));
            OnHitAnimation();
        }
    }

    /// <summary>
    /// di chuyển từ ngoài màn hình vào
    /// </summary>
    /// <param name="moveInfor"></param>
    public void MoveToScreen(MoveInformation moveInfor)
    {

        List<Vector3> tmp = new List<Vector3>();
        for (int i = 0; i < moveInfor.Waypoint.Count; i++)
        {
            tmp.Add(moveInfor.Waypoint[i]);
        }

        Vector3[] wp = tmp.ToArray();
        _isMove = true;
        transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, PathMode.Sidescroller2D).OnComplete(() => _isMove = false);
    }

    /// <summary>
    /// di chuyển trong màn hình
    /// dùng invoke gọi trong OnEnable
    /// </summary>
    public void MoveToTarget()
    {
        float dx = Random.Range(_minX, _maxX);
        float dy = Random.Range(_minY, _maxY);
        Vector3 targetPos = new Vector3(dx, dy);
        _isMove = true;
        transform.DOMove(targetPos, 3f).OnComplete(() => _isMove = false);
        float waitSeconds = Random.Range(5f, 10f);
        Invoke(MOVE_METHOD, waitSeconds);
    }

    public void SetInforBoss(BossInfor bossInfor)
    {
        _health = bossInfor.Health;
        _bossInfor.IdPath = bossInfor.IdPath;
        _bossInfor.Health = bossInfor.Health;
        _bossInfor.MinCoin = bossInfor.MinCoin;
        _bossInfor.MaxCoin = bossInfor.MaxCoin;
        _bossInfor.IdBoss = bossInfor.IdBoss;
    }

    public void SetWaveInfor(WaveBoss waveBoss)
    {
        _waveBossInfor = waveBoss;
    }

    #endregion

    #region Private Method

    IEnumerator OnDieAnimation()
    {
        _skAnim.AnimationName = GameTag.ENEMY_DIE;
        yield return new WaitForSeconds(0.8f);
        int coins = Random.Range(_bossInfor.MinCoin, _bossInfor.MaxCoin);
        for (int i = 0; i < coins; i++)
        {
            GameObject coin = Lean.LeanPool.Spawn(HandleEvent.Instance.ItemsGameObject[0], transform.position, Quaternion.identity);
            HandleEvent.Instance.AddItem(coin);
        }
        this.PostEvent(EventID.BossDead, this.gameObject);
        Lean.LeanPool.Despawn(this);

    }

    private void OnHitAnimation()
    {
        if (_isMove) return;

        transform.DOLocalMoveY(transform.position.y + 0.01f, 0.02f).OnComplete(() =>
        {
            transform.DOLocalMoveY(transform.position.y - 0.01f, 0.02f);
        });
    }
    

    /// <summary>
    /// dùng invoke gọi trong hàn OnEnable
    /// </summary>
    void SpawnEnemiesAttack()
    {
        float randomAttackEnemie = Random.Range(0f, 1f);
        if (_waveBossInfor.IsSpawnEnemies && randomAttackEnemie < 1.3f && gameObject.activeSelf)
        {
            StartCoroutine(SpawnEnemyAttack());
        }
        if (gameObject.activeSelf)
        {
            Invoke(ATTACK_METHOD, _timeDelaySpawnEnemiesAttack);
        }
    }

    protected IEnumerator AttackAnimation()
    {
        _skAnim.AnimationName = GameTag.ENEMY_ATTACK;
        yield return new WaitForSeconds(1f);
        _skAnim.AnimationName = GameTag.ENEMY_IDLE;

    }
    
    /// <summary>
    /// tấn công bằng cách sinh enemy
    /// </summary>
    IEnumerator SpawnEnemyAttack()
    {
        //mặc định path sẽ là di chuyển từ trái sang phải - ngược chiều kim đồng hồ
        List<Vector3> pathFromRight = new List<Vector3>();
        for (int i = _moveInfor.Waypoint.Count - 1; i >= 0; i--)
        {
            pathFromRight.Add(_moveInfor.Waypoint[i]);
        }
        MoveInformation moveFromRight = new MoveInformation();
        moveFromRight.Waypoint = pathFromRight;
        moveFromRight.Duration = _moveInfor.Duration;
        moveFromRight.Mode = _moveInfor.Mode;
        moveFromRight.Type = _moveInfor.Type;
        List<GameObject> enemiesObj = new List<GameObject>();
        
        switch (_waveBossInfor.TypeSpawn)
        {
            case TypeSpawnBlock.FromLeft:
                if (_waveBossInfor.EnemySpawns.Count > 0)
                {
                    for (int i = 0; i < _waveBossInfor.CountBlock; i++)
                    {
                        int randomId = Random.Range(0, _waveBossInfor.EnemySpawns.Count - 1);
                        enemiesObj.Add(EnemyController.Instance.SpawnEnemy(_waveBossInfor.EnemySpawns[randomId], transform));
                    }
                    for (int i = 0; i < enemiesObj.Count; i++)
                    {
                        yield return new WaitForSeconds(_waveBossInfor.DelaySpawnenemy);
                        enemiesObj[i].GetComponent<BaseEnemy>().MovePathOnWave(_moveInfor);
                    }
                }
                
                break;
            case TypeSpawnBlock.FromRight:
                if (_waveBossInfor.EnemySpawns.Count > 0)
                {
                    for (int i = 0; i < _waveBossInfor.CountBlock; i++)
                    {
                        int randomId = Random.Range(0, _waveBossInfor.EnemySpawns.Count - 1);
                        enemiesObj.Add(EnemyController.Instance.SpawnEnemy(_waveBossInfor.EnemySpawns[randomId], transform));
                    }
                    for (int i = 0; i < enemiesObj.Count; i++)
                    {
                        yield return new WaitForSeconds(_waveBossInfor.DelaySpawnenemy);
                        enemiesObj[i].GetComponent<BaseEnemy>().MovePathOnWave(moveFromRight);
                    }
                }
                break;
            case TypeSpawnBlock.Both:
                if (_waveBossInfor.EnemySpawns.Count > 0)
                {
                    for (int i = 0; i < _waveBossInfor.CountBlock * 2; i++)
                    {
                        int randomId = Random.Range(0, _waveBossInfor.EnemySpawns.Count - 1);
                        enemiesObj.Add(EnemyController.Instance.SpawnEnemy(_waveBossInfor.EnemySpawns[randomId], transform));
                    }
                    for (int i = 0; i < enemiesObj.Count / 2; i++)
                    {
                        yield return new WaitForSeconds(_waveBossInfor.DelaySpawnenemy);
                        enemiesObj[i].GetComponent<BaseEnemy>().MovePathOnWave(_moveInfor);
                        enemiesObj[i + enemiesObj.Count / 2].GetComponent<BaseEnemy>().MovePathOnWave(moveFromRight);
                    }
                }
                break;
        }
    }

    IEnumerator Effect(float seconds, GameObject effect)
    {
        effect.SetActive(true);
        effect.transform.position = transform.position;
        yield return new WaitForSeconds(seconds);
        effect.SetActive(false);
    }

    #endregion
}
