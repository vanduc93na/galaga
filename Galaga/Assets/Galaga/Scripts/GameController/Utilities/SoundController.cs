using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public bool OnSound = true, OnMusic = true;
    [HideInInspector]
    public AudioSource BackgroundSound;
    [HideInInspector]
    public List<AudioSource> Pools;
    [HideInInspector]
    public List<AudioSource> Spawns;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        BackgroundSound = gameObject.AddComponent<AudioSource>();
    }

    public void PlayBg(AudioClip clip, float volume = 1f, bool loop = true)
    {
        CurrentBgClip = clip;
        if (OnMusic)
        {
            BackgroundSound.Stop();
            BackgroundSound.clip = clip;
            BackgroundSound.volume = volume;
            BackgroundSound.loop = loop;
            BackgroundSound.Play();
        }
    }

    public static AudioClip CurrentBgClip;
    public static void PlayBackgroundSound(AudioClip clip, float volume = 1, bool loop = true)
    {
        Instance.PlayBg(clip, volume, loop);
    }

    public static void PlayBackgroundSound(SoundInfor infor = null, bool loop = true)
    {
        if (infor == null)
            Instance.BackgroundSound.volume = 1;
        else
            Instance.PlayBg(infor.Clip, Instance.OnMusic ? infor.Volume : 0, loop);
    }

    public static void StopBackgroundSound()
    {
        Instance.BackgroundSound.volume = 0;
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if (!OnSound)
            return;

        if (Pools.Count > 0)
        {
            Pools[0].clip = clip;
            Pools[0].volume = volume;
            Pools[0].Play();
            Spawns.Add(Pools[0]);
            Pools.RemoveAt(0);
        }
        else
        {
            int count = Spawns.Count;
            bool has = false;
            for (int i = 0; i < count;)
            {
                if (!Spawns[i].isPlaying)
                {
                    has = true;
                    Pools.Add(Spawns[i]);
                    Spawns.RemoveAt(i);
                    count--;
                }
                else i++;
            }
            if (has)
            {
                PlaySoundEffect(clip, volume);
            }
            else
            {
                AudioSource sound = gameObject.AddComponent<AudioSource>();
                sound.clip = clip;
                sound.volume = volume;
                sound.Play();
                Spawns.Add(sound);
            }
        }
    }

    public static void PlaySoundEffect(SoundInfor infor)
    {
        if (infor.Clip == null)
            return;
        Instance.PlaySound(infor.Clip, infor.Volume);
    }

    public static void StopSoundEffect(SoundInfor infor)
    {
        if (infor.Clip == null) return;
    }

    public static void PlaySoundEffect(AudioClip clip, float volume = 1)
    {
        Instance.PlaySound(clip, volume);
    }

    public static void StopSoundEffect()
    {
        int count = Instance.Spawns.Count;
        for (int i = 0; i < count; i++)
        {
            Instance.Spawns[0].Stop();
            Instance.Pools.Add(Instance.Spawns[0]);
            Instance.Spawns.RemoveAt(0);
        }
    }

    public static void StopAll()
    {
        StopBackgroundSound();
        StopSoundEffect();
    }
}
