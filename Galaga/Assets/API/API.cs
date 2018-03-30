using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Facebook.Unity.Settings;
using UnityEngine;

public partial class API : MonoBehaviour
{
    public static Action OnRemoveAdsChanged;
    public static API Instance;
    public bool IsAutoShowBanner;
    public static float TimeScale;

    public string ApiLink;
    public string IosApiLink;

    public ApiInfo Infor;

    // admob
    private AdmobManager _admobManager;

    // Push
    private PushManager _pushManager;

    // Startapp
    private StartAppManager _startAppManager;

    // Unity ads
    [HideInInspector]
    public UnityAdsManager _unityAdsManager;
    

    private bool useAdMobInterstitial;
    private bool useAdmobVideo;

    void Awake()
    {
        _oldTime = Time.time;
        if (Instance == null)
        {
            // Lưu lại đối tượng api
            Instance = this;

            // Bỏ hủy đối tượng khi chuyển scene
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<GemmobTracking>();
        }
        else
            // Hủy đối tượng nếu nó đã tồn tại trên scene
            Destroy(gameObject);
    }

    void Start()
    {

        GemmobTracking.Instance.Init();
        if (!PlayerPrefs.HasKey(FirstOpen)) {
            GemmobTracking.Instance.AddUser();
            PlayerPrefs.SetString(FirstOpen, "false");
        }
        else {
            GemmobTracking.Instance.Login();
        }

        //if (Application.platform == RuntimePlatform.IPhonePlayer)


        // Check ngày hiện tại và ngày đã lưu, nếu ...
        if (PlayerPrefs.GetString(ApiDayString) != DateTime.Now.ToShortDateString())
        // .. ngày hiện tại khác ngày đã lưu, load data từ server
        {
            // Lưu ngày hiện tại
            PlayerPrefs.SetString(ApiDayString, DateTime.Now.ToShortDateString());
            LoadDataFormServer();
        }
        else

            // .. ngày hiện tại không khác ngày đã lưu, lấy data từ local
            LoadLocalData();
    }

    #region LoadData


    public void LoadDataFormServer2()
    {
        LoadDataFormServer(true);
    }

    /// <summary>
    /// Hàm Load data từ server
    /// </summary>
    /// <param name="isEditorLoad"></param>
    void LoadDataFormServer(bool isEditorLoad = false)
    {
        StartCoroutine(RequsetData(isEditorLoad));
    }

