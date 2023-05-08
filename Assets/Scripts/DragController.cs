using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;
    public GridLayoutGroup grid;

    private RectTransform rectTransform;
    public Transform dropParent;
    private Canvas canvas;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPossition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentTransform.position = eventData.position;

        for (int i = 0; i < mainContent.transform.childCount; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                RectTransform otherTransform = mainContent.transform.GetChild(i) as RectTransform;

                float cellWidth = grid.cellSize.x + grid.spacing.x;
                float cellHeight = grid.cellSize.y + grid.spacing.y;

                float xDistance = Mathf.Abs(currentTransform.anchoredPosition.x - otherTransform.anchoredPosition.x);
                float yDistance = Mathf.Abs(currentTransform.anchoredPosition.y - otherTransform.anchoredPosition.y);

                if (xDistance <= cellWidth && yDistance <= cellHeight)
                {
                    Vector3 otherTransformOldPosition = otherTransform.position;
                    int currentIndex = currentTransform.GetSiblingIndex();
                    currentTransform.SetParent(mainContent.transform);
                    currentTransform.SetSiblingIndex(i);
                    otherTransform.SetParent(mainContent.transform);
                    otherTransform.SetSiblingIndex(currentIndex);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dropParent = transform.parent;
        rectTransform.GetComponent<Image>().raycastTarget = false;
        rectTransform.SetParent(Window.instance.transform);
        print("OnBeginDrag");
    }

}