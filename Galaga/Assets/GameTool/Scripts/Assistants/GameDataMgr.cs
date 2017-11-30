using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameDataMgr : SingletonMonoBehaviour<GameDataMgr>
{
    const string HighScorekey = "HighScoreKEY";
    const string LastScorekey = "LastScoreKEY";
    const string IsOverkey = "IsOverkey";
    const string isOneTapkey = "IsOneTapKey";

    public RectTransform rectT; // Assign the UI element which you wanna capture
    int width; // width of the object to capture
    int height; // height of the object to capture

    [SerializeField]
    Toggle oneTapToggle1, oneTapToggle2;

    void Start()
    {
        Vector2 size = Vector2.Scale(rectT.rect.size, rectT.lossyScale);

        width = Mathf.Min(Screen.width, System.Convert.ToInt32(size.x));
        height = Mathf.Min(System.Convert.ToInt32(size.y), Screen.height);
        oneTapToggle1.isOn = isOneTap;
        oneTapToggle2.isOn = isOneTap;
    }


    public bool isOneTap
    {
        get
        {
            return PlayerPrefs.GetInt(isOneTapkey, 1) == 1; 
        }
        set
        {
            PlayerPrefs.SetInt(isOneTapkey, value ? 1 : 0);
            oneTapToggle2.isOn = value;
            oneTapToggle1.isOn = value;
        }
    }
    public bool IsOver
    {
        get { return PlayerPrefs.GetInt(IsOverkey, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(IsOverkey, value ? 1 : 0);
        }
    }
    public int HighScore
    {
        get { return PlayerPrefs.GetInt(HighScorekey, 0); }
        set
        {
            if (value > PlayerPrefs.GetInt(HighScorekey, 0))
            {
                PlayerPrefs.SetInt(HighScorekey, value);
            }
        }
    }

    public int LastScore
    {
        get { return PlayerPrefs.GetInt(LastScorekey, 0); }
        set
        {
            if (value > PlayerPrefs.GetInt(LastScorekey, 0))
            {
                PlayerPrefs.SetInt(LastScorekey, value);
            }
        }
    }

    public IEnumerator takeScreenShot()
    {
        yield return new WaitForEndOfFrame(); // it must be a coroutine 

        Vector2 temp = rectT.transform.position;
        var startX = temp.x - width / 2;
        var startY = temp.y - height / 2;

        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        Debug.Log(width + "  " + height);
        tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        screenShot = tex.EncodeToPNG();
        Destroy(tex);



    }

    byte[] screenShot;

    /// <summary>
    /// Save the specified data and slot.
    /// 4 is GameData lasted
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="slot">Slot.</param>
    //it's static so we can call it from anywhere
    public void Save(GameData data, int slot, bool saveImage)
    {
        //  print("take screen");
        if (saveImage)
            File.WriteAllBytes(Application.persistentDataPath + "/" + slot + "ScreenShot.png", screenShot);
        // 
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.persistentDataPath + "/" + slot + "savedGames.gd"); //you can call it anything you want

        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Load GameData from the specified slot.
    /// 4 is GameData lasted
    /// </summary>
    /// <param name="slot">Slot.</param>
    public GameData Load(int slot)
    {
        GameData result = null;
        if (File.Exists(Application.persistentDataPath + "/" + slot + "savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + slot + "savedGames.gd", FileMode.Open);
            result = (GameData)bf.Deserialize(file);
            file.Close();

        }
        return result;
    }

    public void Delete(int slot)
    {
        if (File.Exists(Application.persistentDataPath + "/" + slot + "savedGames.gd"))
        {

            File.Delete(Application.persistentDataPath + "/" + slot + "savedGames.gd");
            File.Delete(Application.persistentDataPath + "/" + slot + "ScreenShot.png");

        }
    }

}
