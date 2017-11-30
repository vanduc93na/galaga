using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GoogleMobileAds.Api;
using Berry.Utils;

public class AdAssistant : SingletonMonoBehaviour<AdAssistant>
{
    // Settings
    // AdMob
    public string AdMob_Interstitial_Android = "ca-app-pub-9234065232619132/9268449205";

    public string AdMob_Interstitial_iOS = "ca-app-pub-9234065232619132/4838249604";

    public string AdMob_Baner_Android = "ca-app-pub-9234065232619132/7791716008";
    public string AdMob_Baner_iOS = "ca-app-pub-9234065232619132/3361516405";

    private void Awake()
    {


        RequestBanner();
        RequestInterstitial();
        ShowBanner();

    }

    private string GetAdMobIDs(AdType adType)
    {
        switch (adType)
        {
            case AdType.Interstitial:
                switch (Application.platform)
                {
                    case RuntimePlatform.Android: return AdMob_Interstitial_Android;
                    case RuntimePlatform.IPhonePlayer: return AdMob_Interstitial_iOS;
                }
                break;

            case AdType.Banner:
                switch (Application.platform)
                {
                    case RuntimePlatform.Android: return AdMob_Baner_Android;
                    case RuntimePlatform.IPhonePlayer: return AdMob_Baner_iOS;
                }
                break;

            default:
                break;
        }
        return "";
    }


    private BannerView bannerView;

    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(GetAdMobIDs(AdType.Banner), AdSize.SmartBanner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    private InterstitialAd interstitial;

    private void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(GetAdMobIDs(AdType.Interstitial));
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;

        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;


    }

    Action callBack;

    private void HandleInterstitialClosed(object sender, EventArgs e)
    {
        if (callBack != null)
            callBack.Invoke();
    }

    private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        if (callBack != null)
            callBack.Invoke();
    }

    public void ShowInterstitial(Action action)
    {
        callBack = action;
        if (interstitial.IsLoaded())
        {

            interstitial.Show();
            RequestInterstitial();
        }
        else
        {
            if (action != null)
                action.Invoke();
        }
    }

    public void ShowBanner()
    {
        if (Utilities.IsInternetAvailable())
        {
            bannerView.Show();
            Debug.Log("Show");
        }
    }

    public void DestroyBanner()
    {
        bannerView.Destroy();
    }
}

public enum AdType
{
    Interstitial,
    Banner
}