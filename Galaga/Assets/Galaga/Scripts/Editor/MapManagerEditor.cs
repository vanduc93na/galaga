using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class MapManagerEditor : EditorWindow
{
    /// <summary>
    /// init windows extension
    /// </summary>
    [MenuItem("Gemmod/ToolMap %m")]
    static void Init()
    {
        GetWindow<MapManagerEditor>().minSize = new Vector2(500, 500);
    }

    private List<int> _idPath;
    private LevelInformation[] _levels;
    private LevelInformation _level;
    private Vector3 _scrollPossition;
    private int _selectedIndex = 0;
    private int _row = 0;
    private int _col = 0;
    private int _rowChange = 0;
    private int _colChange = 0;
    private int _sizeListDropItem = 0;

    void OnGUI()
    {
        InitValue();
        OnProcess();
    }

    void InitValue()
    {

    }

    void OnProcess()
    {
        LoadAsset();
        CreateGUI();
    }

    void CreateGUI()
    {

        _scrollPossition = EditorGUILayout.BeginScrollView(_scrollPossition);
        EditorGUILayout.BeginHorizontal();
        EditorGUITool.BorderBox(100, 100, () =>
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(300);
            GUILayout.Label("Level", EditorStyleExtension.Level, GUILayout.Width(150));
            GUILayout.BeginVertical();
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _levels.Select(s => s.name).ToArray(), GUILayout.Width(150));
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(300);
            if (GUILayout.Button("Load", GUILayout.Width(200), GUILayout.Height(30)))
            {
                LoadLevel();
            }
            GUILayout.Space(30);
            if (GUILayout.Button("Save", GUILayout.Width(200), GUILayout.Height(30)))
            {
                SaveDataAsset();
            }
            GUILayout.Space(100);
            EditorGUILayout.EndHorizontal();
        });
        EditorGUILayout.EndHorizontal();
        LoadWave();
        EditorGUILayout.EndScrollView();
    }

    void LoadLevel()
    {
        if (_selectedIndex < 0)
        {
            Debug.Log("selection index must be > 0. " + _selectedIndex);
            return;
        }
        _level = _levels[_selectedIndex];
    }

    void LoadWave()
    {
        if (_level == null)
        {
            Debug.Log("level is null");
            return;
        }

        for (int i = 0; i < _level.Waves.Count; i++)
        {
            WaveInformation waves = _level.Waves[i];
            EditorGUILayout.BeginVertical();
            GUILayout.Space(20);
            EditorGUITool.BorderBox(5, 5, () =>
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Wave: " + i, EditorStyleExtension.WaveNameStyle, GUILayout.Width(100));
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUITool.Label("Type Wave", 100, 80, false);
                waves.TypeWave = (TypeOfWave)EditorGUILayout.EnumPopup(waves.TypeWave, GUILayout.Width(200));
                EditorGUILayout.EndHorizontal();

                switch (waves.TypeWave)
                {
                    case TypeOfWave.Enemies:
                        ShowWaveEnemy(waves);
                        break;
                    case TypeOfWave.Boss:
                        ShowWaveBoss(waves);
                        break;
                }
                EditorGUILayout.EndVertical();
            });

            EditorGUILayout.EndVertical();
        }
    }

    void ShowWaveEnemy(WaveInformation wave)
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Type Move", 100, 80, false);
        wave.TypeMove = (TypeMove)EditorGUILayout.EnumPopup(wave.TypeMove, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("row * col", 100, 80, false);

        if (wave.Row != 0 && wave.Col != 0)
        {
            _row = wave.Row;
            _col = wave.Col;
        }
        _row = EditorGUILayout.IntField(_row, GUILayout.Width(100));
        _col = EditorGUILayout.IntField(_col, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Delay Spawn", 100, 80, false);
        wave.DelaySpawn = EditorGUILayout.FloatField("", wave.DelaySpawn, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Dx", 100, 80, false);
        wave.Dx = EditorGUILayout.FloatField("", wave.Dx, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUITool.Label("Dy", 100, 80, false);
        wave.Dy = EditorGUILayout.FloatField("", wave.Dy, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUITool.Label("Size Dx", 100, 80, false);
        wave.SizeDx = EditorGUILayout.FloatField("", wave.SizeDx, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUITool.Label("Size Dy", 100, 80, false);
        wave.SizeDy = EditorGUILayout.FloatField("", wave.SizeDy, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        if (wave.TypeMove == TypeMove.MoveInRows || wave.TypeMove == TypeMove.MoveInLine)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUITool.Label("Delay Move", 100, 80, false);
            wave.DelaySpawn = EditorGUILayout.FloatField("", wave.DelaySpawn, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }
        switch (wave.TypeMove)
        {
            case TypeMove.OneByOne:
                MoveOneByOne(wave);
                break;
            case TypeMove.MoveInLine:
                MoveInLine(wave);
                break;
            case TypeMove.MoveInRows:
                MoveInRows(wave);
                break;
            case TypeMove.Random:
                MoveRandom(wave);
                break;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUITool.BorderBox(5, 5, () =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Drop Item", EditorStyleExtension.TitleNameStyle, GUILayout.Width(100));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Set Coin Drop", EditorStyleExtension.TitleForHeaderStyle, GUILayout.Width(100));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUITool.Label("Min Coin", 100, 80, false);
            wave.MinCoindDrop = EditorGUILayout.IntField("", wave.MinCoindDrop, GUILayout.Width(50));
            GUILayout.Space(20);
            EditorGUITool.Label("Max Coin", 100, 80, false);
            wave.MaxCoindDrop = EditorGUILayout.IntField("", wave.MaxCoindDrop, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Set List Item", EditorStyleExtension.TitleForHeaderStyle, GUILayout.Width(100));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUITool.Label("Size List", 100, 80, false);
            _sizeListDropItem = EditorGUILayout.IntField("", _sizeListDropItem, GUILayout.Width(50));
            if (_sizeListDropItem != wave.ListItemDop.Count)
            {
                wave.ListItemDop.Clear();
                for (int i = 0; i < _sizeListDropItem; i++)
                {
                    ItemDrop item = new ItemDrop();
                    wave.ListItemDop.Add(item);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < wave.ListItemDop.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("id", EditorStyleExtension.NormalTextStyle, GUILayout.Width(20));
                GUILayout.Space(10);
                wave.ListItemDop[i].IdItem =
                    EditorGUILayout.IntField("", wave.ListItemDop[i].IdItem, GUILayout.Width(20));
                GUILayout.Space(10);
                EditorGUILayout.LabelField("count", EditorStyleExtension.NormalTextStyle, GUILayout.Width(20));
                GUILayout.Space(10);
                wave.ListItemDop[i].Count =
                    EditorGUILayout.IntField("", wave.ListItemDop[i].Count, GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        });
        EditorGUILayout.EndVertical();
    }
    void ShowWaveBoss(WaveInformation wave)
    {

    }

    void InitWave(WaveInformation wave)
    {

        if (wave.Enemies == null || wave.Row != _row || wave.Col != _col)
        {
            wave.Enemies.Clear();
            wave.Row = _row;
            wave.Col = _col;
            for (int i = 0; i < _row * _col; i++)
            {
                EnemyInformation enemy = new EnemyInformation();
                enemy.IdEnemy = 1;
                enemy.Health = 100;
                enemy.IdPath = 0;
                wave.Enemies.Add(enemy);
            }
        }
        else return;
    }

    #region Type Move

    void MoveOneByOne(WaveInformation wave)
    {
        InitWave(wave);
        if (wave.Enemies != null)
        {
            if (_col < 1 || _row < 1) return;
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
            EditorGUITool.BorderBox(5, 10, () =>
            {
                GUILayout.Space(10);
                for (int i = 0; i < wave.Enemies.Count; i++)
                {
                    if ((i) % _col == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }
                    wave.Enemies[i].IdEnemy = EditorGUILayout.IntField("", wave.Enemies[i].IdEnemy, GUILayout.Width(50));
                    if (i % _col == (_col - 1))
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
            });
            EditorGUILayout.EndVertical();
        }
        else
        {
            LoadLevel();
        }
    }

    void MoveInLine(WaveInformation wave)
    {

    }

    void MoveInRows(WaveInformation wave)
    {

    }

    void MoveRandom(WaveInformation wave)
    {

    }

    #endregion
    #region IO

    void LoadAsset()
    {
        _levels = Resources.LoadAll<LevelInformation>("Maps");
    }

    void SaveDataAsset()
    {
        EditorUtility.SetDirty(_levels[_selectedIndex]);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        LoadAsset();
        this.ShowNotification(new GUIContent("Save data"));
    }

    #endregion



}
#endif
