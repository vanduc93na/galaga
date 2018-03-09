using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genade : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int dame;

    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        HandleEvent.Instance.TriggerGenadevsEnemies(this.gameObject, other.gameObject);

    }

    public int GetDame()
    {
        return dame;
    }
}
