using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObject : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        this.RegisterListener(EventID.Restart, (param) => ResetAllChildren());
        this.RegisterListener(EventID.NextLevel, (param) => ResetAllChildren());
    }

    void ResetAllChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Lean.LeanPool.Despawn(transform.GetChild(i).gameObject);
        }
    }
}