    /// <summary>
    ///  Hàm load data
    /// </summary>
    /// <param name="isEditorLoad"></param>
    /// <returns></returns>
    IEnumerator RequsetData(bool isEditorLoad)
    {
#if UNITY_IOS || UNITY_IPHONE
            ApiLink = IosApiLink;
#endif

        WWW client = new WWW(ApiLink);
        yield return client;

        // Check load, nếu...
        if (client.error == null || (client.error != null && client.error == string.Empty))
        {
            // ... nếu không có lỗi xảy ra, xử lý dữ liệu
            Debug.Log("Api requst data success!");

            // Convert data sang dạng ApiInfo
            Infor = JsonUtility.FromJson<ApiInfo>(client.text);

            // Save data to resource folder
            // Nếu là unity editor
            if (isEditorLoad)
            {
                // Lưu file
                using (FileStream fs = new FileStream(string.Format("Assets/API/Resources/{0}.json", SaveFileString), FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(client.text);
                        writer.Close();
                        writer.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
#if UNITY_EDITOR
                // Refesh data
                UnityEditor.AssetDatabase.Refresh();

                // Setting facebook
                // - Set facebook app id
                FacebookSettings.AppIds = new List<string> { Infor.info.facebook_app_id };
                // - Set facebook name
                FacebookSettings.AppLabels = new List<string> { Infor.info.facebook_name };

                // Setting compress Apk file
                UnityEditor.PlayerSettings.strippingLevel = UnityEditor.StrippingLevel.StripAssemblies;

                // Setting force to SD card
                UnityEditor.PlayerSettings.Android.forceSDCardPermission = true;

                // Setting game information
                // - Set tên company
                UnityEditor.PlayerSettings.companyName = Infor.info.name;
                // - Set tên game, tên sản phẩm
                UnityEditor.PlayerSettings.productName = Infor.info.name;
                // - Set package game
                UnityEditor.PlayerSettings.applicationIdentifier = Infor.info.package;
                // - Set package cho ios
                UnityEditor.PlayerSettings.SetApplicationIdentifier(UnityEditor.BuildTargetGroup.iOS, Infor.info.package);
                // - Set min api level là 16
                UnityEditor.PlayerSettings.Android.minSdkVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevel16;

                // Setting Startapp data
                // Lưu startapp id
                using (FileStream fs = new FileStream("Assets/Resources/StartAppData.txt", FileMode.OpenOrCreate))
                {
                    string startAppData = string.Format("applicationId={0}\nreturnAds=true", Infor.ads.backup.startapp);
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(startAppData);
                        writer.Close();
                        writer.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }

                // Lưu các thay đổi trên editor
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }

            // Check version, nếu...
            if (Infor.info.name == string.Empty)
            {
                // ... nếu không có version, khởi tọa bằng data mặc định hoặc gần nhấn
                Debug.Log("Web faild, load data local data");
                Infor = JsonUtility.FromJson<ApiInfo>(Resources.Load<TextAsset>(SaveFileString).text);
            }
        }
        else
        {
            //... request lỗi, khởi tọa bằng data mặc định hoặc gần nhấn
            Debug.Log("Request faild, load data local data" + client.error.ToString());
            Infor = JsonUtility.FromJson<ApiInfo>(Resources.Load<TextAsset>(SaveFileString).text);
        }

        // Nếu game đang play, init api
        if (Application.isPlaying)
            InitApi();
        //Lưu ngày API 
        PlayerPrefs.SetString(ApiDayString, DateTime.Now.ToShortDateString());
    }

    private void LoadLocalData()
    {
        Debug.Log("Load data from local");
        Infor = JsonUtility.FromJson<ApiInfo>(Resources.Load<TextAsset>(SaveFileString).text);
        if (Application.isPlaying)
            InitApi();
    }

    private void InitApi()
    {
        // Admob
        useAdMobInterstitial = Infor.ads.control.interstitial == "admob" ? true : false;
        useAdmobVideo = Infor.ads.control.video == "admob" ? true : false;
        if (Infor.info.name != string.Empty &&(useAdMobInterstitial || useAdmobVideo))
        {
            AddAdmob(useAdMobInterstitial, useAdmobVideo);
        }

        if (!useAdmobVideo)
        {
            AddUnityAds();
        }
        if (!useAdMobInterstitial) {
            AddStartApp();
        }

        // Push
        GameObject push = new GameObject("Push");
        push.transform.SetParent(transform);
        _pushManager = push.AddComponent<PushManager>();
        _pushManager.Init(Infor.info.onesignal_code);

        // Facebook
        // - Tạo gameobject facebook
        GameObject facebook = new GameObject("Facebook");
        // - Set làm con của obj API
        facebook.transform.SetParent(transform);
        // - Add script FacbookController vào obj facebook
        facebook.AddComponent<FacebookController>();

#if UNITY_EDITOR
        AddUnityAds();
#endif
    }

    void AddAdmob(bool useAdmobInterstitial, bool useAdmobRewarded)
    {
        // - Tạo object admob
        GameObject admob = new GameObject("Admob");
        // - Set làm con của API
        admob.transform.SetParent(transform);
        // - Add script
        _admobManager = admob.AddComponent<AdmobManager>();
        // - Gọi hàm Init admob
        _admobManager.Init(Infor.ads.admob.banner, Infor.ads.admob.interstitial, Infor.ads.admob.video, useAdmobInterstitial, useAdmobRewarded);
    }

    /// <summary>
    /// Hàm Add UnityAds
    /// </summary>
    public void AddUnityAds()
    {
        if (_unityAdsManager == null)
        {
            // StartApp
            GameObject unityAds = new GameObject("UnityAds");
            unityAds.transform.SetParent(transform);
            _unityAdsManager = unityAds.AddComponent<UnityAdsManager>();
            _unityAdsManager.Init(Infor.ads.backup.unityads);
        }
    }

    /// <summary>
    ///  Hàm ads startapp
    /// </summary>
    private void AddStartApp()
    {
        if (_startAppManager == null)
        {
            // StartApp
            GameObject startApp = new GameObject("StartApp");
            startApp.transform.SetParent(transform);
            _startAppManager = startApp.AddComponent<StartAppManager>();
            _startAppManager.Init();
        }
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("API/InitData")]
    public static void EditorInitData()
    {
        var obj = GameObject.FindObjectOfType<API>();
        obj.LoadDataFormServer(true);
    }
#endif

    #endregion

    #region Ads

    // Show banner ads
    public static void ShowBanner()
    {
        if (Instance == null || IsRemoveAds)
            return;
        Instance.ShowBannerAds();
    }

    // Show hide banner
    public static void HideBanner()
    {
        if (Instance == null)
            return;
        Instance.HideBannerAds();
    }

    // Show full 30%
    public static void ShowFull30(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback.Invoke();
        return;
#endif
        int rand = UnityEngine.Random.Range(0, 101);
        if (rand <= 30)
        {
            if (Instance == null || IsRemoveAds)
            {
                if (callback != null)
                    callback.Invoke();
                return;
            }
            Instance.ShowFullAds(callback);
        }
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }

    private static float _oldTime;

    // Show full ads
    public static void ShowFullWait(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback.Invoke();
        return;
#endif

        if (Time.time - _oldTime > 100)
        {
            _oldTime = Time.time;
            if (Instance == null || IsRemoveAds)
            {
                if (callback != null)
                    callback.Invoke();
                return;
            }
            Instance.ShowFullAds(callback);
        }
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }

    // Show full ads
    public static void ShowFull(Action callback)
    {
#if UNITY_EDITOR ||UNITY_STANDALONE_WIN
        if (callback != null)
            callback.Invoke();
        return;
#endif
        if (Instance == null)
        {
            Debug.Log("Api null");
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (IsRemoveAds)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        Instance.ShowFullAds(callback);
    }

    public static bool CheckHasVideo()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return true;
#endif

        if (Instance == null)
            return false;

        return Instance.CheckHasVideoAds();
    }

    public static void ShowVideo(Action callBack = null)
    {
//#if UNITY_EDITOR || UNITY_STANDALONE_WIN
//        if (callBack != null)
//            callBack.Invoke();
//        return;
//#endif

        if (Instance == null)
            return;
        Instance.ShowVideoAds(callBack);
    }

    private void ShowBannerAds()
    {
            if (_admobManager != null)
                _admobManager.ShowBanner();
            if (_startAppManager != null)
                if (_startAppManager.IsShowingBanner)
                    _startAppManager.HideBanner();
        GemmobTracking.Instance.showAds(Banner);
    }

    private void HideBannerAds()
    {
            if (_admobManager != null)
                _admobManager.HideBanner();
            if (_startAppManager != null)
                if (_startAppManager.IsShowingBanner)
                    _startAppManager.HideBanner();
    }

    private void ShowFullAds(Action callback)
    {
            if (_admobManager != null)
                _admobManager.ShowFull(callback);
            else
            {
                Debug.Log("Admob full null");
                if (callback != null)
                    callback.Invoke();
            }
        GemmobTracking.Instance.showAds(Interstitial);
    }

    public static void ShowFullWithX1(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback.Invoke();
        return;
#endif
        if (IsRemoveAds)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (Instance == null)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (UnityEngine.Random.Range(0, 101) <= Instance.Infor.ads.show.x1)
            Instance.ShowFullAds(callback);
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }

    public static void ShowFullWithX2(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback.Invoke();
        return;
#endif
        if (IsRemoveAds)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (Instance == null)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (UnityEngine.Random.Range(0, 101) <= Instance.Infor.ads.show.x2)
            Instance.ShowFullAds(callback);
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }
    public static void ShowFullWithX3(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback.Invoke();
        return;
#endif
        if (IsRemoveAds)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (Instance == null)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (UnityEngine.Random.Range(0, 101) <= Instance.Infor.ads.show.x3)
            Instance.ShowFullAds(callback);
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }
    public static void ShowFullWithX4(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
            callback.Invoke();
        return;
#endif
        if (IsRemoveAds)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (Instance == null)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (UnityEngine.Random.Range(0, 101) <= Instance.Infor.ads.show.x4)
            Instance.ShowFullAds(callback);
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }
    public static void ShowFullWithX5(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
        {
            callback.Invoke();
        }
        return;
#endif
        if (IsRemoveAds)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (Instance == null)
        {
            if (callback != null)
                callback.Invoke();
            return;
        }
        if (UnityEngine.Random.Range(0, 101) <= Instance.Infor.ads.show.x5)
            Instance.ShowFullAds(callback);
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }

    private bool CheckHasVideoAds()
    {
        if (_admobManager != null && _admobManager.IsVideoLoaded())
            return true;

        // return true if has unityads
        if (_unityAdsManager == null)
        {
            AddUnityAds();
            return false;
        }
        if (_unityAdsManager.IsVideoAvailable())
            return true;
        return false;
    }

    private void ShowVideoAds(Action callBack = null)
    {
#if UNITY_EDITOR
        // Show unityads video ads trên editor
        if (_unityAdsManager == null)
            AddUnityAds();
        else if (_unityAdsManager.IsVideoAvailable())
            _unityAdsManager.ShowRewardedAd(callBack);
        return;
#endif
        if (Infor.ads.control.video == "unity")
        {
            // Ưu tiên unity
            if (_unityAdsManager != null && _unityAdsManager.IsVideoAvailable())
            {
                // Show unity ads
                _unityAdsManager.ShowRewardedAd(callBack);
            }
            else
            {
                if (_admobManager == null)
                {
                    AddAdmob(useAdMobInterstitial, useAdmobVideo);
                }
                else
                {
                    if (_admobManager.IsVideoLoaded())
                        _admobManager.ShowVideo(callBack);
                }
            }
        }
        else
        {
            // Ưu tiên admob
            if (_admobManager != null && _admobManager.IsVideoLoaded())
                _admobManager.ShowVideo(callBack);
            else
            {
                // Show unityads video ads
                if (_unityAdsManager == null)
                    AddUnityAds();
                else if (_unityAdsManager.IsVideoAvailable())
                    _unityAdsManager.ShowRewardedAd(callBack);
            }
        }
        GemmobTracking.Instance.showAds(Rewarded);
    }


    // ------------------------------------------------------------------------------------------------------
    // More function 

    private static void ShowAdmobBanner()
    {
        if (Instance == null || Instance._admobManager == null)
            return;
        Instance._admobManager.ShowBanner();
    }

    private static void HideAdmobBanner()
    {
        if (Instance == null || Instance._admobManager == null)
            return;

        Instance._admobManager.HideBanner();
    }

    private static void ShowStartAppBanner()
    {
        if (Instance == null)
            return;

        if (Instance._startAppManager == null)
            Instance.AddStartApp();
        else
            Instance._startAppManager.ShowBanner();
    }

    private static void HideStartAppBanner()
    {
        if (Instance == null)
            return;

        if (Instance._startAppManager == null)
            Instance.AddStartApp();
        else
            Instance._startAppManager.HideBanner();
    }

    private static void ShowAdmobFull(Action callback)
    {
#if UNITY_EDITOR
        if (callback != null)
        {
            callback.Invoke();
        }
        return;
#endif
        if (Instance == null || Instance._admobManager == null)
            return;
        Instance._admobManager.ShowFull(callback);
    }

    private static void ShowStartappFull()
    {
        if (Instance == null)
            return;

        if (Instance._startAppManager == null)
            Instance.AddStartApp();
        else
            Instance._startAppManager.ShowFull();
    }

    private static void ShowAdmobVideo(Action callBack)
    {
        if (Instance == null || Instance._admobManager == null)
            return;

        Instance._admobManager.ShowVideo(callBack);
    }

    private static void ShowUnityAdsVideo(Action callBack)
    {
        if (Instance == null)
            return;

        if (Instance._unityAdsManager == null)
            Instance.AddUnityAds();
        else
            Instance._unityAdsManager.ShowRewardedAd(callBack);
    }

    #endregion

    public static void GaLogItem(string scene)
    {
        //GoogleAnalyticsV4.instance.LogItem("transID", "name", "sku", "category", 1, Int64.MaxValue);
    }

    /// <summary>
    /// Hàm log các sự kiện màn hình
    /// </summary>
    /// <param name="scene"></param>
    public static void GaLogScene(string scene)
    {
        //GoogleAnalyticsV4.instance.LogScreen(scene);
    }

    /// <summary>
    /// Hàm log các sự kiện quảng cáo, inapp,...
    /// </summary>
    /// <param name="eventCategory"></param>
    /// <param name="eventAction"></param>
    public static void GaLogEvent(string eventCategory, string eventAction)
    {
        //GoogleAnalyticsV4.instance.LogEvent(eventCategory, eventAction, "", 0);
    }

    public static bool IsRemoveAds
    {
        get
        {
            return PlayerPrefs.GetInt("IsRemoveAds", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsRemoveAds", value ? 1 : 0);
            if (OnRemoveAdsChanged != null)
                OnRemoveAdsChanged.Invoke();
        }
    }
    #region  gemmob-tracking

    public void TrackingShowAds(string adsType) {
        GemmobTracking.Instance.showAds(adsType);
    }

    public void TrackingClickAds(string adsType) {
        GemmobTracking.Instance.clickAds(adsType);
    }

    public void TrackingIAP(float amount) {
        GemmobTracking.Instance.purchase(amount);
    }

    #endregion
}


// Class

[Serializable]
public class ApiInfo {
    // Thông tin game
    public GameInfo info;
    // Thông tin quảng cáo
    public AdsInfo ads;
}

[Serializable]
public class GameInfo {
    // android or ios
    public string type;
    // Tên ứng dụng
    public string name;
    // Bundle ứng dụng
    public string package;
    // Version api (version ứng ụng)
    public string version_name;
    // Version code
    public string version_code;
    // Id GA
    public string tracking_id;
    //OneSignal id
    public string onesignal_code;
    // Facebook name
    public string facebook_name;
    // Facebook id
    public string facebook_app_id;
    // Developer name => show more games link
    public string developer;
}

[Serializable]
public class AdmobInfo {
    // Id banner
    public string banner;
    // Id full
    public string interstitial;
    // Id video reward
    public string video;
}

[Serializable]
public class BackupInfo {
    //banner id (starapp)
    public string startapp;
    //video id (unity)
    public string unityads;
}
[Serializable]
public class ControlInfo {
    //intertitial ads(admob or backup)
    public string interstitial;

    //video ads(admob or backup)
    public string video;
}
[Serializable]
public class ShowInfo {
    public int x1;
    public int x2;
    public int x3;
    public int x4;
    public int x5;
}

[Serializable]
public class AdsInfo {
    public AdmobInfo admob;
    public BackupInfo backup;
    public ControlInfo control;
    public ShowInfo show;
}
