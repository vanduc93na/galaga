using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private int _dame;
    [SerializeField] private float _fireRate;
    private LineRenderer _lineRenderer;
    private Vector3 _startTransform;
    private RaycastHit2D hit;

    private float _time = 0;
    // Use this for initialization
    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _lineRenderer.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.up, 100, _layerMask);
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, transform.position);
        if (hit.collider != null && (hit.collider.gameObject.tag == GameTag.ENEMY || 
            hit.collider.gameObject.tag == GameTag.BORDER ||
            hit.collider.gameObject.tag == GameTag.BOSS))
        {
            _lineRenderer.SetPosition(1, hit.point);
            _time += Time.deltaTime;
            if (_time > _fireRate)
            {
                HandleEvent.Instance.TriggerLazerVsOther(hit.collider.gameObject, _dame);
                _time = 0;
            }
        }
        else
        {
            _lineRenderer.SetPosition(1, transform.position + Vector3.up * 100);
        }

    }

    public void SetParentTransform(Vector3 parentTrans)
    {
        _startTransform = parentTrans;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print(other.name);
    }
}
