using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{

    void Awake()
    {
        this.RegisterListener(EventID.Restart, (param) => ResetLevel());
        this.RegisterListener(EventID.NextLevel, (param) => ResetLevel());
    }

    void ResetLevel()
    {
        Lean.LeanPool.Despawn(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == GameTag.PLAYER || other.gameObject.tag == GameTag.BORDER)
        {
            Lean.LeanPool.Despawn(this);
        }
    }
}
