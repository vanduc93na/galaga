using System;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdmobManager : MonoBehaviour
{
    private Action _onRewardComplete;

    private string _bannerId, _fullId, _videoId;

    private AdSize _adSize = AdSize.Banner;
    private AdPosition _adPostion = AdPosition.Bottom;
    private BannerView _bannerView;
    private InterstitialAd _interstitial;
    private RewardBasedVideoAd _rewardBasedVideo;

    public bool IsFullRequesting;
    public bool IsBannerRequesting;
    public bool IsVideoRequesting;

    private bool _isBannerLoaded;

    public void Init(string bannerId, string fullId, string videoId, bool useInterstitial, bool useVideo)
    {
        Debug.Log("Init admob");
        _bannerId = bannerId;
        _fullId = fullId;
        _videoId = videoId;
        bool isRewarded = false;
        _rewardBasedVideo = RewardBasedVideoAd.Instance;
        _rewardBasedVideo.OnAdClosed += (sender, args) =>
        {
            if (isRewarded)
            {
                isRewarded = false;
                if (_onRewardComplete != null)
                    _onRewardComplete.Invoke();
            }
            GaLog("admob_video", "closed");
            //Invoke("PlayAudio", 0);

            RequestVideo();
        };
        _rewardBasedVideo.OnAdLeavingApplication += (sender, args) =>
        {
            GaLog("admob_video", "click");
            API.Instance.TrackingClickAds(API.Rewarded);
        };
        _rewardBasedVideo.OnAdOpening += (sender, args) =>
        {
            GaLog("admob_video", "opening");
        };
        _rewardBasedVideo.OnAdFailedToLoad += (sender, args) =>
        {
            IsVideoRequesting = false;

            Debug.Log("Admob video FailedToLoad: " + args.Message);
            GaLog("admob_video", "failedToLoad");
        };
        _rewardBasedVideo.OnAdLoaded += (sender, args) =>
        {
            IsVideoRequesting = false;
            GaLog("admob_video", "loaded ");
        };
        _rewardBasedVideo.OnAdStarted += (sender, args) =>
        {
            //Invoke("PauseAudio", 0);
            GaLog("admob_video", "started");
        };
        _rewardBasedVideo.OnAdRewarded += (sender, reward) =>
        {
            GaLog("admob_video", "rewarded");
            isRewarded = true;
        };

        // Request banner
        //RequestBanner();
        // Request full
        if (useInterstitial) {
            RequestInterstitial();
        }

        // Request video (reward)
        if (useVideo) {
            RequestVideo();
        }

        Debug.Log("Init admob completed");
    }

    private void RequestBanner()
    {
        Debug.Log("Admob banner request id: " + _bannerId);
        _bannerView = new BannerView(_bannerId, _adSize, _adPostion);
        _bannerView.OnAdLeavingApplication += (sender, args) =>
        {
            GaLog("admob_banner", "click");
            API.Instance.TrackingClickAds(API.Banner);
        };
        _bannerView.OnAdFailedToLoad += (sender, args) =>
        {
            GaLog("admob_banner", "failedToLoad");
            IsBannerRequesting = true;
        };
        _bannerView.OnAdClosed += (sender, args) =>
        {
            GaLog("admob_banner", "closed");
        };
        _bannerView.OnAdLoaded += (sender, args) =>
        {
            GaLog("admob_banner", "loaded");
            _isBannerLoaded = true;
        };
        _bannerView.OnAdOpening += (sender, args) =>
        {
            GaLog("admob_banner", "opening");
        };

        AdRequest request = new AdRequest.Builder().Build();
        _bannerView.LoadAd(request);
    }

    void InvockFullClose()
    {
        if (OnFullClosed != null)
            OnFullClosed.Invoke();
    }

    private void RequestInterstitial()
    {
        if (IsFullRequesting)
            return;

        if (_interstitial != null)
            _interstitial.Destroy();

        IsFullRequesting = true;

        Debug.Log("Admob full request id: " + _fullId);
        _interstitial = new InterstitialAd(_fullId);
        _interstitial.OnAdClosed += (sender, args) =>
        {
            //Invoke("PlayAudio", 0);
            Invoke("InvockFullClose", 0);
            RequestInterstitial();
            GaLog("admob_full", "closed");
            
        };
        _interstitial.OnAdLeavingApplication += (sender, args) =>
        {
            GaLog("admob_full", "click");
            API.Instance.TrackingClickAds(API.Interstitial);
        };
        _interstitial.OnAdOpening += (sender, args) =>
        {
            GaLog("admob_full", "opening");
            //Invoke("PauseAudio", 0);
        };
        _interstitial.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.Log("Admob full failed to load: " + args.Message);
            GaLog("admob_full", "failedToLoad");
            IsFullRequesting = false;
            //if (API.Instance._unityAdsManager == null)
            //    API.Instance.AddUnityAds();
        };
        _interstitial.OnAdLoaded += (sender, args) =>
        {
            GaLog("admob_full", "loaded");
            IsFullRequesting = false;
        };

        AdRequest request = new AdRequest.Builder().Build();
        //request.TestDevices.Add("FDA89F2EDECB26D2822088001079CBEB");
        _interstitial.LoadAd(request);
    }

    private void RequestVideo()
    {
        if (IsVideoRequesting)
            return;

        IsVideoRequesting = true;
        Debug.Log("Admob video request id: " + _videoId);
        AdRequest request = new AdRequest.Builder().Build();
        //request.TestDevices.Add("FDA89F2EDECB26D2822088001079CBEB");
        _rewardBasedVideo.LoadAd(request, _videoId);
    }

    private void PauseAudio()
    {
        AudioListener.pause = true;
    }

    private void PlayAudio()
    {
        AudioListener.pause = false;
    }

    public void ShowBanner()
    {
        //_bannerView.Show();
    }

    public void HideBanner()
    {
        //if (_bannerView != null)
        //    _bannerView.Hide();
    }

    public bool IsBannerLoaded()
    {
        return _isBannerLoaded;
    }

    public Action OnFullClosed;
    public void ShowFull(Action callback)
    {
        if (_interstitial == null)
        {
            Debug.Log("_interstitial null");
            if (callback != null)
                callback.Invoke();
            RequestInterstitial();
        }
        else
        {
            if (_interstitial.IsLoaded())
            {
                OnFullClosed = null;
                OnFullClosed = callback;
                _interstitial.Show();
            }
            else
            {
                if (callback != null)
                    callback.Invoke();
                RequestInterstitial();
            }
        }
    }

    public bool IsVideoLoaded()
    {
        if (_rewardBasedVideo == null)
        {
            Debug.Log("Admob video null");
            return false;
        }

        if (_rewardBasedVideo.IsLoaded())
            return true;

        RequestVideo();
        return false;
    }

    public void ShowVideo(Action callBack = null)
    {
        if (_rewardBasedVideo.IsLoaded())
        {
            _onRewardComplete = null;
            _onRewardComplete = callBack;
            _rewardBasedVideo.Show();
        }
        else
            RequestVideo();
    }

    public void GaLog(string eventCategory, string eventAction)
    {
        FirebaseAnalytics.LogEvent("ads", eventCategory, eventAction);
        FirebaseAnalytics.LogEvent("ads", eventCategory + ": " + eventAction, 1);
    }
}