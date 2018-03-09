using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayConfig : MonoBehaviour
{
    [Tooltip("thời gian chờ đợi giữa 2 wave")]
    [SerializeField]
    private float timeDelayBetweenWave = 0;
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
    [Tooltip("Smooth Speed của phi thuyền")] [SerializeField] private float smoothSpeed;
    [Tooltip("khoảng cách hiệu ứng di chuyển camera khi phi thuyền lượn sang trái - phải")]
    [SerializeField] private float deltaTransform;
    /// <summary>
    /// config đạn
    /// </summary>
    [Tooltip("Loại đạn cơ bản")]
    [SerializeField] private int basicBulletIndex;
    [Tooltip("Số lượng đạn nhỏ nhất")]
    [SerializeField]
    private int minBullet;
    [Tooltip("Số lượng đạn lớn nhất có thể bắn ra")]
    [SerializeField]
    private int maxBullets;
    

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

    public float DeltaTransform()
    {
        return deltaTransform;
    }

    public int BasicBulletIndex()
    {
        return basicBulletIndex;
    }

    public int GetMinBullet()
    {
        return minBullet;
    }

    public int GetMaxBullet()
    {
        return maxBullets;
    }

    public float GetTimeDelayBetweenWaves()
    {
        return timeDelayBetweenWave;
    }

    public float GetSmoothSpeed()
    {
        return smoothSpeed;
    }
}
