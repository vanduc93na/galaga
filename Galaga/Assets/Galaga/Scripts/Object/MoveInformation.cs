using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// class chứa thông tin cách di chuyển trên path
/// </summary>
public class MoveInformation
{
    /// <summary>
    /// list waypoint
    /// </summary>
    public List<Vector3> Waypoint = new List<Vector3>();
    // time duration
    public float Duration = 0;
    // type move
    public PathType Type = PathType.Linear;

    public PathMode Mode = PathMode.Sidescroller2D;
}
