using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    [Tooltip("Tag name")] [SerializeField] private string tagName;
    [Tooltip("Fire Rate")] [SerializeField] private float fireRate;
    [Tooltip("Dame")] [SerializeField] private int dame;
    [Tooltip("Speed")] [SerializeField] private float speed;

    private float scaleDelta = 0.05f;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    public void InitBullet1(float rotationZ)
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
    }

    public void InitBullet2(Vector3 newPos)
    {
        transform.localPosition = newPos;
    }

    public void InitBullet3(int numberBullet)
    {
        Vector3 scale = transform.localScale;
        scale.x += numberBullet * scaleDelta;
        scale.y += (float)(numberBullet * scaleDelta) / 2;
        transform.localScale = scale;
    }

    public String TagName()
    {
        return tagName;
    }

    public float FireRate()
    {
        return fireRate;
    }

    public int Dame()
    {
        return dame;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Nó bi do cái này này
        // Đây layder dèault va chậm tất cả
        // nên khi viên đạn này gặp cáilaseser kia ở dèault là nó hủy cái laser kia
        HandleEvent.Instance.TriggerBulletVsOther(gameObject, other.gameObject);
    }
}
