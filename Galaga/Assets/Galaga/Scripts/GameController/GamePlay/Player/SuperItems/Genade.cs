using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genade : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int dame;
    [SerializeField] private GameObject explosionGenade;

    void Awake()
    {
        this.RegisterListener(EventID.Restart, (param) => ResetItem());
        this.RegisterListener(EventID.GameWin, (param) => ResetItem());
        this.RegisterListener(EventID.GameOver, (param) => ResetItem());
    }

    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * speed;
    }

    void ResetItem()
    {
        if (gameObject.activeSelf)
        {
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleEvent.Instance.TriggerGenadevsEnemies(this.gameObject, other.gameObject, explosionGenade);
    }

    public int GetDame()
    {
        return dame;
    }
}
