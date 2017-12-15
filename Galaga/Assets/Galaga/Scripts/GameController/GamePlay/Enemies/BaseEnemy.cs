using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IHealth
{
    [SerializeField] private int _health = 0;

    private bool _isLastEnemyOnWave = false;
//    private EnemyInformation _enemyInfor;
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
