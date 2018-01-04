using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : BaseBoss
{
    private const string NORMAL_ATTACK_METHOD = "NormalAttack";

    void OnEnable()
    {
        base.OnEnable();
        CancelInvoke(NORMAL_ATTACK_METHOD);
        Invoke(NORMAL_ATTACK_METHOD, 10f);
    }

    /// <summary>
    /// tấn công bằng cách sinh đạn
    /// </summary>
    void NormalAttack()
    {
        
    }

}
