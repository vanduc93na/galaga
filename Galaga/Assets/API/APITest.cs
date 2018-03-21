using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APITest : MonoBehaviour
{
    public Text TxtLog;
    public void HideBanner()
    {
        API.HideBanner();
    }
    //public void HideAdmobBanner()
    //{
    //    API.HideAdmobBanner();
    //}
    //public void HideStartappBanner()
    //{
    //    API.HideStartAppBanner();
    //}

    public void ShowBanner()
    {
        API.ShowBanner();
    }
    //public void ShowAdmobBanner()
    //{
    //    API.ShowAdmobBanner();
    //}
    //public void ShowStartappBanner()
    //{
    //    API.ShowStartAppBanner();
    //}
    public void ShowFull()
    {
        API.ShowFull(null);
    }
    //public void ShowAdmobFull()
    //{
    //    API.ShowAdmobFull();
    //}
    //public void ShowStartapFull()
    //{
    //    API.ShowStartappFull();
    //}
    public void ShowVideo()
    {
        API.ShowVideo(() =>
        {
            TxtLog.text = "Video completed!";
            TxtLog.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        });
    }

    public void IAP() {
        API.Instance.TrackingIAP(1.99f);
    }
    //public void ShowAdmobVideo()
    //{
    //    API.ShowAdmobVideo(() =>
    //    {
    //        TxtLog.text = "Admob video completed!";
    //        TxtLog.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    //    });
    //}
    //public void ShowUnityAdsVideo()
    //{
    //    API.ShowUnityAdsVideo(() =>
    //    {
    //        TxtLog.text = "UnityAds video completed!";
    //        TxtLog.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    //    });
    //}
}
