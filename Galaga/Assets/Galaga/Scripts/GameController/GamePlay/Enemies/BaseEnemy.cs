using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private int _health = 0;
    // danh sách items drop
    private List<int> _itemsDrop;
    private bool _isLastEnemyOnWave = false;
    private int _coinDrop = 0;
    private Vector2 _targetPosition;
    private bool _onMove = false;
    private Vector3 _rootPos;
    private bool _isFromBoss = false;
    private GameObject _boss;

    void Awake()
    {
        _rootPos = transform.position;
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
        _isFromBoss = isFromBoss;
        _health = infor.Health;
        _isLastEnemyOnWave = isLast;
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
        if (_isLastEnemyOnWave)
        {
            transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, PathMode.Sidescroller2D).SetLookAt(lookAhead: 0).OnComplete(() =>
            {
                transform.DOLocalRotate(Vector3.up, 0.2f);
                _onMove = false;
                this.PostEvent(EventID.LastEnemyMoveDone);
            });
        }
        else
        {
            transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, PathMode.Sidescroller2D).SetLookAt(lookAhead: 0).OnComplete(() =>
            {
                transform.DOLocalRotate(Vector3.up, 0.2f);
                _onMove = false;
            });
        }
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
        InstanceDropItem();
        DOTween.Kill(transform);
        transform.position = _rootPos;
        Lean.LeanPool.Despawn(this);
    }

    void Attack()
    {
        
    }
    
    #endregion

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        print(other.gameObject.name);
//    }
}
