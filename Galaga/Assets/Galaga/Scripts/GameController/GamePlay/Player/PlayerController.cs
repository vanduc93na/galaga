using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    void OnMouseDown()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("OnMouseDown");
    }
}