using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSuperItem : MonoBehaviour
{

    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bullet;
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
    }
}
