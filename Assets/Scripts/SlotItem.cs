using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SlotItem : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var dragItemTransform = eventData.pointerDrag.transform;
        var dragItem = dragItemTransform.GetComponent<DragDrop>();

        var index = transform.GetSiblingIndex();

        Debug.Log("SLOTitem" + " Над чем рука: " + transform.name + "index " + index);




        if (dragItem.dropParent == Window.instance.WorkSpaceGrid)
        {
            // Move the item being dragged to the specified index in the parent object
            dragItemTransform.SetParent(transform.parent);
            dragItemTransform.SetSiblingIndex(index);
            print("SLOTitem  родитель воркспейс меняем местами OnEndDrag");
        }
        else
        {
            var newItem = Instantiate(dragItemTransform.gameObject, Window.instance.WorkSpaceGrid);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.SetSiblingIndex(index);

            // todo добавление нового айтема в массив

            newItem.GetComponentInChildren<Image>().raycastTarget = true;

            print("SLOTitem родитель правая панель добавляем новый в массив Item added");
        }
    }
}
