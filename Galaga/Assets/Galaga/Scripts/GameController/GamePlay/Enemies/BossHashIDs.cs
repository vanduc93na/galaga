using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHashIDs : MonoBehaviour
{
    public readonly int Dead = Animator.StringToHash("Dead");
    public readonly int Hit = Animator.StringToHash("Hit");

    void Awake()
    {
        
    }
}