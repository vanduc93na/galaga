using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleEvent : Singleton<HandleEvent>
{
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

    void Awake()
    {
        Init();
        RegisterEvent();
    }

    void Init()
    {
        _enemiesOnWave = new Dictionary<GameObject, BaseEnemy>();
        _bulletsSpawn = new Dictionary<GameObject, BasicBullet>();
    }

    void RegisterEvent()
    {
        this.RegisterListener(EventID.EnemyDead, (param) => RemoveEnemy((GameObject) param));
        this.RegisterListener(EventID.LastEnemyMoveDone, (param) => MoveEnemyOnWave());
    }

    private void MoveEnemyOnWave()
    {
        
    }
    
    private void RemoveEnemy(GameObject enemy)
    {
        if (_enemiesOnWave.ContainsKey(enemy))
        {
            _enemiesOnWave.Remove(enemy);
        }
    }

    #region Public Methods

    /// <summary>
    /// lắng nghe và xử lý các va chạm xảy ra
    /// </summary>
    /// <param name="trigger">object va chạm</param>
    /// <param name="targetTriggerObject">object bị va chạm</param>
    public void OnTriggerHandle(GameObject trigger, GameObject targetTriggerObject)
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
            Lean.LeanPool.Despawn(trigger);
            _bulletsSpawn.Remove(trigger);
        }
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

    #endregion
}
