using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollLoop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Action OnFocusComplete;
    public bool UseAlpha;
    // List Items
    public List<RectTransform> Items;
    public List<CanvasGroup> CanvasItems;

    public Canvas MyCanvas;
    public int StartIndex = 0;
    public int CurIndex;
    public int With = 100;
    public float Limit;
    public float Limit2 = 400;
    public float ScaleRate;
    Vector3 scale = Vector3.one;
    public Vector2 _oldPosition;
    public Vector2 _newPosition;
    public RectTransform CurCenter;
    public RectTransform CurChoose;
    public bool IsFocusing;
    public bool IsDrawing;
    public Vector3 DownPosition;
    public bool AutoOrder;
    public float MinScale = .5f;

    void Start()
    {
        if (AutoOrder)
            InitWithStartIndex(StartIndex);
    }
    [ContextMenu("Focus")]
    public void FocusWithID()
    {
        FocusItem(Items[StartIndex]);
    }

    [ContextMenu("Test Order")]
    public void TestOrder()
    {
        InitWithStartIndex(StartIndex);
    }
    
    public void InitWithStartIndex(int startIndex)
    {
        int index;

        // Get list item
        Items = new List<RectTransform>();

        foreach (RectTransform child in transform)
        {
            CanvasItems.Add(child.gameObject.AddComponent<CanvasGroup>());

            Items.Add(child);
            if (child.GetComponent<EventTrigger>() == null)
                child.gameObject.AddComponent<EventTrigger>();
            var child1 = child;
            var eventTrigger = child.GetComponent<EventTrigger>().triggers;
            var eventClick = new EventTrigger.Entry();
            eventClick.eventID = EventTriggerType.PointerClick;
            eventClick.callback.AddListener(arg0 =>
            {
                if (Vector2.Distance(GetMousePosition(), DownPosition) <= .1f)
                    FocusItem(child1);
            });
            eventTrigger.Add(eventClick);

            var eventDown = new EventTrigger.Entry();
            eventDown.eventID = EventTriggerType.PointerDown;
            eventDown.callback.AddListener(arg0 =>
            {
                DownPosition = GetMousePosition();
                OnBeginDrag(arg0 as PointerEventData);
            });
            eventTrigger.Add(eventDown);

            var eventBeginDrag = new EventTrigger.Entry();
            eventBeginDrag.eventID = EventTriggerType.BeginDrag;
            eventBeginDrag.callback.AddListener(arg0 =>
            {
                OnBeginDrag(arg0 as PointerEventData);
            });
            eventTrigger.Add(eventBeginDrag);

            var eventdrag = new EventTrigger.Entry();
            eventdrag.eventID = EventTriggerType.Drag;
            eventdrag.callback.AddListener(arg0 => OnDrag(arg0 as PointerEventData));
            eventTrigger.Add(eventdrag);

            var eventenddrag = new EventTrigger.Entry();
            eventenddrag.eventID = EventTriggerType.EndDrag;
            eventenddrag.callback.AddListener(arg0 => OnEndDrag(arg0 as PointerEventData));
            eventTrigger.Add(eventenddrag);

            child.name = "Item" + Items.Count;
        }

        // order list index
        List<int> leftIndexs = new List<int>();
        List<int> rightIndexs = new List<int>();
        int number = (int)(Items.Count / 2);

        for (int i = 1; i <= number; i++)
        {
            index = startIndex - i;
            if (index < 0)
                index += Items.Count;
            leftIndexs.Add(index);
        }

        if (Items.Count % 2 != 0)
            number += 1;

        for (int i = 0; i < number; i++)
        {
            index = startIndex + i;
            if (index > Items.Count - 1)
                index -= Items.Count - 1;
            rightIndexs.Add(index);
        }

        // display
        ScaleRate = With * number;
        ScaleRate = Limit2;
        Limit = With * number;
        for (int i = 0; i < rightIndexs.Count; i++)
        {
            Items[rightIndexs[i]].anchoredPosition = new Vector2(With * i, 0);
            scale.x = scale.y = 1 - Mathf.Abs(Items[rightIndexs[i]].anchoredPosition.x) / Limit2;
            if (scale.x < 0)
                scale.x = scale.y = .1f;
            Items[rightIndexs[i]].localScale = scale;

            CanvasItems[i].alpha = 1 - Mathf.Abs(Items[rightIndexs[i]].anchoredPosition.x) / Limit;
        }
        for (int i = 0; i < leftIndexs.Count; i++)
        {
            Items[leftIndexs[i]].anchoredPosition = new Vector2(-With * (i + 1), 0);
            scale.x = scale.y = 1 - Mathf.Abs(Items[leftIndexs[i]].anchoredPosition.x) / Limit;
            if (scale.x < 0)
                scale.x = scale.y = .1f;
            Items[leftIndexs[i]].localScale = scale;

            CanvasItems[i].alpha = 1 - Mathf.Abs(Items[rightIndexs[i]].anchoredPosition.x) / Limit;
        }
    }

    public void FocusItem(RectTransform rect)
    {
        if (IsFocusing)
            return;

        IsFocusing = true;
        CurCenter = rect;
        CurChoose = rect;

        CurIndex = CurCenter.transform.GetSiblingIndex();

        _oldPosition = CurCenter.anchoredPosition;
        CurCenter.DOAnchorPosX(0, .2f).OnUpdate(() =>
         {
             UpdatePosition(CurCenter.anchoredPosition - _oldPosition);
             _oldPosition = CurCenter.anchoredPosition;
         }).OnComplete(() =>
         {
             IsFocusing = false;
             CurCenter = null;
             if (OnFocusComplete != null)
                 OnFocusComplete();
         });
    }

    public void UpdatePosition(Vector2 delta)
    {
        delta.y = 0;
        for (var index = 0; index < Items.Count; index++)
        {
            var rectTransform = Items[index];
            if (CurCenter == null || (CurCenter != null && CurCenter != rectTransform))
            {
                rectTransform.anchoredPosition += delta;
            }
            if (delta.x < 0)
            {
                if (rectTransform.anchoredPosition.x < -Limit)
                {
                    rectTransform.anchoredPosition += Items.Count * With * Vector2.right;
                }
            }
            else
            {
                if (rectTransform.anchoredPosition.x > Limit)
                {
                    rectTransform.anchoredPosition += Items.Count * With * Vector2.left;
                }
            }
            scale.x = scale.y = 1 - Mathf.Abs(rectTransform.anchoredPosition.x) / Limit2;
            rectTransform.localScale = scale;
            CanvasItems[index].alpha = 1 - Mathf.Abs(rectTransform.anchoredPosition.x) / Limit;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsFocusing)
            return;
        _oldPosition = GetMousePosition();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsFocusing)
            return;

        // Get center Item;
        int index = 0;
        for (int i = 0; i < Items.Count; i++)
            if (Mathf.Abs(Items[i].anchoredPosition.x) < Mathf.Abs(Items[index].anchoredPosition.x))
                index = i;

        FocusItem(Items[index]);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsFocusing)
            return;
        CurChoose = null;
        _newPosition = GetMousePosition();
        UpdatePosition(_newPosition - _oldPosition);
        _oldPosition = _newPosition;
    }
    Vector2 pos;
    public Vector3 GetMousePosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas.transform as RectTransform, Input.mousePosition, MyCanvas.worldCamera, out pos);
        return pos;
    }

    public void NextItemClick()
    {
        CurIndex++;
        if (CurIndex >= Items.Count)
            CurIndex = 0;
        FocusItem(Items[CurIndex]);
    }

    public void BackItemClick()
    {
        CurIndex--;
        if (CurIndex < 0)
            CurIndex = Items.Count - 1;
        FocusItem(Items[CurIndex]);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Items.Count > 0)
        {
            foreach (var item in Items)
            {
                UnityEditor.Handles.Label(item.position, item.name);
            }
        }

        UpdatePosition(Vector2.zero);
    }
#endif
}

