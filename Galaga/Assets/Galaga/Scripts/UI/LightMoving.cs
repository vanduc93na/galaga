using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightMoving : MonoBehaviour
{
    private Vector3[] _path;

    public void Init(Vector3[] path)
    {
        _path = path;
        transform.position = _path[0];
    }

    public void DoMove()
    {
        
        transform.DOLocalPath(_path, 1f, PathType.CatmullRom, PathMode.Sidescroller2D).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete((() =>
        {
            Lean.LeanPool.Despawn(this.gameObject);
        }));
    }
}
