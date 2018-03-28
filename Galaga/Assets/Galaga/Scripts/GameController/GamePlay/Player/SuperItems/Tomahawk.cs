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
        this.RegisterListener(EventID.Restart, (param) =>
        {
            if (gameObject.activeSelf)
            {
                Lean.LeanPool.Despawn(this.gameObject);
            }
        });
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
