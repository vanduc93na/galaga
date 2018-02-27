using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boss3 : BaseBoss
{
    private const string NORMAL_ATTACK_METHOD = "NormalAttack";
    [SerializeField] private int _numberBullet;
    [SerializeField] private float _minFireRate;
    [SerializeField] private float _maxFireRate;
    [SerializeField] private GameObject _gun;
    [SerializeField] private GameObject _bulletBoss3;
    private int _angleRotate;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        _angleRotate = 0;
        if (gameObject.activeSelf)
        {
            float fireRate = Random.Range(_minFireRate, _maxFireRate);
            CancelInvoke(NORMAL_ATTACK_METHOD);
            Invoke(NORMAL_ATTACK_METHOD, fireRate);
        }
    }

    private void NormalAttack()
    {
        float random = Random.Range(0f, 1f);
        float angle = (float) 360 / _numberBullet;

        for (int i = 0; i < _numberBullet; i++)
        {
            var bullet = Lean.LeanPool.Spawn(_bulletBoss3, _gun.transform.position, Quaternion.identity);
            bullet.transform.Rotate(new Vector3(transform.position.x, transform.position.y, angle * i));
        }

//        int tmp = random < 0.5f ? 1 : -1;
//        _angleRotate = (_angleRotate + 90 * tmp) % 360;
//        for (int i = 0; i < _gun.Length; i++)
//        {
//            Lean.LeanPool.Spawn(_bulletBoss3, _gun[i].transform.position, Quaternion.identity);
//        }
//        _body.transform.DORotate(new Vector3(transform.position.x, transform.position.y, _angleRotate), 1f, RotateMode.Fast).OnComplete(
//            () =>
//            {
//                for (int i = 0; i < _gun.Length; i++)
//                {
//                    Lean.LeanPool.Spawn(_bulletBoss3, _gun[i].transform.position, Quaternion.identity);
//                }
//            });

        float fireRate = Random.Range(_minFireRate, _maxFireRate);
        Invoke(NORMAL_ATTACK_METHOD, fireRate);
    }
}
