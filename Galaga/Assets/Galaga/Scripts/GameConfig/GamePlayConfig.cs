using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayConfig : MonoBehaviour
{
    /// <summary>
    /// config vị trí mà phi thuyển có thể di chuyển trên màn hình
    /// </summary>
    [Tooltip("vị trí x lớn nhất phi thuyền có thể di chuyển đến")]
    [SerializeField] float maxDx;
    [Tooltip("vị trí y lớn nhất phi thuyền có thể di chuyển đến")]
    [SerializeField] float maxDy;
    [Tooltip("vị trí x nhỏ nhất phi thuyền có thể di chuyển đến")]
    [SerializeField] float minDx;
    [Tooltip("vị trí y nhỏ nhất phi thuyền có thể di chuyển đến")]
    [SerializeField] float minDy;

    public float MaxDx()
    {
        return maxDx;
    }

    public float MaxDy()
    {
        return maxDy;
    }

    public float MinDx()
    {
        return minDx;
    }

    public float MinDy()
    {
        return minDy;
    }
}
