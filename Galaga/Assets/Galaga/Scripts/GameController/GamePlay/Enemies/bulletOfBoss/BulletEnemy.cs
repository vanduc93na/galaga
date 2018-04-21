using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private float _baseSpeed;
    [SerializeField] private TypeOfBulletEnemy Type;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _explodedSpeed;
    private Vector3 targetPos;
    private float angle;

    private bool isMoveToCircleComplete;
    private float radius;
    private float _speed;
    void Awake()
    {
        this.RegisterListener(EventID.Restart, (param) => ResetLevel());
        this.RegisterListener(EventID.NextLevel, (param) => ResetLevel());
        this.RegisterListener(EventID.GameOver, (param) => ResetLevel());
        this.RegisterListener(EventID.GameWin, (param) => ResetLevel());
        _speed = _baseSpeed;
    }

    void Update()
    {

        switch (Type)
        {
            case TypeOfBulletEnemy.Default:
                transform.position += transform.up * Time.deltaTime * _speed;
                break;
            case TypeOfBulletEnemy.Bom:
                if (targetPos != Vector3.zero)
                {
                    if (IsMoveToCircle())
                    {
                        transform.position += transform.up * Time.deltaTime * _speed;
                    }
                    else
                    {
                        // quay vong
//                        angle += _rotateSpeed * Time.deltaTime;
//                        var offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
//                        transform.position = targetPos + offset;
                    }
                }
                break;
            case TypeOfBulletEnemy.Exploded:
                transform.position += transform.up * Time.deltaTime * _explodedSpeed;
                break;
        }
    }

    bool IsMoveToCircle()
    {
        
        bool result = Vector2.Distance(transform.position, targetPos) > radius + 1;
//        if (!result)
//        {
//            print("aaa");
//            angle = Vector2.Angle(targetPos.normalized, transform.position.normalized) - 90;
//        }
        return result;
    }

    void OnEnable()
    {
        targetPos = Vector3.zero;
    }


    void ResetLevel()
    {
        if (gameObject.activeSelf)
        {
//            Lean.LeanPool.Despawn(this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == GameTag.PLAYER || other.gameObject.tag == GameTag.BORDER)
        {
            if (gameObject.activeSelf)
            {
                Lean.LeanPool.Despawn(this);
            }
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetDefaultSpeed()
    {
        _speed = _baseSpeed;
    }

    public void SetTargetPos(Vector3 targetPos, float radius)
    {
        this.radius = radius;
        this.targetPos = targetPos;
    }

    public void OnMoveToCircleComplete(bool iscomplete)
    {
        isMoveToCircleComplete = iscomplete;
    }
}

public enum TypeOfBulletEnemy
{
    Default,
    Bom,
    Exploded
}