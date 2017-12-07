using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// di chuyển background
/// </summary>
public class BackgroundMovement : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        transform.DOMoveY(-12.7f, 10f, false).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }
}
