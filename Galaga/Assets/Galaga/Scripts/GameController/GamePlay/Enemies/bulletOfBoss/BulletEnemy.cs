﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private float _baseSpeed;
    private float _speed;
    void Awake()
    {
        this.RegisterListener(EventID.Restart, (param) => ResetLevel());
        this.RegisterListener(EventID.NextLevel, (param) => ResetLevel());
        this.RegisterListener(EventID.GameOver, (param) => ResetLevel());
        this.RegisterListener(EventID.GameWin, (param) => ResetLevel());
        _speed = _baseSpeed;
    }

    void Update()
    {
        transform.position += transform.up * Time.deltaTime * _speed;
    }

    void ResetLevel()
    {
        if (gameObject.activeSelf)
        {
            Lean.LeanPool.Despawn(this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == GameTag.PLAYER || other.gameObject.tag == GameTag.BORDER)
        {
            if (gameObject.activeSelf)
            {
                Lean.LeanPool.Despawn(this);
            }
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetDefaultSpeed()
    {
        _speed = _baseSpeed;
    }
}
