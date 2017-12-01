using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    // config
    [SerializeField] private GamePlayConfig config;

    // private variables
    private bool isMove;
    private void Start()
    {
        isMove = false;
    }

    // Update is called once per frame
    private void Update()
    {
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
            print(deltaPos.x);
            if (deltaPos.x > 0)
            {
                print("rotate left");
            }
            else if (deltaPos.x < 0)
            {
                print("rotate right");
            }
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
            string s = isMoveLeft ? "left" : "right";
            print(s);
            newPos = new Vector3(deltaPos.x + newPos.x, deltaPos.y + newPos.y);
            // so sánh với các vị trí config
            newPos = new Vector3(newPos.x < config.MinDx() ? config.MinDx() : newPos.x > config.MaxDx() ? config.MaxDx() : newPos.x, 
                newPos.y < config.MinDy() ? config.MinDy() : newPos.y > config.MaxDy() ? config.MaxDy() : newPos.y);
            transform.position = newPos;
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
    #endregion
}