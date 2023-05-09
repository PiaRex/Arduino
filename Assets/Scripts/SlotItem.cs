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

        Debug.Log("SLOTITEM" + dragItem.dropParent.name + " is at index " + index + " in the array.");

        // поместить элемент в родителя по указанному индексу
        // Get the drag object and its parent
        transform.SetSiblingIndex(index);



        print("OnEndDrag");
    }
}
