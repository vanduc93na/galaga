using System.Collections;
using System.Collections.Generic;
using StartApp;
using UnityEngine;

public class StartAppManager : MonoBehaviour
{
    public bool IsShowingBanner;
    public void Init()
    {
#if UNITY_ANDROID
        StartAppWrapper.init();
        StartAppWrapper.loadAd();
        API.GaLogEvent("Startapp", "Init");
#endif
    }

    public void ShowSplash()
    {
#if UNITY_ANDROID
        StartAppWrapper.showSplash();
        API.GaLogEvent("Startapp", "Show flash");
#endif
    }

    public void ShowBanner()
    {
#if UNITY_ANDROID
        IsShowingBanner = true;
        StartAppWrapper.addBanner(StartAppWrapper.BannerType.AUTOMATIC, StartAppWrapper.BannerPosition.BOTTOM);
        API.Instance.TrackingShowAds(API.Banner);
#endif
    }

    public void HideBanner()
    {
#if UNITY_ANDROID
        IsShowingBanner = false;
        StartAppWrapper.removeBanner(StartAppWrapper.BannerPosition.BOTTOM);
#endif
    }

    public void ShowFull()
    {
#if UNITY_ANDROID
        StartAppWrapper.showAd();
        StartAppWrapper.loadAd();

        API.GaLogEvent("Startapp", "Show full");
        API.Instance.TrackingShowAds(API.Interstitial);
#endif
    }
}
