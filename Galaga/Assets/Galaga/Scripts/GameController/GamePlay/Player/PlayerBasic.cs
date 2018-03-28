using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBasic : PlayerController
{
    [Tooltip("GameObject chứa các viên đạn")]
    [SerializeField]
    private GameObject _bulletsMgr;
    

    [SerializeField] private int _currentNumberBulletOnScreen;

    [SerializeField] private float _timeAttackTomahawk;

    [SerializeField] private float _timeAttackGenade;

    [SerializeField] private float _timeAttackLazer;

    [SerializeField] private float _timeAttackBlackHole;

    [SerializeField] private int _dameOfBlackHole;

    [SerializeField] private GameObject _sprite;

    [SerializeField] private Text _lifeText;

    [SerializeField] private GameObject _shield;

    [SerializeField] private GameObject _lazer;

    [SerializeField] private GameObject _ship;

    [SerializeField] private GameObject _explosiveEff;

    [SerializeField] private GameObject _countDownPage;

    [SerializeField] private GameObject _gameOverPage;
    
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
    private float _life;
    private bool _isProtected;

    void Awake()
    {
        RegisterEvent();
        _countDownPage.GetComponent<CountDownPage>().OnReturnPlay += ReturnPlay;
        _gameOverPage.GetComponent<GameLose>().OnReplay += ReturnPlay;
        this.RegisterListener(EventID.Restart, (param) => Init());
        this.RegisterListener(EventID.NextLevel, (param) => Init());
    }

    void Start()
    {
        Init();
    }

    void RegisterEvent()
    {
        this.RegisterListener(EventID.EatItem, (param) => EatItem((GameObject)param));
    }

    void ReturnPlay()
    {
        GameController.Instance.gameStage = GameStage.Play;
        _explosiveEff.SetActive(false);
        Invoke(FIRE_BULLET, fireRate);
        _isProtected = true;
        _shield.SetActive(true);
        _ship.SetActive(true);
        StartCoroutine(DeLayTime(5f, () =>
        {
            _isProtected = false;
            _shield.SetActive(false);
        }));
    }

    void EatItem(GameObject obj)
    {
        switch (obj.tag)
        {
            case GameTag.ITEM_ADD_BULLET:
                AddBullet();
                break;
            case GameTag.ITEM_ADD_HEART:
                AddHeart();
                break;
            case GameTag.ITEM_PLAYER_PROTECTED:
                StartCoroutine(PlayerProtected());
                break;
            case GameTag.ITEM_BULLET_3:
                
                break;
            case GameTag.ITEM_BLACK_HOLE:
                StartCoroutine(HandleEvent.Instance.BlackHoleAttack(_timeAttackBlackHole, _dameOfBlackHole));
                break;
            case GameTag.ITEM_SUPPER_GENADE:
                FireGenade();
                break;
            case GameTag.ITEM_SUPPER_TOMAHAWK:
                FireTomahawk();
                break;
            case GameTag.ITEM_LAZER:
                FireLazer();
                break;
        }
    }

    void PlayerDead()
    {
        if (_isProtected) return;
        if (_life == 0)
        {
            CancelInvoke(FIRE_BULLET);
            isMove = false;
            _explosiveEff.SetActive(true);
            StartCoroutine(DeLayTime(.23f, () =>
            {
                _ship.SetActive(false);
                this.PostEvent(EventID.GameOver);
            }));

        }
        else
        {
            _life -= 1;
        }
    }

    IEnumerator PlayerProtected()
    {
        _isProtected = true;
        _shield.SetActive(true);
        InventoryHelper.Instance.LoadInventory();
        switch (InventoryHelper.Instance.UserInventory.shipSelected)
        {
            case 1:
                yield return new WaitForSeconds(10f);
                break;
            case 2:
                yield return new WaitForSeconds(12f);
                break;
            case 3:
                yield return new WaitForSeconds(15f);
                break;
        }
        _isProtected = false;
        _shield.SetActive(false);
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
            SoundController.PlaySoundEffect(SoundController.Instance.PlayerFireBullet);
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
        GameController.Instance.gameStage = GameStage.Play;
        _isProtected = false;
        _bullets = new Dictionary<int, GameObject>();
        // init _bullets
        for (int i = 0; i < _bulletsMgr.transform.childCount; i++)
        {
            _bullets.Add(i, _bulletsMgr.transform.GetChild(i).gameObject);
        }
        _currentNumberBulletOnScreen = 1;
        gunObject = transform.GetChild(0).gameObject;
        _bullet = _bullets[1];
        InventoryHelper.Instance.LoadInventory();
        for (int i = 0; i < _sprite.transform.childCount; i++)
        {
            _sprite.transform.GetChild(i).gameObject.SetActive(false);
        }
        switch (InventoryHelper.Instance.UserInventory.shipSelected)
        {
            case 0:
                _life = 1;
                _sprite.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 1:
                _life = 2;
                _sprite.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case 2:
                _life = 3;
                _sprite.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
        _life += InventoryHelper.Instance.UserInventory.life;
        fireRate = _bullet.GetComponent<BasicBullet>().FireRate();
        _lifeText.text = _life.ToString();
        CancelInvoke(FIRE_BULLET);
        Invoke(FIRE_BULLET, fireRate);
        StartCoroutine(PlayerProtected());
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

    void AddHeart()
    {
        _life += 1;
        _lifeText.text = _life.ToString();
    }
    


    #region SUPER_ITEM

    void FireTomahawk()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_DRONE)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }
        StartCoroutine(DeLayTime(_timeAttackTomahawk, () =>
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag == GameTag.GUN_DRONE)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
        }));
    }

    void FireGenade()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_GENADE)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }
        StartCoroutine(DeLayTime(_timeAttackGenade, () =>
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag == GameTag.GUN_GENADE)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
        }));
    }

    void FireLazer()
    {
        _lazer.SetActive(true);
        StartCoroutine(DeLayTime(5f, () =>
        {
            _lazer.SetActive(false);
        }));
    }

    #endregion

    IEnumerator DeLayTime(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }
}