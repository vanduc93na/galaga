using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;

public class BaseEnemy : MonoBehaviour, IHealth
{
    public int test = 0;
    [SerializeField] private int _health = 0;
    // danh sách items drop
    private List<int> _itemsDrop;
    private bool _isLastEnemyOnWave = false;
    private int _coinDrop = 0;
    private Vector2 _targetPosition;

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
        transform.DOLocalMoveY(transform.position.y + 0.05f, 0.05f).OnComplete(() =>
        {
            transform.DOLocalMoveY(transform.position.y - 0.05f, 0.05f);
        });
    }

    public void Init(EnemyInformation infor, bool isLast = false)
    {
        
        _health = infor.Health;
        _isLastEnemyOnWave = isLast;
    }

    public void SetDopItem(List<int> items)
    {
        _itemsDrop = new List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            _itemsDrop.Add(items[i]);
        }
        test += items.Count;
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
        if (_isLastEnemyOnWave)
        {
            transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, moveInfor.Mode, 10, null).OnComplete(() =>
            {
                this.PostEvent(EventID.LastEnemyMoveDone);
            });
        }
        else
        {
            transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, moveInfor.Mode, 10, null);
        }
    }

    /// <summary>
    /// di chuyển sau khi sắp xếp xong đội hình
    /// </summary>
    /// <param name="wp"></param>
    public void MovePathOnWave(Vector3[] wp)
    {
        
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
        for (int i = 0; i < _itemsDrop.Count; i++)
        {
            Lean.LeanPool.Spawn(HandleEvent.Instance.ItemsGameObject[_itemsDrop[i]], transform.position, Quaternion.identity);
        }
        
    }
    #endregion

    #region Private Method
    
    void OnDead()
    {
        Lean.LeanPool.Despawn(this);
    }

    void Attack()
    {
        
    }
    
    #endregion
}
