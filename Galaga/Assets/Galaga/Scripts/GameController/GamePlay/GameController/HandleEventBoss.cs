using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// xử lý và quản lý các sự kiện liên quan đến boss
/// </summary>
public partial class HandleEvent
{
    private Dictionary<GameObject, BaseBoss> _bosses;
    [SerializeField] private GameObject _bossDeadEffect;

    #region Private Method

    void InitBoss()
    {
        _bosses = new Dictionary<GameObject, BaseBoss>();
    }

    void RegisterBossEvents()
    {
        this.RegisterListener(EventID.BossDead, (param) => RemoveBoss((GameObject)param));
    }

    void RemoveBoss(GameObject bossObj)
    {
        if (_bosses.ContainsKey(bossObj))
        {
            _bosses.Remove(bossObj);
            Lean.LeanPool.Despawn(bossObj);
            SoundController.PlaySoundEffect(SoundController.Instance.BossDead);
        }
        else
        {
            print("dict _bosses doesn't contain key: " + bossObj);
        }
        if (_bosses.Count == 0 && _enemiesOnWave.Count == 0)
        {
            this.PostEvent(EventID.NextWave);
        }
    }

    #endregion
    
    #region Public Method

    public void AddBoss(GameObject bossObj)
    {
        if (!_bosses.ContainsKey(bossObj))
        {
            _bosses.Add(bossObj, bossObj.GetComponent<BaseBoss>());
        }
        else
        {
            print("key: " + bossObj + " allready exits");
        }
        
    }

    #endregion
}
