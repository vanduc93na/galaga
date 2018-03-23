using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
//using NUnit.Framework.Internal.Builders;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public bool isMove = false;
    // game config
    [SerializeField]
    protected GamePlayConfig config;
    /// <summary>
    // đối tượng chứa 2 object con left và right để tính khoảng cách so với
    // phi thuyền -> hiệu ứng di chuyển camera khi phi thuyền di chuyển trái, phải
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (GameController.Instance.gameStage == GameStage.Play)
        {
            isMove = true;
        }
        else
        {
            isMove = false;
        }
    }
    #region Movement
    void Movement()
    {
        // khoảng thay đổi vị trí
        Vector3 deltaPos = Vector3.zero;
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            deltaPos = new Vector3(transform.position.x - point.x, transform.position.y - point.y);
            Collider2D coll = Physics2D.OverlapPoint(point);
            if (coll && coll.transform == transform)
            {
                isMove = true;
            }
        }
        if (Input.GetMouseButton(0) && isMove)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool isMoveLeft = (newPos.x - transform.position.x) > 0 ? false : true;
            newPos = new Vector3(deltaPos.x + newPos.x, deltaPos.y + newPos.y);
            // so sánh với các vị trí config
            newPos = new Vector3(newPos.x < config.MinDx() ? config.MinDx() : newPos.x > config.MaxDx() ? config.MaxDx() : newPos.x,
                newPos.y < config.MinDy() ? config.MinDy() : newPos.y > config.MaxDy() ? config.MaxDy() : newPos.y);
            Vector3 smoothSpeed = Vector3.Lerp(transform.position, newPos, config.GetSmoothSpeed());
            transform.position = smoothSpeed;
            // tạo hiệu ứng rung lắc
            //            CameraMoveEffect();
        };
    }

    void OnMouseDown()
    {
        isMove = true;
    }

    void OnMouseDrag()
    {
        Movement();
    }
    void OnMouseUp()
    {
        isMove = false;
    }

    //    void CameraMoveEffect()
    //    {
    //        Vector3 playerPos = transform.position;
    //        Vector3 playerPosWithZeroY = new Vector3(playerPos.x, 0, playerPos.z);
    //
    //        float deltaLeft = Vector3.Distance(playerPosWithZeroY, border.transform.GetChild(0).transform.position);
    //        float deltaRight = Vector3.Distance(playerPosWithZeroY, border.transform.GetChild(1).transform.position);
    //
    //        bool isMoveLeft = deltaLeft < deltaRight ? true : false;
    //        float deltaMoveX = isMoveLeft ? config.DeltaTransform() * (-1 * ((deltaLeft - deltaRight) / (deltaRight + deltaLeft))) : config.DeltaTransform() * ((deltaRight - deltaLeft) / (deltaRight + deltaLeft));
    //        Camera.main.transform.position = new Vector3(deltaMoveX, 0, -10);
    //    }

    #endregion

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != GameTag.ITEM)
        {
//            SoundController.PlaySoundEffect(SoundController.Instance.PlayerHitBullet);
        }
    }
}