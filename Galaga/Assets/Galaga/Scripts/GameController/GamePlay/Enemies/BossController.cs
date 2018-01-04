using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BossController : Singleton<BossController>
{
    [SerializeField] private GameObject _bossMgrGameObject;
    [SerializeField] private GameObject _pathBossMgrGameObject;
    private Dictionary<int, GameObject> _bossMgr;
    private List<MoveInformation> _listMove;

    void Awake()
    {
        _bossMgr = new Dictionary<int, GameObject>();
        for (int i = 0; i < _bossMgrGameObject.transform.childCount; i++)
        {
            _bossMgr.Add(i, _bossMgrGameObject.transform.GetChild(i).gameObject);
        }

        _listMove = new List<MoveInformation>();
        for (int i = 0; i < _pathBossMgrGameObject.transform.childCount; i++)
        {
            MoveInformation moveInfor = new MoveInformation();
            moveInfor.Waypoint = _pathBossMgrGameObject.transform.GetChild(i).GetComponent<DOTweenPath>().wps;
            moveInfor.Duration = _pathBossMgrGameObject.transform.GetChild(i).GetComponent<DOTweenPath>().duration;
            moveInfor.Type = _pathBossMgrGameObject.transform.GetChild(i).GetComponent<DOTweenPath>().pathType;
            moveInfor.Mode = _pathBossMgrGameObject.transform.GetChild(i).GetComponent<DOTweenPath>().pathMode;
            _listMove.Add(moveInfor);
        }
    }

    public void SpawnBoss(WaveBoss waveBoss)
    {
        for (int i = 0; i < waveBoss.BossInfors.Count; i++)
        {
            GameObject boss = Lean.LeanPool.Spawn(_bossMgr[waveBoss.BossInfors[i].IdBoss], transform.position, Quaternion.identity);
            boss.transform.SetParent(transform);
            boss.GetComponent<BaseBoss>().SetInforBoss(waveBoss.BossInfors[i]);
            boss.GetComponent<BaseBoss>().MoveToScreen(_listMove[1]);
            boss.GetComponent<BaseBoss>().SetWaveInfor(waveBoss);
            HandleEvent.Instance.AddBoss(boss);
        }
    }
}