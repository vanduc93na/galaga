using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss1 : MonoBehaviour
{
    [SerializeField] private float _speed;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        
    }
}
