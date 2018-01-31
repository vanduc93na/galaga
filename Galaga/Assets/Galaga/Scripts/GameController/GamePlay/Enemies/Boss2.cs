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
//        _isFire = true;
//        StartCoroutine(TimeAttack(2f));
        var laser = Lean.LeanPool.Spawn(_lineRenderer, _gunTransform.position, Quaternion.identity);
        laser.transform.SetParent(_gunTransform);
        laser.transform.position = _gunTransform.transform.position;
        laser.GetComponent<BulletBoss2>().SetParentTransform(_gunTransform.position);
        float fireRate = Random.Range(_minFireRate, _maxFireRate);
        Invoke(NORMAL_ATTACK_METHOD, fireRate);
    }

    IEnumerator TimeAttack(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _isFire = false;
        //        Lean.LeanPool.Despawn(laserSpawn);
    }

}
