using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBasic : PlayerController
{

    public static PlayerBasic Instance;

    [Tooltip("GameObject chứa các viên đạn")]
    [SerializeField]
    private GameObject _bulletsMgr;


    [SerializeField] private int _currentNumberBulletOnScreen;

    [SerializeField] private float _timeAttackTomahawk;

    [SerializeField] private float _timeAttackGenade;

    [SerializeField] private float _timeAttackLazer;

    [SerializeField] private float _timeAttackBlackHole;
    [SerializeField] private float _timeAttackArrow;

    [SerializeField] private int _dameOfBlackHole;

    [SerializeField] private GameObject _sprite;

    [SerializeField] private Text _lifeText;

    [SerializeField] private GameObject _shield;

    [SerializeField] private GameObject _lazer;

    [SerializeField] private GameObject _arrow;

    [SerializeField] private GameObject _explosiveEff;

    [SerializeField] private GameObject _countDownPage;

    [SerializeField] private GameObject _gameOverPage;

    [SerializeField] private GameObject _gameWinPage;

    [SerializeField] private Transform _parentBullet;

    [SerializeField] private Material[] _playerMaterial;
    // private variables
    /// <summary>
    /// dict chứa các viên đạn lấy từ _bulletMgr được cache lại
    /// key là id của viên đạn
    /// </summary>
    private Dictionary<int, GameObject> _bullets;

    private const string FIRE_TOMAHAWK = "FireTomahawk";
    private const string FIRE_ARROW = "FireArrow";
    private const string FIRE_BULLET = "FireBullet";
    private const string FIRE_GENADE = "FireGenade";
    private const string FIRE_LAZER = "FireLazer";
    
    private GameObject _bullet;
    private float fireRate;
    private GameObject gunObject;
    private float _life;
    private bool isDead;
    [SerializeField] private bool _isProtected;
    private Vector3 _rootPos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        RegisterEvent();
        _countDownPage.GetComponent<CountDownPage>().OnReturnPlay += ReturnPlay;
        _gameOverPage.GetComponent<GameLose>().OnReplay += ReturnPlay;
        _gameWinPage.GetComponent<GameWin>().OnReplay += ReturnPlay;
        this.RegisterListener(EventID.Restart, (param) => Init());
        this.RegisterListener(EventID.NextLevel, (param) => Init());
        this.RegisterListener(EventID.GameWin, (param) => GameWin());
        _rootPos = transform.position;
    }

    void Start()
    {
        Init();
    }

    void RegisterEvent()
    {
        this.RegisterListener(EventID.EatItem, (param) => EatItem((GameObject)param));
    }

    public void ReturnPlay()
    {
//        ResetSuperItem();
        Init();
        isMove = true;
        GameController.Instance.gameStage = GameStage.Play;
        transform.position = _rootPos;
        Invoke(FIRE_BULLET, fireRate);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_DRONE
                || transform.GetChild(i).gameObject.tag == GameTag.GUN_GENADE)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        StartCoroutine(PlayerProtected(5));
    }

    void OnDisable()
    {
        
    }

    public void TestSuperItems(int test)
    {
        switch (test)
        {
            case 1:
                StopCoroutine(FIRE_TOMAHAWK);
                StartCoroutine(FireTomahawk());
                break;
            case 2:
                StopCoroutine(FIRE_ARROW);
                StartCoroutine(FireArrow());
                break;
            case 3:
                StopCoroutine(FIRE_GENADE);
                StartCoroutine(FireGenade());
                break;
            case 4:
                StartCoroutine(PlayerProtected(5));
                break;
            case 5:
                AddHeart();
                break;
            case 6:
                InventoryHelper.Instance.AddCoin(10000);
//                HandleEvent.Instance.BlackHoleAttack(_timeAttackBlackHole, _dameOfBlackHole);
                break;
            case 7:
                StopCoroutine(FIRE_LAZER);
                StartCoroutine(FireLazer());
                break;
        }
    }

    void EatItem(GameObject obj)
    {
        switch (obj.tag)
        {
            case GameTag.ITEM_ADD_BULLET:
                if (isDead) break;
                AddBullet();
                break;
            case GameTag.ITEM_ADD_HEART:
                if (isDead) break;
                AddHeart();
                break;
            case GameTag.ITEM_PLAYER_PROTECTED:
                if (_isProtected)
                {
                    StopCoroutine("PlayerProtected");
                }
                StartCoroutine(PlayerProtected(5));
                break;
            case GameTag.ITEM_ARROW:
                if (isDead) break;
                StopCoroutine(FIRE_ARROW);
                StartCoroutine(FireArrow());
                break;
            case GameTag.ITEM_BLACK_HOLE:
                if (isDead) break;
                HandleEvent.Instance.BlackHoleAttack(_timeAttackBlackHole, _dameOfBlackHole);
                break;
            case GameTag.ITEM_SUPPER_GENADE:
                if (isDead) break;
                StopCoroutine(FIRE_GENADE);
                StartCoroutine(FireGenade());
                break;
            case GameTag.ITEM_SUPPER_TOMAHAWK:
                if (isDead) break;
                StopCoroutine(FIRE_TOMAHAWK);
                StartCoroutine(FireTomahawk());
                break;
            case GameTag.ITEM_LAZER:
                if (isDead) break;
                SoundController.PlaySoundEffect(SoundController.Instance.ShootLazer);
                StopCoroutine(FIRE_LAZER);
                StartCoroutine(FireLazer());
                break;
        }
    }

    void GameWin()
    {
        transform.DOMoveY(7f, 1.5f).SetEase(Ease.InQuint).OnComplete(() =>
        {
//            transform.position = _rootPos;
        });
    }

    void PlayerDead()
    {
        if (_isProtected) return;
        if (_life == 0)
        {
            isDead = true;
            StopAllCoroutines();
            _isProtected = true;
            CancelInvoke(FIRE_BULLET);
            isMove = false;
            ResetSuperItem();
            _explosiveEff.SetActive(true);
            StartCoroutine(DeLayTime(.23f, () =>
            {
                this.PostEvent(EventID.PlayerDead);
                _sprite.SetActive(false); 
            }));
        }
        else
        {
            _life -= 1;
            _lifeText.text = _life.ToString();
            InventoryHelper.Instance.AddLife(-1);
            StartCoroutine(PlayerProtected(2));
        }
    }

    void RespawnEff()
    {
        InventoryHelper.Instance.LoadInventory();
        int idShip = InventoryHelper.Instance.UserInventory.shipSelected;
//        for (int i = 0; i < 6; i++)
//        {
//            _sprite.SetActive(false);
//            yield return new WaitForSeconds(0.1f);
//            _sprite.SetActive(true);
//            yield return new WaitForSeconds(0.1f);
//        }
//        print(idShip);
        _playerMaterial[idShip].DOColor(new Color(1f, 1f, 1f, 0.1f), 0.2f).SetLoops(6, LoopType.Yoyo).OnComplete(() =>
        {
            _playerMaterial[idShip].color = Color.white;
        });
}

    IEnumerator PlayerProtected(float second)
    {
//        StartCoroutine(RespawnEff());
        RespawnEff();
        _isProtected = true;
        _shield.SetActive(true);
        float seconds = second;
        InventoryHelper.Instance.LoadInventory();
        if (InventoryHelper.Instance.UserInventory.shipSelected == 0)
        {
            seconds += 0;
        }
        else if (InventoryHelper.Instance.UserInventory.shipSelected == 1)
        {
            seconds += 1;
        }
        else if (InventoryHelper.Instance.UserInventory.shipSelected == 2)
        {
            seconds += 3;
        }

        yield return new WaitForSeconds(seconds);
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
            bulletPool.transform.SetParent(_parentBullet);
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

    void ResetSuperItem()
    {
        _explosiveEff.SetActive(false);
        _shield.SetActive(false);
        _arrow.SetActive(false);
        _lazer.SetActive(false);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_DRONE)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_GENADE)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }

    void Init()
    {
        StopAllCoroutines();
        ResetSuperItem();
        transform.position = _rootPos;
        GameController.Instance.gameStage = GameStage.Play;
        _bullets = new Dictionary<int, GameObject>();
        isDead = false;
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
                skeletonAnimation = skeletons[0];
                _life = 1;
                _sprite.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 1:
                skeletonAnimation = skeletons[1];
                _life = 2;
                _sprite.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case 2:
                skeletonAnimation = skeletons[2];
                _life = 3;
                _sprite.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
        _sprite.SetActive(true);
        _shield.SetActive(true);
        _life += InventoryHelper.Instance.UserInventory.life;
        fireRate = _bullet.GetComponent<BasicBullet>().FireRate();
        _lifeText.text = _life.ToString();
        CancelInvoke(FIRE_BULLET);
        Invoke(FIRE_BULLET, fireRate);
        StartCoroutine(PlayerProtected(5));
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
        InventoryHelper.Instance.AddLife(1);
    }



    #region SUPER_ITEM

    IEnumerator FireTomahawk()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_DRONE)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }

        yield return new WaitForSeconds(_timeAttackTomahawk);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_DRONE)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }

    IEnumerator FireGenade()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_GENADE)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }

        yield return new WaitForSeconds(_timeAttackGenade);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == GameTag.GUN_GENADE)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }

    IEnumerator FireArrow()
    {
        _arrow.SetActive(true);
        yield return new WaitForSeconds(_timeAttackArrow);
        _arrow.SetActive(false);
    }

    IEnumerator FireLazer()
    {
        _lazer.SetActive(true);
//        SoundController.PlaySoundEffect(SoundController.Instance.LaserLoop);
        yield return new WaitForSeconds(5f);
        _lazer.SetActive(false);
    }

    #endregion

    IEnumerator DeLayTime(float seconds, Action callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }
}