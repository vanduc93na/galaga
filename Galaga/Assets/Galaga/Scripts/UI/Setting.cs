using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Setting : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text _musicButtonText;
    [SerializeField] private Text _soundButtonText;
    [SerializeField] private RectTransform _groupsItem;

    private bool _soundOn;
    private bool _musicOn;

    void Start()
    {
        Init();
    }

    void OnEnable()
    {
        _groupsItem.anchoredPosition = new Vector2(0, -_groupsItem.rect.height - 70);
        _groupsItem.DOAnchorPosY(0, .7f).SetUpdate(true);
    }

    void Init()
    {
        if (PlayerPrefs.HasKey(StringKeys.SOUND))
        {
            _soundOn = PlayerPrefs.GetInt(StringKeys.SOUND) == 1 ? true : false;
        }
        else
        {
            _soundOn = true;
            PlayerPrefs.SetInt(StringKeys.SOUND, 1);
        }

        if (PlayerPrefs.HasKey(StringKeys.MUSIC))
        {
            _musicOn = PlayerPrefs.GetInt(StringKeys.MUSIC) == 1 ? true : false;
        }
        else
        {
            _musicOn = true;
            PlayerPrefs.SetInt(StringKeys.MUSIC, 1);
        }
    }

    public void SoundControl()
    {
        _soundOn = !_soundOn;
        if (_soundOn)
        {
            PlayerPrefs.SetInt(StringKeys.SOUND, 1);
            AudioListener.pause = false;
            AudioListener.volume = 1f;
            _soundButtonText.text = "SOUND FX: ON";
        }
        else
        {
            PlayerPrefs.SetInt(StringKeys.SOUND, 0);
            AudioListener.pause = true;
            AudioListener.volume = 0;
            _soundButtonText.text = "SOUND FX: OFF";
        }
    }

    public void MusicControl()
    {
        _musicOn = !_musicOn;
        if (_musicOn)
        {
            PlayerPrefs.SetInt(StringKeys.MUSIC, 1);
            GetComponent<AudioSource>().Play();
            _musicButtonText.text = "BG MUSIC: ON";
        }
        else
        {
            PlayerPrefs.SetInt(StringKeys.MUSIC, 0);
            GetComponent<AudioSource>().Pause();
            _musicButtonText.text = "BG MUSIC: OFF";
        }
    }

    public void Close()
    {
        _groupsItem.DOAnchorPosY(-_groupsItem.rect.height - 70, 0.7f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }).SetUpdate(true);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            Close();
        }
    }
}
