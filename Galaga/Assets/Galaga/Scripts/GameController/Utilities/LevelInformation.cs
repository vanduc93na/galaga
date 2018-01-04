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
    /// <summary>
    /// số hàng ma trận
    /// </summary>
    public int Row = 0;
    /// <summary>
    /// sắp xếp quái chồng chéo
    /// </summary>
    public bool OverLapping = false;
    /// <summary>
    /// số cột ma trận
    /// </summary>
    public int Col = 0;
    /// <summary>
    /// thời gian delay sinh ra
    /// </summary>
    public float DelaySpawn = 0;
    /// <summary>
    /// thời gian delay di chuyển sau khi spawn - sử dụng trong cách di chuyển theo hàng, cột
    /// </summary>
    public float DelayMove = 0;
    /// <summary>
    /// vị trí x con đầu tiên
    /// </summary>
    public float Dx = 0;

    /// <summary>
    /// vị trí y con đầu tiên
    /// </summary>
    public float Dy = 0;
    /// <summary>
    /// khoảng cách x cả đội hình
    /// </summary>
    public float SizeDx = 0;
    /// <summary>
    /// khoảng cách y cả đội hình
    /// </summary>
    public float SizeDy = 0;
    /// <summary>
    /// chỉ số đánh dấu ma trận thay đổi trên màn hình - không sử dụng trong game
    /// </summary>
    public int DeltaMatrix = 0;
    /// <summary>
    /// tốc độ tùy chỉnh, mặc định = 0 thì sẽ sử dụng tốc độ di chuyển của từng path
    /// tốc độ thật của enemy sẽ bằng tốc độ của path cộng với giá trị này(có thể âm)
    /// </summary>
    public float CustomSpeed = 0;
    /// <summary>
    /// điều kiện hoàn thành wave
    /// == true: phải giết hết quái mới complete wave
    /// == false: tính theo time để hoàn thành wave - dành cho những wave quái không sắp xếp thành ma trận mà sẽ list những quái bay theo đội hình tấn công
    /// </summary>
    public bool ClearEnemiesToCompleteWave = true;
    /// <summary>
    /// thời gian để hoàn thành wave nếu như wave không cần phải clear hết quái
    /// </summary>
    public float TimeCompleteWave = 0;

    public TypeSort TypeSort;
    public TypeOfWave TypeWave;
    public TypeMove TypeMove;
    /// <summary>
    /// danh sánh enemies
    /// </summary>
    public List<EnemyInformation> Enemies = new List<EnemyInformation>();
    /// <summary>
    /// số lượng coin nhỏ nhất drop trên wave
    /// </summary>
    public int MinCoindDrop = 0;
    /// <summary>
    /// số lượng coin lớn nhất drop trên wave
    /// </summary>
    public int MaxCoindDrop = 0;
    /// <summary>
    /// danh sách các item drop trong 1 wave
    /// </summary>
    public List<ItemDrop> ListItemDop = new List<ItemDrop>();

    public WaveBoss WaveBossInformation = new WaveBoss();

}

[Serializable]
public class EnemyInformation
{
    public int IdEnemy = 1;
    public TypeOfEnemy Type = TypeOfEnemy.Minion;
    public int Health = 100;
    public int IdPath = -1;
}

[Serializable]
public class WaveBoss
{
    public List<BossInfor> BossInfors = new List<BossInfor>();
    /// <summary>
    /// biến kiểm tra xem con boss này có sinh quái ko
    /// </summary>
    public bool IsSpawnEnemies = false;
    /// <summary>
    /// danh sách enemy sẽ được sinh ra từ boss. Enemy sẽ được ngẫu nhiên chọn trong danh sách này
    /// </summary>
    public List<EnemyInformation> EnemySpawns = new List<EnemyInformation>();
    /// <summary>
    /// kiểu sinh quái từ boss
    /// </summary>
    public TypeSpawnBlock TypeSpawn;
    /// <summary>
    /// số lượng con sinh ra trong 1 block
    /// </summary>
    public int CountBlock = 0;
    /// <summary>
    /// thời gian delay giữa 2 con boss
    /// </summary>
    public float DelaySpawnBoss = 0;
    /// <summary>
    /// thời gian delay giữa 2 enemy sinh ra từ boss
    /// </summary>
    public float DelaySpawnenemy = 0;
    /// <summary>
    /// số lượng coin nhỏ nhất drop
    /// </summary>
    public int MinCoin = 0;
    /// <summary>
    /// số lượng coin lớn nhất drop
    /// </summary>
    public int MaxCoin = 0;

    public List<int> ListItemDrop = new List<int>();
}

[Serializable]
public class BossInfor
{
    public int IdBoss = 0;
    public int Health = 0;
    /// <summary>
    /// path move to screen
    /// </summary>
    public int IdPath = 0;

    public int MinCoin = 0;
    public int MaxCoin = 0;
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
    Gift
}

/// <summary>
/// di chuyển
/// </summary>
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
/// <summary>
/// kiểu wave
/// </summary>
public enum TypeOfWave
{
    Boss,
    Enemies
}

/// <summary>
/// kiểu sắp xếp vào ma trận
/// </summary>
public enum TypeSort
{
    LeftToRightAndTopDown,
    BottomUpAndRightToLeft,
    SpiralMatrix
}
/// <summary>
/// kiểu sinh quái từ boss
/// </summary>
public enum TypeSpawnBlock
{
    FromLeft,
    FromRight,
    Both
}