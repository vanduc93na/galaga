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
        transform.Translate(_direction * Time.deltaTime * _speed);
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
