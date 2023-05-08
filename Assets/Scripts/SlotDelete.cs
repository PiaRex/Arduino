using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDelete : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;
        var otherItem = otherItemTransform.GetComponent<DragDrop>();
        if (otherItem.dropParent == Window.instance.elementsPanel)
        {
            Debug.Log("РОДИТЕЛЬ ЭЛЕМЕНТ ПАНЕЛЬ" + otherItem.dropParent);
        }
        else
        {
            // уничтожить текущий элемент
            Destroy(otherItem.gameObject);
            Debug.Log("Item deleted");
        }

    }


}


