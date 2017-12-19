using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     scriptable chứa thông tin lưu được của 1 level
///     các thông tin này sẽ được đọc từ file map được tạo ra khi vào từng level
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "level", menuName = "Create Map/map", order = 1)]
public class LevelInformation : ScriptableObject
{
    public int temp = 1;
    public List<WaveInformation> Waves = new List<WaveInformation>();
}

[Serializable]
public class WaveInformation
{
    public int IdWave;
    // số hàng ma trận
    public int Row = 0;
    // số cột ma trận
    public int Col = 0;
    // thời gian delay sinh ra
    public float DelaySpawn = 0;
    // thời gian delay di chuyển sau khi spawn - sử dụng trong cách di chuyển theo hàng, cột
    public float DelayMove = 0;
    // vị trí x con đầu tiên
    public float Dx = 0;
    // vị trí y con đầu tiên
    public float Dy = 0;
    // khoảng cách x cả đội hình
    public float SizeDx = 0;
    // khoảng cách y cả đội hình
    public float SizeDy = 0;
    // chỉ số đánh dấu ma trận thay đổi
    public int DeltaMatrix = 0;
    public TypeOfWave TypeWave;
    public TypeMove TypeMove;
    [SerializeField]
    public List<EnemyInformation> Enemies = new List<EnemyInformation>();
    /// <summary>
    /// số lượng coind drop trên wave
    /// </summary>
    public int MinCoindDrop = 0;

    public int MaxCoindDrop = 0;

    public List<ItemDrop> ListItemDop = new List<ItemDrop>();

}

[Serializable]
public class EnemyInformation
{
    public int IdEnemy = 01;
    public TypeOfEnemy Type = TypeOfEnemy.Minion;
    public int Health = 100;
    public int IdPath = 0;
}

[Serializable]
public class ItemDrop
{
    // id loại item
    public int IdItem = 0;
    // số lượng
    public int Count = 0;
}
public enum TypeOfEnemy
{
    Minion,
    Boss
}

public enum TypeMove
{
    // random cách di chuyển
    Random,
    // từng con một
    OneByOne,
    // di chuyển theo cột
    MoveInRows,
    // di chuyển theo hàng
    MoveInLine,
    // không theo path
    // optional - dùng thuật toán di chuyển đến vị trí có sẵn
    None
}

public enum TypeOfWave
{
    Boss,
    Enemies
}