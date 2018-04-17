using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testAPI : MonoBehaviour
{

    public GameObject image;

    private bool isShow = true;
	// Use this for initialization
	void Start ()
	{
	    isShow = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TestShowFull()
    {
        API.ShowFull(() =>
        {
            isShow = !isShow;
            if (isShow)
            {
                image.SetActive(true);
            }
            else
            {
                image.SetActive(false);
            }
        });
    }
}
