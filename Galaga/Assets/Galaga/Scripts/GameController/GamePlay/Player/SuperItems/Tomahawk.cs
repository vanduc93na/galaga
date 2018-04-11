using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Tomahawk : MonoBehaviour
{

    [SerializeField] private float maxSpeed;
    [SerializeField] private int dame;

    private Transform _targetTransform;
    private BaseEnemy _targetEnemyScript;
    private Vector3 _direction;
    private float speed;
    void Awake()
    {
        this.RegisterListener(EventID.Restart, (param) => ResetItem());
        this.RegisterListener(EventID.PlayerDead, (param) => ResetItem());
        this.RegisterListener(EventID.GameWin, (param) => ResetItem());
        this.RegisterListener(EventID.GameOver, (param) => ResetItem());
    }

    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        speed = 1.5f;
        _direction = Vector3.up;
    }

    void ResetItem()
    {
        if (gameObject.activeSelf)
        {
            Lean.LeanPool.Despawn(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (speed >= 1.5f && speed < 1.8f)
        {
            speed += Time.deltaTime;
        }
        else if (speed >= 1.8f && speed < 2.2f)
        {
            speed += Time.deltaTime * 15;
        }
        else
        {
            speed = maxSpeed;
        }
        transform.position += transform.up * speed * Time.deltaTime;
        if (transform.position.y >= 5 || transform.position.y <= -5 || transform.position.x > 3 ||
            transform.position.x <= -3)
        {
            if (gameObject.activeSelf)
            {
                HandleEvent.Instance.RemoveTomahawk(gameObject);
            }
        }
        if (_targetEnemyScript == null || !_targetEnemyScript.IsAlive() || !_targetEnemyScript.IsActiveOnScene())
        {
            FindTarget();
        }
        else
        {
            _direction = _targetTransform.position - transform.position;
            _direction.Normalize();
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
//            float angle = Vector2.Angle(transform.position, _targetTransform.position) + 90;
//            print(angle);
            transform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, 10));
        }
    }
    
    void FindTarget()
    {
        List<BaseEnemy> baseEnemies = HandleEvent.Instance.GetAllEnemiesAlive();
        if (baseEnemies.Count < 1) return;
        int random = Random.Range(0, baseEnemies.Count);
        _targetEnemyScript = baseEnemies[random];
        _targetTransform = _targetEnemyScript.transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleEvent.Instance.TriggerTomahawkVsEnemies(this.gameObject, other.gameObject);
    }

    public int GetDame()
    {
        return dame;
    }
}
