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
        Init();
    }

    void Init()
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
        Debug.Log("Pass level: " + InventoryHelper.Instance.UserInventory.passLevel);
        for (int i = 0; i < _levels.Length; i++)
        {
            if (i <= InventoryHelper.Instance.UserInventory.passLevel)
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
        var listWps = _path.GetComponent<DOTweenPath>().wps;
        for (int i = 0; i < listWps.Count - 1; i++)
        {
            Vector3 direction = listWps[i + 1] - listWps[i];
            direction.Normalize();
            Vector3 linePos = listWps[i];
            while (true)
            {
                if (linePos.y >= listWps[i + 1].y) break;
                var lighSpawn = Lean.LeanPool.Spawn(_light);
                lighSpawn.transform.SetParent(_lightParent.transform);
                lighSpawn.transform.position = linePos;
                lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                linePos += direction * _deltaLight;
            }
        }
        //        for (int i = 0; i < listWps.Count - 1; i++)
        //        {
        //            if (i % 4 == 0)
        //            {
        //                float posX = listWps[i].x;
        //                while (posX + _deltaLight < listWps[i+1].x)
        //                {
        //                    posX += _deltaLight;
        //                    var lighSpawn = Lean.LeanPool.Spawn(_light);
        //                    lighSpawn.transform.SetParent(_lightParent.transform);
        //                    lighSpawn.transform.position = new Vector3(posX, listWps[i].y);
        //                    lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //                }
        //            }
        //            if (i % 4 == 1 || i % 4 == 3)
        //            {
        //                float posY = listWps[i].y + _deltaLight;
        //                while (posY < listWps[i + 1].y - 0.3f)
        //                {
        //                    var lighSpawn = Lean.LeanPool.Spawn(_light);
        //                    lighSpawn.transform.SetParent(_lightParent.transform);
        //                    lighSpawn.transform.position = new Vector3(listWps[i].x, posY);
        //                    lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //                    posY += _deltaLight;
        //                }
        //            }
        //            if (i % 4 == 2)
        //            {
        //                float posX = listWps[i].x;
        //                while (posX - _deltaLight > listWps[i + 1].x)
        //                {
        //                    posX -= _deltaLight;
        //                    var lighSpawn = Lean.LeanPool.Spawn(_light);
        //                    lighSpawn.transform.SetParent(_lightParent.transform);
        //                    lighSpawn.transform.position = new Vector3(posX, listWps[i].y);
        //                    lighSpawn.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //                }
        //            }
        //        }
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