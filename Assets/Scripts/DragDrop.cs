using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    public Transform dropParent;
    private Canvas canvas;
    bool isProgramRunning;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        EventManager.AddListener(EventNames.StartProgramEvent, HandleStartProgramEvent);
        EventManager.AddListener(EventNames.StopProgramEvent, HandleStopProgramEvent);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isProgramRunning)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isProgramRunning)
        {
            dropParent = transform.parent;
            rectTransform.GetComponentInChildren<Image>().raycastTarget = false;
            rectTransform.SetParent(Window.instance.transform);
            print("OnBeginDrag");
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isProgramRunning)
        {
            rectTransform.GetComponentInChildren<Image>().raycastTarget = true;
            rectTransform.SetParent(dropParent);
            transform.localPosition = Vector3.zero;
            print("OnEndDrag");
        }
    }
    void HandleStartProgramEvent()
    {
        isProgramRunning = true;
        print(isProgramRunning);
    }
    void HandleStopProgramEvent()
    {

        print(isProgramRunning);

    }
    void Update()
    {
        print(isProgramRunning);
    }
}

