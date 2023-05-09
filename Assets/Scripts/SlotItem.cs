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
        var otherItemTransform = eventData.pointerDrag.transform;
        var otherItem = otherItemTransform.GetComponent<DragDrop>();

        var index = transform.GetSiblingIndex();

        Debug.Log("SLOTITEM" + otherItem.dropParent + " is at index " + index + " in the array.");

        // Check if the position is already occupied
        for (int i = 0; i < otherItem.dropParent.childCount; i++)
        {
            if (otherItem.dropParent.GetChild(i).GetComponent<SlotItem>().transform.GetSiblingIndex() == index)
            {
                // If occupied, remove existing item and shift others
                otherItem.dropParent.GetChild(i).GetComponent<SlotItem>().transform.SetSiblingIndex(index);

            }
        }
    }
}
