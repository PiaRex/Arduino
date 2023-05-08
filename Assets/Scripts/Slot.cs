using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;
        var newItem = Instantiate(otherItemTransform.gameObject, transform);
        newItem.transform.localPosition = Vector3.zero;
    }
}


