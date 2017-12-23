using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseItem : MonoBehaviour
{
    private Rigidbody2D rg;
    // Use this for initialization
    void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rg.AddForce(new Vector2(Random.Range(-5f, 5f), 10));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleEvent.Instance.TriggerItemVsOther(this.gameObject, other.gameObject);
    }

    #region Public Method

    public void MoveToPlayer(Vector3 playerPos, Action callBack)
    {
        transform.DOMove(playerPos, 0.1f).OnComplete(() =>
        {
            callBack();
        });
    }

    #endregion
}
