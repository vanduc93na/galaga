using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    // biến số tính khoảng cách giữa 2 viên đạn - loại 1
    private const float DELTA_BULLET_1 = 0.5f;

    private const float DELTA_BULLET_2 = 0.1f;
    /// <summary>
    /// hàm tính vị trí viên đạn đầu tiên bên trái cùng
    /// </summary>
    /// <param name="numberBullet">tổng số viên đạn</param>
    /// <returns>vị trí viên đạn đầu tiên</returns>
    public static float StartPosXBullet1(int numberBullet)
    {
        float result = 0;
        if (numberBullet % 2 == 1)
        {
            return -1 * DELTA_BULLET_1 * ((numberBullet - 1) / 2);
        }
        else
        {
            return -1 * DELTA_BULLET_1 * ((float) ((numberBullet - 1) / 2));
        }
    }

    public static float StartPosXBullet2(int numberBullet)
    {
        float result;
        return -1 * DELTA_BULLET_2 * ((float)((numberBullet - 1) / 2));
        return result;
    }
    public static float DeltaBullet1()
    {
        return DELTA_BULLET_1;
    }

    public static float DeltaBullet2()
    {
        return DELTA_BULLET_2;
    }
}
