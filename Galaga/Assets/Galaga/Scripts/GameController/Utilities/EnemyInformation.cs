using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     scriptable chứa thông tin lưu được của 1 level
///     các thông tin này sẽ được đọc từ file map được tạo ra khi vào từng level
/// </summary>
public class LevelInformation : ScriptableObject
{
    public List<WaveInformation> Waves = new List<WaveInformation>();
}

public class WaveInformation
{
    public int IdWave;
    public Dictionary<int, List<EnemyInformation>> Enemys = new Dictionary<int, List<EnemyInformation>>();
}

public class EnemyInformation
{
    public int IdEnemy;
    public TypeOfEnemy Type;
    public int Health;
}

public enum TypeOfEnemy
{
    Enemy,
    Gift
}