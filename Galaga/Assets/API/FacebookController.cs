using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public partial class FacebookController : MonoBehaviour
{
    public const string AndroidAppLink = "";
    public const string IosAppLink = "https://itunes.apple.com/us/app/overdrive-ninja-shadow/id1325407162?ls=1&mt=8";
    public static string AppLink;

    public static FacebookController Instance;

    // ... Action khi user login thành công, có thể dùng để thưởng điểm khi user login thành công
    public static Action OnLoginSuccess;
    public static Action OnLoginFailed;

    // ... Action load thành công avatar facebook của user, có thể dùng để cập nhật lại ảnh đại diện trong game của người chơi theo facebook avatar
    public static Action OnAvatarLoadComplete;

    // Avatar của người chơi
    public static Sprite AvatarSprite;

    // Nội dung share với image
    public const string ContentShare = "Awesome! Join with me :D ";

    public static List<string> Description = new List<string>()
    {
        "Let's play: ",
        "Play fun game: ",
        "Let's try it: ",
    };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AppLink = IosAppLink;
#if UNITY_ANDROID
        AppLink = "https://play.google.com/store/apps/details?id=" + Application.identifier;
#endif
    }

    public void InitFacebook()
    {
        // Check Facebook đã khởi tạo?...
        if (!FB.IsInitialized)
            // ... chưa khởi tạo, gọi hàm khởi tạo
            FB.Init(OnInitComplete);
        else
            // ... đã khởi tạo, active app
            ActiveApp();
    }


    public static void LoginFacebook(Action onLoginSuccess = null, Action onLogginFailed = null)
    {
        // Check facebook đã được init
        if (FB.IsInitialized)
        {
            OnLoginSuccess = null;
            OnLoginSuccess = onLoginSuccess;

            OnLoginFailed = null;
            OnLoginFailed = onLogginFailed;

            // Danh sách các quyền cần dùng
            var perms = new List<string>() { "public_profile", "email", "user_friends", };

            // Gọi hàm login với các quyền trong list và gọi lại vào hàm AuthCallback khi user login thành công
            FB.LogInWithReadPermissions(perms, AuthCallback);

            //GooglePlayServicesScript.ReportProgress(GPGSIds.achievement_save_data);
        }
        else
        {
            // Khởi tạo facebook và gọi lại vào login khi khởi tạo thành công
            FB.Init(() =>
            {
                LoginFacebook(onLoginSuccess, onLogginFailed);
            });
        }
    }

    public static void ShareImage(string link, Texture2D screenTexture, Action onShareComplete = null)
    {
        // Chuyển đôi Texture2D sang mảng byte
        var screenshot = screenTexture.EncodeToPNG();

        // Đặt tên ảnh theo quy tắc
        string imgName = "CatDefender Screenshort" + PlayerPrefs.GetInt("CountImage", 0) + ".png";

        // Lưu số đếm số lượng ảnh
        PlayerPrefs.SetInt("CountImage", PlayerPrefs.GetInt("CountImage") + 1);
        var wwwForm = new WWWForm();

        // Add mảng byte vào form
        wwwForm.AddBinaryData("image", screenshot, imgName);

#if UNITY_ANDROID

        // Nếu ứng dụng là android, thêm link vào nội dung share
        wwwForm.AddField("name", ContentShare + "https://play.google.com/store/apps/details?id=" + Application.identifier);
#endif

#if UNITY_IPHONE
		// Nếu ứng dụng là android, thêm link vào nội dung share
		wwwForm.AddField("name", ContentShare + "https://itunes.apple.com/us/app/galaxy-strike-space-shooting-squadron/id1262324521?ls=1&mt=8");
#endif

        // Gọi hàm post ảnh lên face
        FB.API("me/photos", HttpMethod.POST, result =>
        {
            if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
            {
                Debug.Log("ShareImage Error: " + result.Error);
            }
            else if (result.Error == null)
            {
                // Gọi các event share complete
                if (onShareComplete != null)
                    onShareComplete.Invoke();
            }
            else
            {
                Debug.Log("ShareImage Error: " + result.Error);
            }
        }, wwwForm);
    }

    public static void ShareAppLink(string link, Action onShareComplete = null)
    {
        if (FB.IsLoggedIn)
        {
            // Share
            FB.ShareLink(
                new Uri(link),
                Application.productName,
                Description[UnityEngine.Random.Range(0, Description.Count)],
                null,
                result =>
                {
                    if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
                    {
                        Debug.Log("ShareLink Error: " + result.Error);
                    }
                    else if (!String.IsNullOrEmpty(result.PostId))
                    {
                        // Print post identifier of the shared content
                        Debug.Log(result.PostId);
                        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                        {
                            if (onShareComplete != null)
                                onShareComplete.Invoke();
                        }
                    }
                    else
                    {
                        // Share succeeded without postID
                        Debug.Log("ShareLink success!");

                        if (onShareComplete != null)
                            onShareComplete.Invoke();
                    }
                });
        }
        else
        {
            LoginFacebook(() =>
            {
                ShareAppLink(link, onShareComplete);
            });
        }
    }

    // --------------------------------------------------------------------------------------

    private static void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
                Debug.Log(perm);

            // Lưu lại Id facebook của user
            SaveUserId(aToken.UserId);

            // Gọi lại khi login xong
            if (OnLoginSuccess != null)
                OnLoginSuccess.Invoke();
            Instance.StartCoroutine(GetFBProfilePicture(aToken.UserId));
        }
        else
        {
            if (OnLoginFailed != null)
                OnLoginFailed.Invoke();
            Debug.Log("User cancelled login");
        }
    }

    public void OnInitComplete()
    {
        // Active app
        ActiveApp();
    }

    void ActiveApp()
    {
        FB.ActivateApp();
        FB.GetAppLink(result =>
        {

        });
    }

    /// <summary>
    /// Hàm lấy ảnh profile của user theo user id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static IEnumerator GetFBProfilePicture(string userId)
    {
        WWW url = new WWW(System.Uri.EscapeUriString("https://graph.facebook.com/" + userId + "/picture?type=large"));
        yield return url;
        Debug.Log("Completed.");

        var bytes = url.texture.EncodeToPNG();
        var path = System.IO.Path.Combine(Application.persistentDataPath, "useravatar.png");
        print("Avatar path: " + path);
        System.IO.File.WriteAllBytes(path, bytes);

        if (OnAvatarLoadComplete != null)
            OnAvatarLoadComplete.Invoke();
    }

    /// <summary>
    /// Lưu ảnh avatar vào bộ nhớ và trả về ảnh avatar cho nơi gọi
    /// </summary>
    /// <returns></returns>
    public static Sprite GetAvatarSprite()
    {
        if (AvatarSprite == null)
        {
            var path = System.IO.Path.Combine(Application.persistentDataPath, "useravatar.png");
            var bytesRead = System.IO.File.ReadAllBytes(path);
            Texture2D myTexture = new Texture2D(128, 128);
            myTexture.LoadImage(bytesRead);
            AvatarSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);
        }
        return AvatarSprite;
    }

    /// <summary>
    /// Hàm lưu user id
    /// </summary>
    /// <param name="id"></param>
    public static void SaveUserId(string id)
    {
        PlayerPrefs.SetString("FacebookUserId", id);
    }

    /// <summary>
    /// Hàm get user id
    /// </summary>
    /// <returns></returns>
    public static string GetUserid()
    {
#if UNITY_EDITOR
        Debug.Log("UserID:" + PlayerPrefs.GetString("FacebookUserId"));
#endif
        return PlayerPrefs.GetString("FacebookUserId", string.Empty);
    }
}