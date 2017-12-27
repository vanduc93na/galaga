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
    private int _sizePath = 0;
    private Dictionary<int, int> _healthEnemies = new Dictionary<int, int>();
    private Dictionary<int, int> _pathRoadToScreen = new Dictionary<int, int>();
    private int _deltaEnemyMatrix = -1;

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
        EditorGUITool.Label("Clear Enemies", 100, 80, false, "giết hết quái để qua wave ?");
        wave.ClearEnemiesToCompleteWave = EditorGUILayout.Toggle(wave.ClearEnemiesToCompleteWave);
        EditorGUILayout.EndHorizontal();
        if (!wave.ClearEnemiesToCompleteWave)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUITool.Label("Time Complete", 100, 80, false, "Thời gian hoàn thành wave");
            wave.TimeCompleteWave = EditorGUILayout.FloatField("", wave.TimeCompleteWave, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Type Move", 100, 80, false);
        wave.TypeMove = (TypeMove)EditorGUILayout.EnumPopup(wave.TypeMove, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("row * col", 100, 80, false, "độ lớn ma trận");

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
        EditorGUITool.Label("Overlapping", 100, 80, false, "sắp xếp chéo đội hình trong ma trận");
        wave.OverLapping = EditorGUILayout.Toggle(wave.OverLapping);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Delay Spawn", 100, 80, false, "thời gian delay giữa 2 con đi cùng nhau");
        wave.DelaySpawn = EditorGUILayout.FloatField("", wave.DelaySpawn, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Dx", 100, 80, false, "vị trí x của con đầu tiên");
        wave.Dx = EditorGUILayout.FloatField("", wave.Dx, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUITool.Label("Dy", 100, 80, false, "vị trí ý của con đầu tiên");
        wave.Dy = EditorGUILayout.FloatField("", wave.Dy, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUITool.Label("Size Dx", 100, 80, false, "độ lớn x cả đội hình");
        wave.SizeDx = EditorGUILayout.FloatField("", wave.SizeDx, GUILayout.Width(50));
        GUILayout.Space(20);
        EditorGUITool.Label("Size Dy", 100, 80, false, "độ lớn y cả đội hình");
        wave.SizeDy = EditorGUILayout.FloatField("", wave.SizeDy, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Delay Move", 100, 80, false, "thời gian delay giữa các đợt - từng path");
        wave.DelayMove = EditorGUILayout.FloatField("", wave.DelayMove, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUITool.Label("Custom Speed", 100, 80,false, " Tùy chọn tốc độ bay \n Tốc độ thật của enemy sẽ được cộng từ tốc độ cơ bản của path và tốc độ này \n Giá trị này có thể âm");
        wave.CustomSpeed = EditorGUILayout.FloatField("", wave.CustomSpeed, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        SetMatrix(wave);
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
        AddKeyDictionary(wave);

        EditorGUILayout.BeginVertical();
        EditorGUITool.BorderBox(5, 5, () =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Path Enemy", EditorStyleExtension.TitleNameStyle, GUILayout.Width(100));
            GUILayout.Space(10);
            AddKeyForPath(wave);
            switch (wave.TypeMove)
            {
                case TypeMove.OneByOne:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUITool.Label("Type Sort", 100, 80, false, "kiểu sắp xếp vào ma trận");
                    wave.TypeSort = (TypeSort)EditorGUILayout.EnumPopup(wave.TypeSort, GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginVertical();
                    var listKey = _pathRoadToScreen.Keys.ToList();
                    listKey.Sort();
                    for (int i = 0; i < listKey.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Id Path Of IdEnemy = " + listKey[i],
                            EditorStyleExtension.NormalTextStyle, GUILayout.Width(50));
                        GUILayout.Space(100);
                        _pathRoadToScreen[listKey[i]] = EditorGUILayout.IntField("", _pathRoadToScreen[listKey[i]], GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                    }
                    for (int i = 0; i < wave.Enemies.Count; i++)
                    {
                        if (_pathRoadToScreen.ContainsKey(wave.Enemies[i].IdEnemy))
                        {
                            wave.Enemies[i].IdPath = _pathRoadToScreen[wave.Enemies[i].IdEnemy];
                        }
                    }
                    EditorGUILayout.EndVertical();
                    break;
                case TypeMove.MoveInRows:
                    break;
                case TypeMove.MoveInLine:
                    break;
                case TypeMove.Random:
                    break;
                case TypeMove.None:
                    break;
            }
            
            EditorGUILayout.EndVertical();
        });
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUITool.BorderBox(5, 5, () =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Health Enemy", EditorStyleExtension.TitleNameStyle, GUILayout.Width(100));
            GUILayout.Space(10);
            var listKey = _healthEnemies.Keys.ToList();
            listKey.Sort();
            for (int i = 0; i < listKey.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("id = " + listKey[i],
                    EditorStyleExtension.NormalTextStyle, GUILayout.Width(50));
                GUILayout.Space(10);
                _healthEnemies[listKey[i]] = EditorGUILayout.IntField("", _healthEnemies[listKey[i]], GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        });
        for (int i = 0; i < wave.Enemies.Count; i++)
        {
            if (_healthEnemies.ContainsKey(wave.Enemies[i].IdEnemy))
            {
                wave.Enemies[i].Health = _healthEnemies[wave.Enemies[i].IdEnemy];
            }
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
            EditorGUITool.Label("Min Coin", 100, 80, false, "số lượng coin nhỏ nhất");
            wave.MinCoindDrop = EditorGUILayout.IntField("", wave.MinCoindDrop, GUILayout.Width(50));
            GUILayout.Space(20);
            EditorGUITool.Label("Max Coin", 100, 80, false, "số lượng coin lớn nhất");
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
                enemy.IdPath = -1;
                wave.Enemies.Add(enemy);
            }
        }
        else return;
    }

    #region Type Move

    void SetMatrix(WaveInformation wave)
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
                wave.DeltaMatrix = 0;
                for (int i = 0; i < wave.Enemies.Count; i++)
                {
                    wave.DeltaMatrix += wave.Enemies[i].IdEnemy * (i + 1);
                }
            });
            EditorGUILayout.EndVertical();
        }
        else
        {
            LoadLevel();
        }
    }

    void MoveOneByOne(WaveInformation wave)
    {

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

    #region Utilities

    /// <summary>
    /// kiểm tra _pathRoadToScreen nếu rỗng thì tạo mới
    /// nếu không rỗng thì lấy giá trị cũ theo từng type move của wave
    /// </summary>
    private void AddKeyForPath(WaveInformation wave)
    {
        switch (wave.TypeMove)
        {
            case TypeMove.OneByOne:
                _pathRoadToScreen = new Dictionary<int, int>();
                // kiểm tra có tồn tại idPath chưa
//                if (wave.Enemies.Count > 0 && wave.Enemies[0].IdPath == -1)
//                {
                    for (int i = 0; i < wave.Enemies.Count; i++)
                    {
                        if (!_pathRoadToScreen.ContainsKey(wave.Enemies[i].IdEnemy))
                        {
                            _pathRoadToScreen.Add(wave.Enemies[i].IdEnemy,
                                wave.Enemies[i].IdPath);
                        }
                    }
//                }
                // nếu chưa
//                else
//                {
//                    _pathRoadToScreen = new Dictionary<int, int>();
//                    for (int )
//                }
                break;
            case TypeMove.MoveInLine:
                break;
            case TypeMove.MoveInRows:
                break;
            case TypeMove.Random:
                break;
            case TypeMove.None:
                break;
        }
    }


    /// <summary>
    /// add key và vualue cho _healthEnemy
    /// nếu rỗng thì tạo mới, nếu không rỗng thì lấy giá trị trong map đã có
    /// </summary>
    /// <param name="wave"></param>
    private void AddKeyDictionary(WaveInformation wave)
    {
        int count = 0;
        for (int i = 0; i < wave.Enemies.Count; i++)
        {
            count += wave.Enemies[i].IdEnemy * (i + 1);
        }

        Dictionary<int, int> temp = new Dictionary<int, int>();
        for (int i = 0; i < wave.Enemies.Count; i++)
        {
            if (!temp.ContainsKey(wave.Enemies[i].IdEnemy))
            {
                temp.Add(wave.Enemies[i].IdEnemy, 1);
            }
            else
            {
                temp[wave.Enemies[i].IdEnemy] += 1;
            }
        }
        if (wave.DeltaMatrix != count || temp.Count != _healthEnemies.Count)
        {
            _deltaEnemyMatrix = count;
            _healthEnemies.Clear();
            foreach (int key in temp.Keys)
            {
                _healthEnemies.Add(key, 0);
            }
            var listKeySort = _healthEnemies.Keys.ToList();
            listKeySort.Sort();
            for (int i = 0; i < wave.Enemies.Count; i++)
            {
                if (_healthEnemies.ContainsKey(wave.Enemies[i].IdEnemy))
                {
                    _healthEnemies[wave.Enemies[i].IdEnemy] = wave.Enemies[i].Health;
                }
            }
        }
        else
        {
            for (int i = 0; i < wave.Enemies.Count; i++)
            {
                if (_healthEnemies.ContainsKey(wave.Enemies[i].IdEnemy))
                {
                    _healthEnemies[wave.Enemies[i].IdEnemy] = wave.Enemies[i].Health;
                }
            }
            return;
        }
    }

    #endregion

}
#endif
