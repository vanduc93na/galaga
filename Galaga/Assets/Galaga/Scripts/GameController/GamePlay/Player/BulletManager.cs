using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [Tooltip("Danh sách đạn")]
    [SerializeField] private GameObject[] bullets;

    [Tooltip("Game config dùng để lấy viên đạn mặc định khởi tạo trong trường hợp không tìm thấy viên đạn")]
    [SerializeField] private GamePlayConfig config;
    #region Public Method

    /// <summary>
    /// lấy viên đạn trong danh sách đạn được quản lý
    /// nếu không có viên đạn nào giống tag name thì sẽ lấy viên đạn cơ bản khi được khởi tạo
    /// </summary>
    /// <param name="bulletTag">tag name của bullet</param>
    /// <returns></returns>
    public GameObject GetBullet(string bulletTag)
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i].tag == bulletTag)
            {
                return bullets[i];
            }
        }
        // nếu không tìm thấy viên đạn bằng tag
        Debug.Log("can't find bullet with tag:" + bulletTag);
        Debug.Log("default is will be return basic bullet with index object in GamePlayConfig:" + config.BasicBulletIndex());
        return bullets[config.BasicBulletIndex()];
    }
    #endregion
}
