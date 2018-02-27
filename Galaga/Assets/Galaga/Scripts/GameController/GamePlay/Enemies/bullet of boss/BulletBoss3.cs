using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss3 : MonoBehaviour
{

    [SerializeField] private float _speed;

    private Vector3 _direction;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _speed);
        if (transform.position.x >= 10 || transform.position.x <= -10
            || transform.position.y >= 10 || transform.position.y <= -10)
        {
            Lean.LeanPool.Despawn(this);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
