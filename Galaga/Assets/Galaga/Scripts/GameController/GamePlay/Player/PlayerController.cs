using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : Singleton<PlayerController>
{
    [Tooltip("GameObject chứa các viên đạn")]
    [SerializeField] private GameObject _bulletsMgr;
    // game config
    [SerializeField] private GamePlayConfig config;

    /// <summary>
    // đối tượng chứa 2 object con left và right để tính khoảng cách so với
    // phi thuyền -> hiệu ứng di chuyển camera khi phi thuyền di chuyển trái, phải
    /// </summary>
    [SerializeField] private GameObject border;
    // private variables
    /// <summary>
    /// dict chứa các viên đạn lấy từ _bulletMgr được cache lại
    /// key là id của viên đạn
    /// </summary>
    private Dictionary<int, GameObject> _bullets;
    private bool isMove;
    private const string FIRE_BULLET = "FireBullet";
    private GameObject _bullet;
    private int currentNumberBulletOnScreen;
    private float fireRate;
    private GameObject gunObject;

    private void Start()
    {
        Init();
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
            transform.position = newPos;
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

    void CameraMoveEffect()
    {
        Vector3 playerPos = transform.position;
        Vector3 playerPosWithZeroY = new Vector3(playerPos.x, 0, playerPos.z);

        float deltaLeft = Vector3.Distance(playerPosWithZeroY, border.transform.GetChild(0).transform.position);
        float deltaRight = Vector3.Distance(playerPosWithZeroY, border.transform.GetChild(1).transform.position);

        bool isMoveLeft = deltaLeft < deltaRight ? true : false;
        float deltaMoveX = isMoveLeft ? config.DeltaTransform() * (-1 * ((deltaLeft - deltaRight) / (deltaRight + deltaLeft))) : config.DeltaTransform() * ((deltaRight - deltaLeft) / (deltaRight + deltaLeft));
        Camera.main.transform.position = new Vector3(deltaMoveX, 0, -10);
    }

    #endregion

    #region Attack

    void OnEnable()
    {
    }

    void FireBullet()
    {
        switch (_bullet.tag)
        {
            case GameTag.BULLET_1:
                FireBulletOne();
                break;
            case GameTag.BULLET_2:
                FireBulletTwo();
                break;
            case GameTag.BULLET_3:
                FireBulletThree();
                break;
            case GameTag.BULLET_4:
                FireBulletFour();
                break;
            default:
                break;
        }
        Invoke(FIRE_BULLET, fireRate);
    }

    /// <summary>
    /// hàm bắn loại đạn 1 
    /// </summary>
    void FireBulletOne()
    {
        float startPosX = Utilities.StartPosXBullet1(currentNumberBulletOnScreen);

        for (int i = 1; i <= currentNumberBulletOnScreen; i++)
        {
            GameObject bulletPool = Lean.LeanPool.Spawn(_bullet, gunObject.transform.position, Quaternion.identity);
            HandleEvent.Instance.AddBullet(bulletPool);
            bulletPool.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
            float dx = startPosX + (i - 1) * Utilities.DeltaBullet1();
            float angle = Vector2.Angle(new Vector2(dx, 10), Vector2.up);
            angle = i <= (int)(currentNumberBulletOnScreen / 2) ? angle : -1 * angle;
            bulletPool.GetComponent<BasicBullet>().InitBullet1(angle);
        }
    }

    /// <summary>
    /// bắn loại đạn 2
    /// </summary>
    void FireBulletTwo()
    {
        float startPosX = Utilities.StartPosXBullet2(currentNumberBulletOnScreen);
        
        for (int i = 1; i <= currentNumberBulletOnScreen; i++)
        {
            GameObject bulletPool = Lean.LeanPool.Spawn(_bullet, gunObject.transform.position, Quaternion.identity);
            HandleEvent.Instance.AddBullet(bulletPool);
            bulletPool.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 newTransformBullet = bulletPool.transform.position;
            newTransformBullet.x = newTransformBullet.x + startPosX + (i - 1) * Utilities.DeltaBullet2();
            bulletPool.GetComponent<BasicBullet>().InitBullet2(newTransformBullet);
        }
    }

    /// <summary>
    /// bắn loại đạn 3
    /// </summary>
    void FireBulletThree()
    {
        GameObject bulletPool = Lean.LeanPool.Spawn(_bullet, gunObject.transform.position, Quaternion.identity);
        HandleEvent.Instance.AddBullet(bulletPool);
        bulletPool.transform.localScale = Vector3.one;
        bulletPool.GetComponent<BasicBullet>().InitBullet3(currentNumberBulletOnScreen);
    }

    /// <summary>
    /// bắn loại đạn 4
    /// </summary>
    void FireBulletFour()
    {

    }
    #endregion

    #region UtilitiesGameTool Method
    
    void Init()
    {
        _bullets = new Dictionary<int, GameObject>();
        // init _bullets
        for (int i = 0; i < _bulletsMgr.transform.childCount; i++)
        {
            _bullets.Add(i, _bulletsMgr.transform.GetChild(i).gameObject);
        }
        currentNumberBulletOnScreen = 1;
        gunObject = transform.GetChild(0).gameObject;
        _bullet = _bullets[1];
        fireRate = _bullet.GetComponent<BasicBullet>().FireRate();
        currentNumberBulletOnScreen = config.GetMinBullet();
        Invoke(FIRE_BULLET, fireRate);
        isMove = false;
    }
    #endregion
}