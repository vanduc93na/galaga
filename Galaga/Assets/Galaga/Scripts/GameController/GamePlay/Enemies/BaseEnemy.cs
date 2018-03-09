using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private int _health = 0;

    private bool isAlive;

    // danh sách items drop
    private List<int> _itemsDrop;
    private bool _isLastEnemyOnWave = false;
    private int _coinDrop = 0;
    private Vector2 _targetPosition;
    private bool _onMove = false;
    private Vector3 _rootPos;
    private bool _isFromBoss = false;
    private GameObject _boss;

    private bool _isBlackHoleAttack;
    private Vector2 _centreBlackHole;
    private float _rotateSpeed = 1f;
    private float _radius;
    private float _angle;

    void Awake()
    {

        _isBlackHoleAttack = false;
        isAlive = true;
        _rootPos = transform.position;
    }

    void Update()
    {
        if (_isBlackHoleAttack && _radius > 0)
        {
            _angle += _rotateSpeed * Time.deltaTime;
            var offet = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), 0) * _radius;
            transform.position = new Vector3(_centreBlackHole.x, _centreBlackHole.y, 0) + new Vector3(offet.x, offet.y, 0);
            _radius -= Time.deltaTime;
        }
    }

    void OnEnable()
    {
        _isBlackHoleAttack = false;
        isAlive = true;
    }

    #region Public Method
    public void OnHit(int dame)
    {
        _health -= dame;
        if (_health <= 0)
        {
            this.PostEvent(EventID.EnemyDead, this.gameObject);
            OnDead();
        }
        else
        {
            AnimationOnHit();
        }
    }

    public void AnimationOnHit()
    {
        transform.DOLocalMoveY(transform.position.y + 0.04f, 0.02f).OnComplete(() =>
        {
            transform.DOLocalMoveY(transform.position.y - 0.04f, 0.02f);
        });
    }

    public void Init(EnemyInformation infor, bool isLast = false, bool isFromBoss = false)
    {
        isAlive = true;
        _isFromBoss = isFromBoss;
        _health = infor.Health;
        _isLastEnemyOnWave = isLast;
        if (_isLastEnemyOnWave)
        {
            print("last");
        }
    }

    public void SetCoind(int coin)
    {
        _coinDrop = coin;
    }

    public void SetDopItem(List<int> items)
    {
        _itemsDrop = new List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            _itemsDrop.Add(items[i]);
        }
    }
    /// <summary> 
    /// di chuyển từ lúc sinh đến lúc sắp xếp xong ma trận trên màn hình
    /// </summary>
    /// <param name="waypoints"></param>
    public void MovePathToSortMatrix(MoveInformation moveInfor)
    {
        List<Vector3> tmp = new List<Vector3>();
        for (int i = 0; i < moveInfor.Waypoint.Count; i++)
        {
            tmp.Add(moveInfor.Waypoint[i]);
        }
        // thêm vị trí cuối cùng vào waypoint
        tmp.Add(_targetPosition);
        Vector3[] wp = tmp.ToArray();
        _onMove = true;
        transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, PathMode.Sidescroller2D).SetLookAt(lookAhead: 0).OnComplete(() =>
        {
            transform.DOLocalRotate(Vector3.up, 0.2f);
            _onMove = false;
            if (_isLastEnemyOnWave)
            {
                this.PostEvent(EventID.LastEnemyMoveDone);
            }

        });
    }

    public void BlackHoleAttack(GameObject blackHoleCentre)
    {
        _centreBlackHole = blackHoleCentre.transform.position;
        DOTween.Kill(transform);
        _isBlackHoleAttack = true;
        _radius = Vector2.Distance(transform.position, blackHoleCentre.transform.position);
        _angle = Mathf.Atan2(transform.position.y, transform.position.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// di chuyển sau khi sắp xếp xong đội hình
    /// </summary>
    /// <param name="wp"></param>
    public void MovePathOnWave(MoveInformation moveInfor)
    {
        List<Vector3> tmp = new List<Vector3>();
        for (int i = 0; i < moveInfor.Waypoint.Count; i++)
        {
            tmp.Add(moveInfor.Waypoint[i]);
        }
        // thêm vị trí cuối cùng vào waypoint
        tmp.Add(_targetPosition);
        Vector3[] wp = tmp.ToArray();
        _onMove = true;
        transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, moveInfor.Mode, 10, null).OnComplete(() =>
        {
            _onMove = false;
            this.PostEvent(EventID.EnemyDead, this.gameObject);
            Lean.LeanPool.Despawn(this);
        });
    }

    /// <summary>
    /// vị trí cuối cùng sau khi di chuyển theo path
    /// </summary>
    /// <param name="target"></param>
    public void SetTargetPosition(Vector3 target)
    {
        _targetPosition = target;
    }

    /// <summary>
    /// sinh item sau khi chết
    /// </summary>
    /// <param name="itemObject"></param>
    public void InstanceDropItem()
    {
        // instance item
        if (_itemsDrop != null)
        {
            for (int i = 0; i < _itemsDrop.Count; i++)
            {
                GameObject item = Lean.LeanPool.Spawn(HandleEvent.Instance.ItemsGameObject[_itemsDrop[i]], transform.position, Quaternion.identity);
                HandleEvent.Instance.AddItem(item);
            }
        }

        // instance coin
        for (int i = 0; i < _coinDrop; i++)
        {
            GameObject coin = Lean.LeanPool.Spawn(HandleEvent.Instance.ItemsGameObject[0], transform.position, Quaternion.identity);
            HandleEvent.Instance.AddItem(coin);
        }

    }

    /// <summary>
    /// kiểm tran trạng thái của quái trên màn hình có đang di chuyển không
    /// </summary>
    /// <returns></returns>
    public bool OnMoving()
    {
        return _onMove;
    }
    #endregion

    #region Private Method

    void OnDead()
    {
        isAlive = false;
        InstanceDropItem();
        DOTween.Kill(transform);
        transform.position = _rootPos;
        Lean.LeanPool.Despawn(this);
    }

    public void Attack(GameObject bullet)
    {
        var go = Lean.LeanPool.Spawn(bullet);
        go.transform.position = transform.position;
    }

    #endregion

    public bool IsAlive()
    {
        return isAlive;
    }
}
