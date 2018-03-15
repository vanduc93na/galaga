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

    private Button[] _levels;
    private List<Vector3> _pathVector3s;
    private List<GameObject> _lightList;

    void Awake()
    {
        //        _scroll.enabled = false;
        _levels = _content.GetComponentsInChildren<Button>();
        for (int i = 0; i < _levels.Length; i++)
        {
            Button btn = _levels[i];
            _levels[i].onClick.AddListener(() => ButtonsSelect(btn));
        }
        _pathVector3s = _path.GetComponent<DOTweenPath>().wps;
    }
    
    void Start()
    {
        _lightList = new List<GameObject>();
        CreateLight();
    }

    Vector3 GetAngle(Vector2 start, Vector2 end)
    {
        return new Vector3(0, 0, Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg - 90);
    }

    void ButtonsSelect(Button btn)
    {
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
        for (int i = 0; i < _pathVector3s.Count; i++)
        {
            GameObject lightSpawn = Lean.LeanPool.Spawn(_light);
            lightSpawn.transform.SetParent(_lightParent.transform);
            lightSpawn.transform.position = _pathVector3s[i];
            lightSpawn.transform.localScale = Vector3.one;
            lightSpawn.transform.eulerAngles = GetAngle(lightSpawn.transform.position, _pathVector3s[i]);
            _lightList.Add(lightSpawn);
        }
        DrawnLight();
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