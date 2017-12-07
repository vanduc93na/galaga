using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IHealth
{
    [Tooltip("Id of enemy")] [SerializeField] private int id;
    [Tooltip("Health")] [SerializeField] private int health;
    

    public void OnHit(int dame)
    {
        
    }

    void OnMove()
    {
        
    }

    void OnDead()
    {
        
    }

    void Attack()
    {
        
    }
}
