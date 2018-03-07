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
        if (other.tag == GameTag.BORDER)
        {
            Lean.LeanPool.Despawn(this.gameObject);
        }
        else if (other.tag == GameTag.ENEMY)
        {
            HandleEvent.Instance.TriggerGenadevsEnemies(this.gameObject);
        }
    }

    public int GetDame()
    {
        return dame;
    }
}
