using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPage : MonoBehaviour
{
    [SerializeField] private GameObject _homePanel;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _path;
    [SerializeField] private GameObject _lightParent;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private Sprite _passLevelSprite;
    [SerializeField] private Sprite _unPassLevelSprite;
    [SerializeField] private float _deltaLight;

    private Button[] _levels;
    private List<Vector3> _pathVector3s;
    private List<GameObject> _lightList;

    void Awake()
    {
        _levels = _content.GetComponentsInChildren<Button>();
        _pathVector3s = _path.GetComponent<DOTweenPath>().wps;
    }

    void Start()
    {
        _lightList = new List<GameObject>();
        CreateLight();
    }

    void OnEnable()
    {
        InventoryHelper.Instance.LoadInventory();
        for (int i = 0; i < _levels.Length; i++)
        {
            Button btn = _levels[i];
            if (i < InventoryHelper.Instance.UserInventory.passLevel + 1)
            {
                _levels[i].onClick.AddListener(() => ButtonsSelect(btn));
            }

        }
        for (int i = 0; i < _levels.Length; i++)
        {
            if (i <= InventoryHelper.Instance.UserInventory.passLevel - 1)
            {
                _levels[i].GetComponent<Image>().sprite = _passLevelSprite;
            }
            else
            {
                _levels[i].GetComponent<Image>().sprite = _unPassLevelSprite;
            }
        }
    }

    Vector3 GetAngle(Vector2 start, Vector2 end)
    {
        return new Vector3(0, 0, Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg - 90);
    }

    void ButtonsSelect(Button btn)
    {
        SoundController.PlaySoundEffect(SoundController.Instance.Click);
        int level = btn.transform.GetSiblingIndex();
        print(level);
        InventoryHelper.Instance.AddSelectedLevel(level);
        SceneManager.LoadScene(1);
    }

    public void BackButton()
    {
        _homePanel.SetActive(true);
        gameObject.SetActive(false);
    }

    void CreateLight()
    {
        //        for (int i = 0; i < _pathVector3s.Count; i++)
        //        {
        //            GameObject lightSpawn = Lean.LeanPool.Spawn(_light);
        //            lightSpawn.transform.SetParent(_lightParent.transform);
        //            lightSpawn.transform.position = _pathVector3s[i];
        //            lightSpawn.transform.localScale = Vector3.one;
        //            lightSpawn.transform.eulerAngles = GetAngle(lightSpawn.transform.position, _pathVector3s[i]);
        //            _lightList.Add(lightSpawn);
        //        }
        //        DrawnLight();
        var listWps = _path.GetComponent<DOTweenPath>().wps;
        for (int i = 0; i < listWps.Count - 1; i++)
        {
            if (i % 4 == 0)
            {
                float posX = listWps[i].x;
                while (posX + _deltaLight < listWps[i+1].x)
                {
                    posX += _deltaLight;
                    var lighSpawn = Lean.LeanPool.Spawn(_light);
                    lighSpawn.transform.SetParent(_lightParent.transform);
                    lighSpawn.transform.position = new Vector3(posX, listWps[i].y);
                    lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
            if (i % 4 == 1 || i % 4 == 3)
            {
                float posY = listWps[i].y + _deltaLight;
                while (posY < listWps[i + 1].y - 0.3f)
                {
                    var lighSpawn = Lean.LeanPool.Spawn(_light);
                    lighSpawn.transform.SetParent(_lightParent.transform);
                    lighSpawn.transform.position = new Vector3(listWps[i].x, posY);
                    lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    posY += _deltaLight;
                }
            }
            if (i % 4 == 2)
            {
                float posX = listWps[i].x;
                while (posX - _deltaLight > listWps[i + 1].x)
                {
                    posX -= _deltaLight;
                    var lighSpawn = Lean.LeanPool.Spawn(_light);
                    lighSpawn.transform.SetParent(_lightParent.transform);
                    lighSpawn.transform.position = new Vector3(posX, listWps[i].y);
                    lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }
    }

    void DrawnLight()
    {
        for (int i = 0; i < _lightList.Count - 1; i++)
        {
            _lightList[i].transform.DOLocalMove(_lightList[i + 1].transform.localPosition, 0.8f).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}