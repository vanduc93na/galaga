using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Singleton<BossController>
{
    [SerializeField] private GameObject BossMgrGameObject;
    private Dictionary<int, GameObject> _bossMgr;

    void Awake()
    {
        _bossMgr = new Dictionary<int, GameObject>();
        for (int i = 0; i < BossMgrGameObject.transform.childCount; i++)
        {
            _bossMgr.Add(i, BossMgrGameObject.transform.GetChild(i).gameObject);
        }
    }

    public void SpawnBoss(WaveBoss waveBoss)
    {
        
    }
}
