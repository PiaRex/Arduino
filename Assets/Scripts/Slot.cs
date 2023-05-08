using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour, IDropHandler
{
    public List<BaseElementClass> items = new List<BaseElementClass>();
    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;
        var otherItem = otherItemTransform.GetComponent<DragDrop>();
        if (otherItem.dropParent == Window.instance.WorkSpaceGrid)
        {
            print("РОДИТЕЛЬ СПЭЙСВОРК" + otherItem.dropParent);
        }
        else
        {
            var newItem = Instantiate(otherItemTransform.gameObject, Window.instance.WorkSpaceGrid);
            newItem.transform.localPosition = Vector3.zero;
            items.Add(newItem.GetComponent<BaseElementClass>());
            print("Item added");
        }

    }


}


