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
        }
    }
    public void Login() {
        Login login = new Login();
        login.DeviceModel = SystemInfo.deviceModel;
        login.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        login.GamePackage = API.Instance.Infor.info.package;
        login.Time = DateTime.Now.ToString();
        login.IP = IPAddress;

        string Json = JsonUtility.ToJson(login);
        reference.Child("Logins").Child(login.DeviceUID).Push().SetRawJsonValueAsync(Json);
    }


    public void AddUser() {
        User user = new User();
        user.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        string Json = JsonUtility.ToJson(user);
        reference.Child("Users").Child(user.DeviceUID).SetRawJsonValueAsync(Json);
    }

    public void clickAds(string AdsType) {
        ClickAds click = new ClickAds();
        click.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        click.AdsType = AdsType;
        click.GamePackage = API.Instance.Infor.info.package;
        click.IP = IPAddress;
        DateTime now = DateTime.Now;
        click.Time = now.ToString();
        string json = JsonUtility.ToJson(click);
        reference.Child("ClickAds").Push().SetRawJsonValueAsync(json);
    }

    public void showAds(string AdsType) {
        ShowAds show = new ShowAds();
        show.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        show.AdsType = AdsType;
        show.GamePackage = API.Instance.Infor.info.package;
        show.IP = IPAddress;
        DateTime now = DateTime.Now;
        show.Time = now.ToString();
        string json = JsonUtility.ToJson(show);
        reference.Child("ShowAds").Push().SetRawJsonValueAsync(json);
    }

    public void purchase(float amount) {
        InApp ia = new InApp();
        ia.DeviceUID = SystemInfo.deviceUniqueIdentifier;
        ia.Amount = amount;
        ia.GamePackage = API.Instance.Infor.info.package;
        DateTime now = DateTime.Now;
        ia.Time = now.ToString();
        string json = JsonUtility.ToJson(ia);
        reference.Child("InApps").Push().SetRawJsonValueAsync(json);
    }
}


[Serializable]
public class User {
    public string DeviceUID;
    public string Gmail;
    public string Facebook;
    public string GameCenter;
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
}


[Serializable]

public class InApp {
    public string DeviceUID;
    public string Time;
    public string IP;
    public string GamePackage;
    public float Amount;
}

[Serializable]
public class ShowAds {
    public string DeviceUID;
    public string Time;
    public string IP;
    public string AdsType;
    public string GamePackage;
}