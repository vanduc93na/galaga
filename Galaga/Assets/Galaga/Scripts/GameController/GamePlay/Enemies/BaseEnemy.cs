﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private int _health = 0;
    [SerializeField] private GameObject _deadEffect;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _explodedBullet;
    [SerializeField] private int _numberExploded;
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
    private Vector3 _centreBlackHole;
    private float _rotateSpeed = 5f;
    private float _radius;
    private float _angle;
    public int id;

    void Awake()
    {
        id = 0;
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
            Vector3 smoothPos = new Vector3(Mathf.Lerp(transform.position.x, offet.x, 0.01f),
                Mathf.Lerp(transform.position.y, offet.y, 0.01f), 0);
            transform.position = _centreBlackHole + new Vector3(offet.x, offet.y, 0);
            _radius -= Time.deltaTime;
        }
    }

    void OnEnable()
    {
        _radius = 0;
        _isBlackHoleAttack = false;
        isAlive = true;
    }

    void OnDisable()
    {
        _isLastEnemyOnWave = false;
        DOTween.Kill(transform);
        _deadEffect.SetActive(false);
        _hitEffect.SetActive(false);
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
        id = infor.IdEnemy;
        isAlive = true;
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
        transform.position = wp[0];
        _onMove = true;
        transform.DOPath(wp, moveInfor.Duration, PathType.CatmullRom, PathMode.Sidescroller2D).SetEase(Ease.Linear).SetLookAt(lookAhead: 0).OnComplete(() =>
        {
            transform.DOLocalRotate(Vector3.up, 0.2f);
            _onMove = false;
            if (_isLastEnemyOnWave)
            {
                this.PostEvent(EventID.LastEnemyMoveDone);
            }

        });
    }

    public void BlackHoleAttack(GameObject blackHoleCentre, float timeAttack, int dame)
    {
        StartCoroutine(OnHitByBlackHole(timeAttack, dame));
        transform.DOMove(blackHoleCentre.transform.position, 2f).SetEase(Ease.InQuint).OnComplete(() => DOTween.Kill(transform));
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
        if (!IsActiveOnScene()) return;
        // instance item
        if (_itemsDrop != null)
        {
            for (int i = 0; i < _itemsDrop.Count; i++)
            {
                GameObject item = Lean.LeanPool.Spawn(HandleEvent.Instance.ItemsGameObject[_itemsDrop[i]], transform.position, Quaternion.identity);
                item.transform.position = transform.position;
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
        _isLastEnemyOnWave = false;
        isAlive = false;
        InstanceDropItem();
        DOTween.Kill(transform);
        ExplodedAttack();
//        transform.position = _rootPos;
    }

    IEnumerator OnHitByBlackHole(float seconds, int dame)
    {
        yield return new WaitForSeconds(seconds);
        OnHit(dame);
    }

    public void AttackSpawnEgg(GameObject bullet, Transform parentTransform)
    {
        var go = Lean.LeanPool.Spawn(bullet);
        go.transform.position = transform.position;
        go.transform.eulerAngles = new Vector3(0, 0, 180);
    }

    public void AttackShotBulletToShip(GameObject bullet, Transform parentTransform, Transform shipTrs)
    {
        //        float angle = Vector2.Angle(transform.position, shipTrs.position);
        if (!IsActiveOnScene())
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, shipTrs.position);
        if (distance < 2) return;
        Vector3 direction = shipTrs.position - transform.position;
         direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        
        transform.DOLocalRotate(new Vector3(0, 0, angle), 0.7f).OnComplete(() =>
        {
            var go = Lean.LeanPool.Spawn(bullet);
            go.transform.position = transform.position;
            go.transform.SetParent(parentTransform);
            go.transform.eulerAngles = new Vector3(0, 0, angle);
            transform.DOLocalRotate(Vector3.zero, 0.3f);
        });
    }

    /// <summary>
    /// sinh quả bom ra đến gần player
    /// </summary>
    public void SpawnBom(Vector3 playerPos, float radius, GameObject bom)
    {
        Vector3 triggerPos = (transform.position - playerPos) - (transform.position - playerPos).normalized * radius;
        var bomSpawn = Lean.LeanPool.Spawn(bom);
        bomSpawn.GetComponent<BulletEnemy>().SetTargetPos(playerPos, radius);
        Vector3 direcltion = playerPos - transform.position;
        direcltion.Normalize();
        float angle = Mathf.Atan2(direcltion.y, direcltion.x) * Mathf.Rad2Deg - 90;

        bomSpawn.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// lúc chết tạo hiệu ứng nổ văng
    /// </summary>
    public void ExplodedAttack()
    {

        if (id == 7 || id == 15)
        {
            float angle = 360 / _numberExploded;
            for (int i = 0; i < _numberExploded; i++)
            {
                var explodedBullet = Lean.LeanPool.Spawn(_explodedBullet);
                explodedBullet.transform.position = gameObject.transform.position;
                explodedBullet.transform.eulerAngles = new Vector3(0, 0, angle * i);
            }
        }
    }

    #endregion

    public bool IsAlive()
    {
        return isAlive;
    }

    public bool IsActiveOnScene()
    {
        var pos = transform.position;
        return pos.x <= 3 && pos.x >= -2.5f && pos.y >= -4.5 && pos.y <= 5;
    }
}
