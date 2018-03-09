using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSuperItem : MonoBehaviour
{

    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bullet;
    [SerializeField] private SuperItems _superItem;

    private const string FIRE_METHOD = "Fire";

    void OnEnable()
    {
        Fire();
    }

    void OnDisable()
    {
        CancelInvoke(FIRE_METHOD);
    }

    void Fire()
    {
        var bulletSpawn = Lean.LeanPool.Spawn(bullet);
        bulletSpawn.transform.position = transform.position;
        Invoke(FIRE_METHOD, fireRate);
        switch (_superItem)
        {
            case SuperItems.Tomahawk:
                HandleEvent.Instance.AddTomahawk(bulletSpawn);
                break;
            case SuperItems.Arrow:
                HandleEvent.Instance.AddArrow(bulletSpawn);
                break;
            case SuperItems.Genade:
                HandleEvent.Instance.AddGenade(bulletSpawn);
                break;
        }
    }
}

enum SuperItems
{
    Tomahawk,
    Arrow,
    Genade
}
