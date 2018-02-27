using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class kế thừa đối tượng boss
/// dùng để tạo ra cách tấn công riêng cho từng boss
/// </summary>
public class Boss1 : BaseBoss
{
    private const string NORMAL_ATTACK_METHOD = "NormalAttack";

    [SerializeField]
    private int _numberOfBullet;
    [SerializeField] private Transform _bulletPossition;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _minFireRate;
    [SerializeField] private float _maxFireRate;

    void OnEnable()
    {
        base.OnEnable();
        CancelInvoke(NORMAL_ATTACK_METHOD);
        if (gameObject.activeSelf)
        {
            float fireRate = Random.Range(_minFireRate, _maxFireRate);
            Invoke(NORMAL_ATTACK_METHOD, fireRate);
        }
    }

    /// <summary>
    /// tấn công bằng cách sinh đạn
    /// </summary>
    void NormalAttack()
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
        
    }

}
