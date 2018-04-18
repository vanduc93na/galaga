using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : BaseBoss
{
    private const string NORMAL_ATTACK_METHOD = "NormalAttack";
    [SerializeField] private GameObject _lineRenderer;
    [SerializeField] private float _minFireRate;
    [SerializeField] private float _maxFireRate;
    [SerializeField] private Transform _gunTransform;
    [SerializeField] private Transform _bulletPossition;
    [SerializeField] private GameObject _bullet;
    [SerializeField]
    private int _numberOfBullet;
    private LineRenderer laserSpawn;
    private RaycastHit2D hit;
    private bool _isFire;
    public LayerMask LayerMask;

    // Use this for initialization

    // Update is called once per frame

    private void OnEnable()
    {
        if (gameObject.activeSelf)
        {
            float fireRate = Random.Range(_minFireRate, _maxFireRate);
            CancelInvoke(NORMAL_ATTACK_METHOD);
            Invoke(NORMAL_ATTACK_METHOD, fireRate);
        }
    }

    private void NormalAttack()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(AttackAnimation());
            for (int i = 1; i <= _numberOfBullet; i++)
            {
                var bullet = Lean.LeanPool.Spawn(_bullet, _bulletPossition.position, Quaternion.identity);
                bullet.transform.position = _bulletPossition.position;
                Vector3 tmp = bullet.transform.position;

                if (_numberOfBullet % 2 == 1)
                {
                    if (i <= _numberOfBullet / 2)
                    {
                        bullet.transform.Rotate(tmp.x, tmp.y, tmp.z - (_numberOfBullet / 2 - i + 1) * 10);
                    }
                    else if (i - 1 > _numberOfBullet / 2)
                    {
                        bullet.transform.Rotate(tmp.x, tmp.y, tmp.z + (i - _numberOfBullet / 2 - 1) * 10);
                    }
                    else
                    {
                        bullet.transform.Rotate(tmp.x, tmp.y, tmp.z);
                    }
                }
                else
                {
                    if (i <= _numberOfBullet / 2)
                    {
                        if (i == _numberOfBullet / 2)
                        {
                            bullet.transform.Rotate(tmp.x, tmp.y, tmp.z - 5);
                        }
                        else
                        {
                            bullet.transform.Rotate(tmp.x, tmp.y, tmp.z - (5 + (_numberOfBullet / 2 - i) * 10));
                        }
                    }
                    else
                    {
                        if (i == _numberOfBullet / 2 + 1)
                        {
                            bullet.transform.Rotate(tmp.x, tmp.y, tmp.z + 5);
                        }
                        else
                        {
                            bullet.transform.Rotate(tmp.x, tmp.y, tmp.z + (5 + (i - _numberOfBullet / 2 - 1) * 10));
                        }
                    }
                }

            }
            float fireRate = Random.Range(_minFireRate, _maxFireRate);
            Invoke(NORMAL_ATTACK_METHOD, fireRate);
        }
        /*
        var laser = Lean.LeanPool.Spawn(_lineRenderer, _gunTransform.position, Quaternion.identity);
        laser.transform.SetParent(_gunTransform);
        laser.transform.position = _gunTransform.transform.position;
        laser.GetComponent<BulletBoss2>().SetParentTransform(_gunTransform.position);
        float fireRate = Random.Range(_minFireRate, _maxFireRate);
        Invoke(NORMAL_ATTACK_METHOD, fireRate);
        */
    }

    IEnumerator TimeAttack(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _isFire = false;
        //        Lean.LeanPool.Despawn(laserSpawn);
    }

}
