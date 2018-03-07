using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == GameTag.PLAYER || other.gameObject.tag == GameTag.BORDER)
        {
            Lean.LeanPool.Despawn(this);
        }
    }
}
