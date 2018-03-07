using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private int dame;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
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
            HandleEvent.Instance.TriggerArrowVsEnemies(this.gameObject, other.gameObject);
        }
    }

    public int GetDame()
    {
        return dame;
    }
}
