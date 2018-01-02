using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseBoss : MonoBehaviour
{
    private int _health = 0;
    private int _coinDrop = 0;
    private Vector2 _nextPosition;
    private Animator anim;
    private int _hitState;
    private int _deadState;

    void Awake()
    {
        anim = GetComponent<Animator>();
        _hitState = GetComponent<BossHashIDs>().Hit;
        _deadState = GetComponent<BossHashIDs>().Dead;
    }

    #region Public Method

    public void OnHit(int dame)
    {
        _health -= dame;
        if (_health <= 0)
        {
            OnDeadAnimation();
        }
        else
        {
            OnHitAnimation();
        }
    }

    public void MoveToScreen(MoveInformation moveInfor)
    {
        List<Vector3> tmp = new List<Vector3>();
        for (int i = 0; i < moveInfor.Waypoint.Count; i++)
        {
            tmp.Add(moveInfor.Waypoint[i]);
        }

        Vector3[] wp = tmp.ToArray();

        transform.DOPath(wp, moveInfor.Duration, moveInfor.Type, PathMode.Sidescroller2D);
    } 
    
    public void MoveToTarget()
    {
        
    }


    public void Attack()
    {
        
    }

    public void SetInfor(int health)
    {
        _health = health;
    }

    #endregion

    #region Private Method

    private void OnDeadAnimation()
    {
        anim.SetBool(_deadState, true);
    }

    private void OnHitAnimation()
    {
        anim.SetBool(_hitState, true);
    }

    private void IdleAnimationFromHit()
    {
        anim.SetBool(_hitState, false);
    }

    private void OnDead()
    {
        this.PostEvent(EventID.BossDead, this.gameObject);
        Lean.LeanPool.Despawn(this);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }


    #endregion
}
