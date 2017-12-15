using UnityEngine;

public static class EditorStyleExtension
{
    public static GUIStyle CurrentLevelNameStyle = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = Color.yellow,
        }
    };

    public static GUIStyle WaveNameStyle = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = Color.green,
        },
        fontSize = 20
    };

    public static GUIStyle TypeWave = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = Color.gray
        }
    };

    public static GUIStyle Wave = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = Color.yellow
        },

        fontSize = 13
    };

    public static GUIStyle Level = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = new Color(1, 1, 0.5f, 1)
        },
        fontSize = 30
    };

    public static GUIStyle Achievements = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = Color.yellow,
        },
        fontSize = 15
    };

    public static GUIStyle Others = new GUIStyle()
    {
        normal = new GUIStyleState()
        {
            textColor = new Color(1, 1, 0.5f, 1)
        },
    };
}
