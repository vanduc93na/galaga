using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasic : PlayerController
{
    [Tooltip("GameObject chứa các viên đạn")]
    [SerializeField]
    private GameObject _bulletsMgr;

    [SerializeField] private int _currentNumberBulletOnScreen;
    
    // private variables
    /// <summary>
    /// dict chứa các viên đạn lấy từ _bulletMgr được cache lại
    /// key là id của viên đạn
    /// </summary>
    private Dictionary<int, GameObject> _bullets;
    
    private const string FIRE_BULLET = "FireBullet";
    private GameObject _bullet;
    private float fireRate;
    private GameObject gunObject;

    void Awake()
    {
        RegisterEvent();
    }

    void Start()
    {
        Init();
    }

    void RegisterEvent()
    {
        this.RegisterListener(EventID.EatItem, (param) => EatItem((GameObject) param));
    }

    void EatItem(GameObject obj)
    {
        switch (obj.tag)
        {
            case GameTag.ITEM_ADD_BULLET:
                AddBullet();
                break;
            case GameTag.ITEM_BULLET_1:
                ChangeBullet1();
                break;
            case GameTag.ITEM_BULLET_2:
                ChangeBullet2();
                break;
            case GameTag.ITEM_BULLET_3:
                ChangeBullet3();
                break;
        }
    }

    void PlayerDead()
    {
        print("Player Dead - GameOver");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == GameTag.ENEMY || other.gameObject.tag == GameTag.ENEMY_BULLET)
        {
            PlayerDead();
        }
    }

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
        float startPosX = Utilities.StartPosXBullet1(_currentNumberBulletOnScreen);

        for (int i = 1; i <= _currentNumberBulletOnScreen; i++)
        {
            GameObject bulletPool = Lean.LeanPool.Spawn(_bullet, gunObject.transform.position, Quaternion.identity);
            HandleEvent.Instance.AddBullet(bulletPool);
            bulletPool.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
            float dx = startPosX + (i - 1) * Utilities.DeltaBullet1();
            float angle = Vector2.Angle(new Vector2(dx, 10), Vector2.up);
            angle = i <= (int)(_currentNumberBulletOnScreen / 2) ? angle : -1 * angle;
            bulletPool.GetComponent<BasicBullet>().InitBullet1(angle);
        }
    }

    /// <summary>
    /// bắn loại đạn 2
    /// </summary>
    void FireBulletTwo()
    {
        float startPosX = Utilities.StartPosXBullet2(_currentNumberBulletOnScreen);

        for (int i = 1; i <= _currentNumberBulletOnScreen; i++)
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
        bulletPool.GetComponent<BasicBullet>().InitBullet3(_currentNumberBulletOnScreen);
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
        _currentNumberBulletOnScreen = 1;
        gunObject = transform.GetChild(0).gameObject;
        _bullet = _bullets[0];
        fireRate = _bullet.GetComponent<BasicBullet>().FireRate();
        Invoke(FIRE_BULLET, fireRate);
    }
    #endregion


    void AddBullet()
    {
        _currentNumberBulletOnScreen++;
        if (_currentNumberBulletOnScreen > config.GetMaxBullet())
        {
            _currentNumberBulletOnScreen = config.GetMaxBullet();
        }
    }

    void ChangeBullet1()
    {
        _bullet = _bullets[0];
    }

    void ChangeBullet2()
    {
        _bullet = _bullets[1];
    }

    void ChangeBullet3()
    {
        _bullet = _bullets[2];
    }
}
