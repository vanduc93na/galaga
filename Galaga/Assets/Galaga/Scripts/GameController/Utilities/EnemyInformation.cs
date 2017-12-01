using Boo.Lang;
using UnityEngine;

/// <summary>
///     scriptable chứa thông tin lưu được của 1 level
///     các thông tin này sẽ được đọc từ file map được tạo ra khi vào từng level
/// </summary>
public class LevelInformation : ScriptableObject
{

    public List<EnemyInformation> listEnemy = new List<EnemyInformation>();
}

public class EnemyInformation : MonoBehaviour
{
    public int Health;
    public bool IsDead;
}