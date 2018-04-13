using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// di chuyển background
/// </summary>
public class BackgroundMovement : MonoBehaviour
{
    public float scrollSpeed;
    private float _scrollSpeed;
    private Vector2 savedOffset;
    private Renderer rend;

    void Awake()
    {
        this.RegisterListener(EventID.GameWin, (param) => StartCoroutine(GameWin()));
    }

    IEnumerator GameWin()
    {
        _scrollSpeed = 0.2f;
        yield return new WaitForSeconds(2f);
        _scrollSpeed = scrollSpeed;
    }

    void Start()
    {
        _scrollSpeed = scrollSpeed;
        rend = gameObject.GetComponent<Renderer>();
        savedOffset = rend.sharedMaterial.GetTextureOffset("_MainTex");
    }
    void Update()
    {
        float y = Mathf.Repeat(Time.time * _scrollSpeed, 1);
        Vector2 offset = new Vector2(savedOffset.x, y);
        rend.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
    void OnDisable()
    {
        rend.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}
