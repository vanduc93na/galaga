using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomahawk : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private int dame;

    private Transform _targetTransform;
    private BaseEnemy _targetEnemyScript;
    private Vector3 _direction;

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
        if (transform.position.y >= 5 || transform.position.y <= -5 || transform.position.x > 3 ||
            transform.position.x <= -3)
        {
            if (gameObject.activeSelf)
            {
                Lean.LeanPool.Despawn(gameObject);
            }
        }
        if (_targetEnemyScript == null || !_targetEnemyScript.IsAlive() || !_targetEnemyScript.IsActiveOnScene())
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            transform.eulerAngles = Vector3.zero;
            FindTarget();
        }
        else
        {
            _direction = _targetTransform.position - transform.position;
            _direction.Normalize();
            transform.position += _direction * speed * Time.deltaTime;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    void OnEnable()
    {

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
