using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Networking;

public class GemmobTracking : MonoBehaviour{

    private DatabaseReference reference;

    private const string IPLink = "https://api.ipify.org/";

    public string IPAddress;

    public static GemmobTracking Instance;

    void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Init() {
        //create firebase reference
        Firebase.AppOptions secondaryAppOptions = new Firebase.AppOptions {
            ApiKey = API.ApiKey,
            AppId = API.AppId,
            ProjectId = API.ProjectId
        };

        FirebaseApp secondaryApp = Firebase.FirebaseApp.Create(secondaryAppOptions, API.AppName);
        FirebaseDatabase secondaryDatabase = Firebase.Database.FirebaseDatabase.GetInstance(secondaryApp, API.AppUrl);
        reference = secondaryDatabase.RootReference;

        IPAddress = "";

        StartCoroutine(getIP());
    }

    IEnumerator getIP() {
        WWW client = new WWW(IPLink);
        yield return client;
        if (client.error == null || (client.error != null && client.error == string.Empty)) {
            IPAddress = client.text;
            Debug.Log("Ip: " + IPAddress);
            if (!PlayerPrefs.HasKey(API.FirstOpen)) {
                AddUser();
                PlayerPrefs.SetString(API.FirstOpen, "true");
            }
            else {
                Login();
            }
        }
    }
    public void Login() {
        Login login = new Login();
        login.DeviceModel = SystemInfo.deviceModel;
        login.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        login.GamePackage = API.Instance.Infor.info.package;
        string pattern = "yyyy-MM-dd HH:mm:ss";
        login.Time = DateTime.Now.AddMinutes((DateTime.UtcNow - DateTime.Now).TotalMinutes - 8 * 60).ToString(pattern);
        login.IP = IPAddress;

        string Json = JsonUtility.ToJson(login);
        reference.Child("Logins").Child(login.DeviceUID).Push().SetRawJsonValueAsync(Json);
    }


    public void AddUser() {
        User user = new User();
        user.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        user.DeviceModel = SystemInfo.deviceModel;
        string Json = JsonUtility.ToJson(user);
        reference.Child("Users").Child(user.DeviceUID).SetRawJsonValueAsync(Json);
    }

    public void clickAds(string AdsType) {
        ClickAds click = new ClickAds();
        click.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        click.AdsType = AdsType;
        click.GamePackage = API.Instance.Infor.info.package;
        click.IP = IPAddress;
        click.DeviceModel = SystemInfo.deviceModel;
        string pattern = "yyyy-MM-dd HH:mm:ss";
        click.Time = DateTime.Now.AddMinutes((DateTime.UtcNow - DateTime.Now).TotalMinutes - 8 * 60).ToString(pattern);
        string json = JsonUtility.ToJson(click);
        reference.Child("ClickAds").Child(click.DeviceUID).Push().SetRawJsonValueAsync(json);
    }

    public void showAds(string AdsType) {
        ShowAds show = new ShowAds();
        show.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        show.AdsType = AdsType;
        show.GamePackage = API.Instance.Infor.info.package;
        show.IP = IPAddress;
        show.DeviceModel = SystemInfo.deviceModel;
        string pattern = "yyyy-MM-dd HH:mm:ss";
        show.Time = DateTime.Now.AddMinutes((DateTime.UtcNow - DateTime.Now).TotalMinutes - 8 * 60).ToString(pattern);
        string json = JsonUtility.ToJson(show);
        reference.Child("ShowAds").Child(show.DeviceUID).Push().SetRawJsonValueAsync(json);
    }

    public void purchase(float amount) {
        InApp ia = new InApp();
        ia.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        ia.Amount = amount;
        ia.GamePackage = API.Instance.Infor.info.package;
        ia.DeviceModel = SystemInfo.deviceModel;
        string pattern = "yyyy-MM-dd HH:mm:ss";
        ia.Time = DateTime.Now.AddMinutes((DateTime.UtcNow - DateTime.Now).TotalMinutes - 8 * 60).ToString(pattern);
        string json = JsonUtility.ToJson(ia);
        reference.Child("InApps").Child(ia.DeviceUID).Push().SetRawJsonValueAsync(json);
    }
}


[Serializable]
public class User {
    public string DeviceUID;
    public string Gmail;
    public string Facebook;
    public string GameCenter;
    public string DeviceModel;
    public string Phone;
    public string Name;
}
[Serializable]
public class Login {
    public string DeviceUID;
    public string DeviceModel;
    public string Time;
    public string IP;
    public string GamePackage;
}

[Serializable]
public class ClickAds {
    public string DeviceUID;
    public string Time;
    public string IP;
    public string AdsType;
    public string GamePackage;
    public string DeviceModel;
}


[Serializable]

public class InApp {
    public string DeviceUID;
    public string Time;
    public string IP;
    public string GamePackage;
    public float Amount;
    public string DeviceModel;
}

[Serializable]
public class ShowAds {
    public string DeviceUID;
    public string Time;
    public string IP;
    public string AdsType;
    public string GamePackage;
    public string DeviceModel;
}