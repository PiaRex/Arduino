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
    private GameObject rightPanelDelete;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        rightPanelDelete = Window.instance.RightPanelDelete;

        rightPanelDelete.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("DRAGDROP      OnPointerDown");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dropParent = transform.parent;
        rectTransform.GetComponentInChildren<Image>().raycastTarget = false;
        rectTransform.SetParent(Window.instance.transform);

        rightPanelDelete.SetActive(true);
        print("DRAGDROP      OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.GetComponentInChildren<Image>().raycastTarget = true;
        rectTransform.SetParent(dropParent);
        transform.localPosition = Vector3.zero;

        rightPanelDelete.SetActive(false);
        print("DRAGDROP      OnEndDrag");
    }
}

