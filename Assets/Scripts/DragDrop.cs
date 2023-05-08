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



    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dropParent = transform.parent;
        rectTransform.GetComponent<Image>().raycastTarget = false;
        rectTransform.SetParent(Window.instance.transform);
        print("OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.GetComponent<Image>().raycastTarget = true;
        rectTransform.SetParent(dropParent);
        transform.localPosition = Vector3.zero;
        print("OnEndDrag");
    }
}

