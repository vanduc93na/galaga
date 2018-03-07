using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss2 : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private LineRenderer _lineRenderer;
    private Vector3 _startTransform;
    private RaycastHit2D hit;
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
        hit = Physics2D.Raycast(transform.position, Vector2.down, 100, _layerMask);
        _lineRenderer.enabled = true;
        //Debug.Log(hit.collider.gameObject.name);
        _lineRenderer.SetPosition(0, transform.position);
        if (hit.collider != null && (hit.collider.gameObject.tag == GameTag.PLAYER || hit.collider.gameObject.tag == GameTag.BORDER))
        {
           _lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            _lineRenderer.SetPosition(1, transform.position + Vector3.down * 100);
        }

    }

    public void SetParentTransform(Vector3 parentTrans)
    {
        _startTransform = parentTrans;
    }
}
