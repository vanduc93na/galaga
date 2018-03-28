using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
//using NUnit.Framework.Internal.Builders;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public bool isMove = false;

    public ControlType ControlType;
    // game config
    [SerializeField]
    protected GamePlayConfig config;

    private Vector3 _beginPos;
    private Vector3 _deltaPos;
    private Vector3 _tempPos;
    private Vector3 _onMovePos;
    private float _lerpSpeed;
    private int _eventIndex = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            _eventIndex = 0;
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_ANDROID
        MobileMove();
#endif
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

#if UNITY_ANDROID
    void MobileMove()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(_eventIndex)) return;
                StartMove(Camera.main.ScreenToWorldPoint(touch.position));
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (EventSystem.current.IsPointerOverGameObject(_eventIndex)) return;
                OnMove(Camera.main.ScreenToWorldPoint(touch.position));
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (EventSystem.current.IsPointerOverGameObject(_eventIndex)) return;
                EndMove(Camera.main.ScreenToWorldPoint(touch.position));
            }
        }
    }
#endif
#if UNITY_EDITOR

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

    //        void CameraMoveEffect()
    //        {
    //            Vector3 playerPos = transform.position;
    //            Vector3 playerPosWithZeroY = new Vector3(playerPos.x, 0, playerPos.z);
    //    
    //            float deltaLeft = Vector3.Distance(playerPosWithZeroY, border.transform.GetChild(0).transform.position);
    //            float deltaRight = Vector3.Distance(playerPosWithZeroY, border.transform.GetChild(1).transform.position);
    //    
    //            bool isMoveLeft = deltaLeft < deltaRight ? true : false;
    //            float deltaMoveX = isMoveLeft ? config.DeltaTransform() * (-1 * ((deltaLeft - deltaRight) / (deltaRight + deltaLeft))) : config.DeltaTransform() * ((deltaRight - deltaLeft) / (deltaRight + deltaLeft));
    //            Camera.main.transform.position = new Vector3(deltaMoveX, 0, -10);
    //        }
#endif
    #endregion


    void StartMove(Vector3 mousePos)
    {
        isMove = true;
        _deltaPos = transform.position - mousePos;
        _lerpSpeed = 4;
    }

    void OnMove(Vector3 mousePos)
    {
        if (ControlType == ControlType.ControlMode)
            _deltaPos = new Vector2(0, .5f);
        _tempPos = mousePos + _deltaPos;
        _tempPos = new Vector3(_tempPos.x < config.MinDx() ? config.MinDx() : _tempPos.x > config.MaxDx() ? config.MaxDx() : _tempPos.x,
            _tempPos.y < config.MinDy() ? config.MinDy() : _tempPos.y > config.MaxDy() ? config.MaxDy() : _tempPos.y);
        if (_lerpSpeed < config.GetLerpLimit())
        {
            _lerpSpeed += 4 * Time.deltaTime;
        }
        _lerpSpeed = Mathf.Clamp(_lerpSpeed, 4, 15);
        transform.position = Vector2.Lerp(transform.position, _tempPos, _lerpSpeed * Time.deltaTime);
    }

    void EndMove(Vector3 mousePos)
    {
        _beginPos = mousePos;
        _deltaPos = mousePos;
        isMove = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != GameTag.ITEM)
        {
            //            SoundController.PlaySoundEffect(SoundController.Instance.PlayerHitBullet);
        }
    }
}

public enum ControlType
{
    ControlMode,
    Relative
}