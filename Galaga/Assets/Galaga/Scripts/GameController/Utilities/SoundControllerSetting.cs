using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SoundController
{
    public SoundInfor MenuBackgroundSound;
    public SoundInfor GameBackgroundSound;

    public SoundInfor PlayerFireBullet;
    public SoundInfor Click;
    public SoundInfor EnemyHitBullet;
    public SoundInfor PlayerHitBullet;
    public SoundInfor EatCoin;
    public SoundInfor EatItem;
    public SoundInfor EnemyDead;
    public SoundInfor BossDead;

    public void SoundButtonClick()
    {
        PlaySoundEffect(Click.Clip, Click.Volume);
    }

    public void RunSound()
    {
        Invoke("SoundButtonClick", 0);
    }
}

[Serializable]
public class SoundInfor
{
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume = 1;
}